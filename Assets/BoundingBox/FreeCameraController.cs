using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float rotateSpeed = 25f;

    // Update is called once per frame
    void Update()
    {
        // Control transform
        Rotate();
        Move();
    }

    private void Rotate()
    {
        // yaw
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }

    private void Move()
    {
        // Move forward
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        // Move backward
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.forward * -moveSpeed * Time.deltaTime;
        }

        // Strafe Left
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.right * -moveSpeed * Time.deltaTime;
        }
        // Strafe Right
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
    }
}
