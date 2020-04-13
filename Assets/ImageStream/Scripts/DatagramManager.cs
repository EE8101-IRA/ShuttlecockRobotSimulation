using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class DatagramManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField]
    private string ipAddress = "127.0.0.1";
    [SerializeField]
    private int port = 11000;
    #endregion

    #region private members
    private static bool run = false;
    private Thread clientSenderThread;
    #endregion

    // Awake is called before the first frame update
    void Awake()
    {
        // Start UDP Listener
        StartThread(clientSenderThread, new ThreadStart(UDPSenderProcess));
        // Start UDP Listener
        //StartThread(new ThreadStart(UDPListener));
    }

    private void StartThread(Thread thread, ThreadStart threadStart)
    {
        try
        {
            run = true;
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
        if (clientSenderThread != null)
            clientSenderThread.Abort();
    }

    #region UDP Sender
    private void UDPSenderProcess()
    {
        int counter = 0;
        using (var udpClient = new UdpClient())
        {
            try
            {
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //udpServer2.Client.Bind(port);
                udpClient.Connect(ipAddress, port);

                // Keeps sending the current frame until connection is closed
                while (run)
                {
                    byte[] sendBytes = Encoding.ASCII.GetBytes("Hello world! " + counter);  // temporary data

                    // Send bytes
                    udpClient.Send(sendBytes, sendBytes.Length);

                    // Increase counter
                    counter++;

                    // Sleep until next data to send
                    Thread.Sleep(1000); // wait for 1 second
                }
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("[INFO] {0}", ex.Message));
                Debug.Log(string.Format("[INFO] {0}", ex.StackTrace));
            }
        }
    }
    #endregion

    #region UDP Listener
    private void UDPListenerProcess()
    {
        using (var udpClient = new UdpClient(port))
        {
            while (true)
            {
                //IPEndPoint object will allow us to read datagrams sent from any source.
                //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                //Read datagrams sent from localhost.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                //string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Debug.Log("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

            }
        }
    }
    #endregion
}
