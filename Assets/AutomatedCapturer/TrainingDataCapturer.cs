using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TrainingDataCapturer : MonoBehaviour
{
    [SerializeField]
    private GameObject testingUI;
    [SerializeField]
    private GameObject capturesCompleteDisplay;

    [Header("Object")]
    [SerializeField]
    private Transform objectForCapture;
    [SerializeField]
    private FindMinMax objectMinMaxCalculator;

    [SerializeField]
    private InputField filenameIF;

    [Header("Capture Data")]
    [SerializeField]
    private int xAxisStart = 0;
    [SerializeField]
    private int xAxisRange = 360;
    [SerializeField]
    private int xAxisSkip = 1;
    [SerializeField]
    private int yAxisStart = 0;
    [SerializeField]
    private int yAxisRange = 360;
    [SerializeField]
    private int yAxisSkip = 1;
    [SerializeField]
    private int zAxisStart = 0;
    [SerializeField]
    private int zAxisRange = 360;
    [SerializeField]
    private int zAxisSkip = 1;


    // Start is called before the first frame update
    void Start()
    {
        capturesCompleteDisplay.SetActive(false);
        Debug.Log(Application.dataPath);
    }

    public void BeginCapture()
    {
        testingUI.SetActive(false);
        capturesCompleteDisplay.SetActive(false);

        StartCoroutine(Capture());
    }

    public void TakeSingleCapture()
    {
        // disable UI
        testingUI.SetActive(false);
        capturesCompleteDisplay.SetActive(false);

        StartCoroutine(SingleCapture());
    }

    private IEnumerator SingleCapture()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("filename,xmin,ymin,xmax,ymax,classid\n");

        // directory to save to
        string directory = Application.dataPath;
        directory = directory.Remove(directory.Length - 6, 6);    // remove "Assets" from the end of the string
        directory += "Captures";    // save to a Captures folder instead

        // check if directory exists; if it does not, create the necessary folders
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!Directory.Exists(directory + "/Screenshots"))
        {
            Directory.CreateDirectory(directory + "/Screenshots");
        }

        // get file name
        string prefixFilename = filenameIF.text;
        if (string.IsNullOrEmpty(prefixFilename))
            prefixFilename = "Image";   // default

        // calculate bounding box min and max values
        objectMinMaxCalculator.CalculateMinMax();

        // take screenshot
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filename = string.Format("{0}.png", prefixFilename);
        Debug.Log(filename);
        string filePath = Path.Combine(directory + "/Screenshots/", filename);
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // write values to the StringBuilder
        sb.Append(string.Format("{0},{1},{2},{3},{4},{5}\n",
                    filename,
                    objectMinMaxCalculator.MinX,
                    objectMinMaxCalculator.MinY,
                    objectMinMaxCalculator.MaxX,
                    objectMinMaxCalculator.MaxY,
                    0));

        yield return null;
        
        // export the built string to CSV
        string csvFilePath = Path.Combine(directory + string.Format("/data_{0}.csv", prefixFilename));
        File.WriteAllText(csvFilePath, sb.ToString());

        // re-enable UI
        capturesCompleteDisplay.SetActive(true);
        testingUI.SetActive(true);
    }

    private IEnumerator Capture()
    {
        int count = 0;
        StringBuilder sb = new StringBuilder();
        sb.Append("filename,xmin,ymin,xmax,ymax,classid\n");

        // directory to save to
        string directory = Application.dataPath;
        directory = directory.Remove(directory.Length-6, 6);    // remove "Assets" from the end of the string
        directory += "Captures";    // save to a Captures folder instead

        // check if directory exists; if it does not, create the necessary folders
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!Directory.Exists(directory + "/Screenshots"))
        {
            Directory.CreateDirectory(directory + "/Screenshots");
        }

        // get file name
        string prefixFilename = filenameIF.text;
        if (string.IsNullOrEmpty(prefixFilename))
            prefixFilename = "Image";   // default

        // rotate object & capture screenshot
        for (int i = xAxisStart; i < xAxisStart + xAxisRange; i += xAxisSkip)
        {
            for (int j = yAxisStart; j < yAxisStart + yAxisRange; j += yAxisSkip)
            {
                if (i == 90 || i == 270)
                    j = yAxisStart + yAxisRange;    // gimbal lock; y-rotation == z-rotation

                for (int k = zAxisStart; k < zAxisStart + zAxisRange; k += zAxisSkip)
                {
                    // set object's orientation
                    objectForCapture.eulerAngles = new Vector3(i, j, k);
                    // calculate bounding box min and max values
                    objectMinMaxCalculator.CalculateMinMax();

                    // take screenshot
                    yield return new WaitForEndOfFrame();

                    Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                    ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    ss.Apply();

                    string filename = string.Format("{0}_{1}.png", prefixFilename, count);
                    Debug.Log(filename);
                    string filePath = Path.Combine(directory + "/Screenshots/", filename);
                    File.WriteAllBytes(filePath, ss.EncodeToPNG());

                    ++count;

                    // write values to the StringBuilder
                    sb.Append(string.Format("{0},{1},{2},{3},{4},{5}\n",
                                filename,
                                objectMinMaxCalculator.MinX,
                                objectMinMaxCalculator.MinY,
                                objectMinMaxCalculator.MaxX,
                                objectMinMaxCalculator.MaxY,
                                0));

                    yield return null;
                }
            }
        }

        // export the built string to CSV
        string csvFilePath = Path.Combine(directory + string.Format("/data_{0}.csv", prefixFilename));
        File.WriteAllText(csvFilePath, sb.ToString());

        // re-enable UI
        capturesCompleteDisplay.SetActive(true);
        testingUI.SetActive(true);
    }
}
