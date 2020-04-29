using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 25f;

    private float xRot = 0f;
    private float yRot = 0f;

    // Update is called once per frame
    void Update()
    {
        // pitch
        if (Input.GetKey(KeyCode.UpArrow))
        {
            xRot -= rotateSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            xRot += rotateSpeed * Time.deltaTime;
        }

        // yaw
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            yRot -= rotateSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            yRot += rotateSpeed * Time.deltaTime;
        }

        transform.eulerAngles = new Vector3(xRot, yRot, 0f);
    }
}
