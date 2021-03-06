﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyControls : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10f;
    [SerializeField]
    float rotateSpeed = 50f;

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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += transform.forward * -moveSpeed * Time.deltaTime;
        }
    }
}
