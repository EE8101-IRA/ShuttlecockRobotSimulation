using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ImageStreamManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private string ipAddress = "127.0.0.1";
    [SerializeField]
    private int outgoingPort = 11000;
    [SerializeField]
    private int incomingPort = 5005;
    [SerializeField]
    private RenderTexture renderTexture;

    [SerializeField]
    private int streamWidth = 320;
    [SerializeField]
    private int streamHeight = 240;
    #endregion

    [Header("For Testing")]
    [SerializeField]
    private FindMinMax findMinMax;

    #region private members
    private static bool run = false;
    private Thread clientReceiveThread;
    #endregion

    #region received bounding box data
    private readonly object responseLock = new object();
    private bool responseReceived;
    public bool ResponseReceived {
        get {
            lock (responseLock)
            {
                return responseReceived;
            }
        }
    }
    public bool ObjectDetected { get; private set; }

    private int xMin;
    private int xMax;
    private int yMin;
    private int yMax;

    public float CenterXNormalized {
        get {
            float centerX = 0.5f * (xMin + xMax);
            return (centerX / streamWidth);
        }
    }
    public float CenterYNormalized {
        get {
            float centerY = 0.5f * (yMin + yMax);
            return (centerY / streamHeight);
        }
    }
    public float YMinNormalized {
        get { return ((float)yMin / streamHeight); }
    }
    #endregion

    // Awake is called before the first frame update
    void Start()
    {
        run = true;

        // Start UDP Sender
        //StartCoroutine(SendImageOverUdp());
        // Start UDP Listener
        StartThread(clientReceiveThread, new ThreadStart(UDPListenerProcess));
    }

    private string TextureToBase64(RenderTexture renderTexture)
    {
        Texture2D texture2D = new Texture2D(streamWidth, streamHeight, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        string imageStr = Convert.ToBase64String(texture2D.EncodeToJPG());

        RenderTexture.active = null; // "just in case" 

        return imageStr;
    }

    private void StartThread(Thread thread, ThreadStart threadStart)
    {
        try
        {
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;

            thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("[INFO] {0}", ex.Message));
            Debug.Log(string.Format("[INFO] {0}", ex.StackTrace));
        }
    }


    void OnDestroy()
    {
        run = false;

        // free the Thread
        if (clientReceiveThread != null)
            clientReceiveThread.Abort();
    }
    
    /// <summary>
    /// Method to automatically send an image every 5 seconds. Call this method for testing purposes.
    /// </summary>
    private IEnumerator SendImageOverUdp()
    {
        while (run)
        {
            SendImage();
            yield return new WaitForSeconds(5f);    // wait for 5 seconds before sending again
        }
    }

    private void LocalTesting()
    {
        // for now, test using FindMinMax
        findMinMax.CalculateMinMax();
        xMin = findMinMax.MinX;
        yMin = findMinMax.MinY;
        xMax = findMinMax.MaxX;
        yMax = findMinMax.MaxY;

        if (xMin >= 640 && xMax < 1280 &&
            yMin >= 0 && yMax < streamHeight)
            ObjectDetected = true;
        else
            ObjectDetected = false;

        Debug.Log("XMin: " + xMin + "XMax: " + xMax);
        Debug.Log("YMin: " + yMin + "YMax: " + yMax);

        lock (responseLock)
        {
            responseReceived = true;
        }
    }

    public void SendImage()
    {
        lock (responseLock)
        {
            responseReceived = false;
        }

        // test without the neural network using FindMinMax script
        //LocalTesting();
        //return;

        string data = TextureToBase64(renderTexture);

        using (var udpClient = new UdpClient())
        {
            try
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);    // allow communication with a local server using same port
                                                                                                                    //udpServer2.Client.Bind(port);
                udpClient.Connect(ipAddress, outgoingPort);

                // Create byte[] data
                byte[] sendBytes = Convert.FromBase64String(data);
                // prepend length of base64 string before sending -- not needed
                //byte[] lengthBytes = BitConverter.GetBytes(imageBytes.Length);
                //byte[] sendBytes = new byte[imageBytes.Length + lengthBytes.Length];
                //sendBytes = lengthBytes.Concat(imageBytes).ToArray();

                Debug.Log(sendBytes.Length);

                // Send bytes
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("[INFO] {0}", ex.Message));
                Debug.Log(string.Format("[INFO] {0}", ex.StackTrace));
            }
        }
    }

    #region UDP Listener
    private void UDPListenerProcess()
    {
        using (var udpClient = new UdpClient(incomingPort))
        {
            while (true)
            {
                //IPEndPoint object will allow us to read datagrams sent from any source.
                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //Read datagrams sent from localhost.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receivedBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedBytes);
                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Debug.Log("Message: " + receivedMessage + "\nThis message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

                // parse received message
                string[] boundingBox = receivedMessage.Split(',');

                lock (responseLock)
                {
                    if (boundingBox.Length == 1)    // no object was detected
                    {
                        ObjectDetected = false;
                    }
                    else
                    {
                        ObjectDetected = true;
                        // XMin
                        xMin = int.Parse(boundingBox[0]);
                        // YMin
                        yMin = int.Parse(boundingBox[1]);
                        // XMax
                        xMax = int.Parse(boundingBox[2]);
                        // YMax
                        yMax = int.Parse(boundingBox[3]);
                    }

                    responseReceived = true;
                }
            }
        }
    }
    #endregion
}
