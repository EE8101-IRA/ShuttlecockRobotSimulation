using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxLabeller : MonoBehaviour
{
    [Header("Cursor Lines")]
    [SerializeField]
    private Transform horizontalLine;
    [SerializeField]
    private Transform verticalLine;

    [Header("Others")]
    [SerializeField]
    private GameObject captureButton;

    private bool capture = false;

    // Start is called before the first frame update
    void Start()
    {
        horizontalLine.gameObject.SetActive(false);
        verticalLine.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (capture)
        {
            Vector3 mousePos = Input.mousePosition;
            verticalLine.position = new Vector3(mousePos.x, verticalLine.position.y);
            horizontalLine.position = new Vector3(horizontalLine.position.x, mousePos.y);
        }
    }

    public void StartCapture()
    {
        captureButton.SetActive(false);

        horizontalLine.gameObject.SetActive(true);
        verticalLine.gameObject.SetActive(true);
        capture = true;
    }
}
