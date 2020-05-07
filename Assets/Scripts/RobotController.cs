﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The robot's logic for how to search and move towards a shuttlecock
/// </summary>
public class RobotController : MonoBehaviour
{
    /// <summary>
    /// Movement speed of the robot
    /// </summary>
    [SerializeField]
    private float moveSpeed = 10f;
    /// <summary>
    /// Rotation speed of the robot
    /// </summary>
    [SerializeField]
    private float rotateSpeed = 50f;

    [SerializeField]
    [Tooltip("Length of one 'step' (in metres) for the robot")]
    private float oneStep = 1f;

    [SerializeField]
    private ImageStreamManager imageStreamManager;

    /// <summary>
    /// Reference to the search coroutine.
    /// </summary>
    private IEnumerator searchCoroutine = null;

    /// <summary>
    /// Call this function to start the robot search algorithm.
    /// </summary>
    public void BeginSearch()
    {
        searchCoroutine = ShuttlecockSearch();
        StartCoroutine(searchCoroutine);
    }

    private IEnumerator ShuttlecockSearch()
    {
        while (true)    // will keep searching until a shuttlecock is found
        {
            // F2 * 2 -- Scan the perimeter of the field
            for (int i = 0; i < 2; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            // rotate 53 degrees anti-clockwise (rotate left)
            yield return rotate(-53f);

            // F1 * 5 -- Scan inside 1/2 field
            for (int i = 0; i < 5; i++)
            {
                // F1: move 1 step and scan
                yield return Move1StepAndScan();
            }

            // rotate 143 degrees clockwise (rotate right)
            yield return rotate(143f);

            // F1 * 4-- Scan the middle line
            for (int i = 0; i < 4; i++)
            {
                // F1: move 1 step and scan
                yield return Move1StepAndScan();
            }

            // rotate 143 degrees anti-clockwise (rotate left)
            yield return rotate(-143f);

            // F1 * 5 -- Scan inside 1/2 field
            for (int i = 0; i < 5; i++)
            {
                // F1: move 1 step and scan
                yield return Move1StepAndScan();
            }

            // rotate 143+90 degrees clockwise (rotate right)
            yield return rotate(233f);

            Debug.Log("ONE LOOP COMPLETED");
        }
    }

    /// <summary>
    /// Function 1: Move 1 step and scan
    /// </summary>
    private IEnumerator Move1StepAndScan()
    {
        Debug.Log("Now in: F1");

        // move 1 step (4.5m) to the front
        Vector3 targetPos = transform.position + oneStep * transform.forward;
        yield return move(targetPos);

        // scan the area
        yield return ScanTheArea();
    }

    /// <summary>
    /// Function 2: Moves 2 steps, each time scanning, then rotates and scans
    /// </summary>
    private IEnumerator MoveAndScan1Side()
    {
        Debug.Log("Now in: F2");

        // scan and find length of court
        for (int i = 0; i < 6; i++) {
            Debug.Log("F1: " + i);
            yield return Move1StepAndScan();    // F1: Move 1 step and scan
        }

        // rotate left by 90 degrees
        yield return rotate(-90f);

        // scan and find width of court
        for (int i = 0; i < 4; i++)
        {
            yield return Move1StepAndScan();    // F1: Move 1 step and scan
        }

        // rotate left by 90 degrees
        yield return rotate(-90f);
    }

    private IEnumerator move(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;

        while (true)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            Vector3 currToTarget = targetPos - transform.position;
            float angle = Vector3.Angle(currToTarget, toTarget);
            if (angle > 175f && angle < 185f)   // overshot
            {
                transform.position = targetPos;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator rotate(float angle)
    {
        Vector3 initialEulerAngles = transform.eulerAngles;
        float rotated = 0f;

        if (angle > 0f)
        {
            while (rotated < angle)
            {
                rotated += rotateSpeed * Time.deltaTime;
                transform.eulerAngles = initialEulerAngles + new Vector3(0f, rotated, 0f);

                yield return null;
            }
        }
        else
        {
            while (rotated > angle)
            {
                rotated -= rotateSpeed * Time.deltaTime;
                transform.eulerAngles = initialEulerAngles + new Vector3(0f, rotated, 0f);

                yield return null;
            }
        }

        transform.eulerAngles = initialEulerAngles + new Vector3(0f, angle, 0f);
    }

    /// <summary>
    /// Function 3: Process the robot's current view
    /// </summary>
    private IEnumerator ScanTheArea()
    {
        // send image to server
        imageStreamManager.SendImage();

        // wait for response
        yield return new WaitForSeconds(0.2f);  // temporary wait

        // react to response
        ///if shuttlecock found
            ///stop the search coroutine
            ///move to shuttlecock
    }
}
