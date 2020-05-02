using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxRatio : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private SpriteRenderer boundingBox;

    [Header("Bounding Box References")]
    [SerializeField]
    private SpriteRenderer bboxFront;
    [SerializeField]
    private SpriteRenderer bboxBack;
    [SerializeField]
    private SpriteRenderer bboxLeft;
    [SerializeField]
    private SpriteRenderer bboxRight;
    [SerializeField]
    private SpriteRenderer bboxTop;
    [SerializeField]
    private SpriteRenderer bboxBottom;

    void Start()
    {
        float width = Screen.width;
        float height = Screen.height;

        Debug.Log("Width: " + width + ", Height: " + height);
    }

    void Update()
    {
        UpdateBoundingBoxSize();
        Billboard();
    }

    private void UpdateBoundingBoxSize()
    {
        // get vector from camera to bbox
        Vector3 camPos = camera.transform.position;
        Vector3 bboxCenterPos = transform.position;
        Vector3 bboxToCamera = bboxCenterPos - camPos;

        // compare position of camera and bbox
        SpriteRenderer bboxFrontOrBack, bboxLeftOrRight;
        Vector3 vectorAgainst;
        if (bboxToCamera.z > 0f)    // camera is looking at the front of the shuttlecock
        {
            bboxFrontOrBack = bboxFront;
            //vectorAgainst = -transform.forward;
            //bboxFrontOrBack.gameObject.SetActive(true);

            Debug.Log("Front");
        }
        else
        {
            bboxFrontOrBack = bboxBack;
            //vectorAgainst = transform.forward;

            Debug.Log("Back");
        }

        if (bboxToCamera.x > 0f)    // camera is looking at the right of the shuttlecock
        {
            bboxLeftOrRight = bboxRight;
            vectorAgainst = transform.right;

            Debug.Log("Right");
        }
        else
        {
            bboxLeftOrRight = bboxLeft;
            vectorAgainst = -transform.right;

            Debug.Log("Left");
        }

        // Interpolate
        float angle = Vector3.Angle(vectorAgainst, bboxToCamera);
        if (angle > 90f)
            angle -= 90f;
        if (bboxFrontOrBack == bboxFront)
        {
            Debug.Log("Angle: " + angle);
            float lerp = angle / 90f;
            // change position
            //boundingBox.transform.position = Vector3.Lerp(bboxLeftOrRight.transform.position, bboxFrontOrBack.transform.position, lerp);
            //boundingBox.eulerAngles = Vector3.Lerp(bboxLeftOrRight.eulerAngles, bboxFrontOrBack.eulerAngles, angle / 90f);

            boundingBox.size = new Vector2(Mathf.Lerp(bboxLeftOrRight.size.x, bboxFrontOrBack.size.x, lerp),
                Mathf.Lerp(bboxLeftOrRight.size.y, bboxFrontOrBack.size.y, lerp));
        }
        else
        {
            //angle = 90f - angle;
            Debug.Log("Angle: " + angle);
            float lerp = angle / 90f;
            // change position
            //boundingBox.transform.position = Vector3.Lerp(bboxLeftOrRight.transform.position, bboxFrontOrBack.transform.position, lerp);
            //boundingBox.eulerAngles = Vector3.Lerp(bboxLeftOrRight.eulerAngles, bboxFrontOrBack.eulerAngles, angle / 90f);

            //boundingBox.position = Vector3.Lerp(bboxFrontOrBack.position, bboxLeftOrRight.position, angle / 90f);
            //boundingBox.eulerAngles = Vector3.Lerp(bboxFrontOrBack.eulerAngles, bboxLeftOrRight.eulerAngles, angle / 90f);

            boundingBox.size = new Vector2(Mathf.Lerp(bboxLeftOrRight.size.x, bboxFrontOrBack.size.x, lerp),
                Mathf.Lerp(bboxLeftOrRight.size.y, bboxFrontOrBack.size.y, lerp));
        }
    }

    private void Billboard()
    {
        // Rotate to face camera
        Vector3 camDir = camera.transform.forward;

        // find yaw angle between camera direction and bounding box
        float angle = Mathf.Rad2Deg * Mathf.Atan2(camDir.x, camDir.z);

        boundingBox.transform.eulerAngles = new Vector3(0f, angle, 0f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Bounds bounds = boundingBox.GetComponent<Renderer>().bounds;

        Vector3 screenCenter = camera.WorldToScreenPoint(bounds.center);
        //Debug.Log("BBox Center (Screen): " + screenCenter);
        Vector3 botLeft = camera.WorldToScreenPoint(bounds.min);
        //Debug.Log("BBox Bottom Left (Screen): " + botLeft);
        Vector3 topRight = camera.WorldToScreenPoint(bounds.max);
        //Debug.Log("BBox Top Right (Screen): " + topRight);
    }

}
