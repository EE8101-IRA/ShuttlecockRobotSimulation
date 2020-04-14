using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Cameras for scene view")]
    private GameObject[] sceneCameras;

    [SerializeField]
    private KeyCode toggleKey = KeyCode.Tab;

    private int currSelectedCamera = 0;
    private int numCameras;

    // Start is called before the first frame update
    void Start()
    {
        numCameras = sceneCameras.Length;
        SetSelectedCameraActive();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            currSelectedCamera = (currSelectedCamera + 1) % numCameras;
            SetSelectedCameraActive();
        }
    }

    private void SetSelectedCameraActive()
    {
        for (int i = 0; i < numCameras; i++)
        {
            if (i == currSelectedCamera)
                sceneCameras[i].SetActive(true);
            else
                sceneCameras[i].SetActive(false);
        }
    }
}
