using System.Collections;
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

    /// <summary>
    /// Reference to the search coroutine.
    /// </summary>
    private IEnumerator searchCoroutine = null;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        searchCoroutine = ShuttlecockSearch();
        StartCoroutine(searchCoroutine);
    }

    private IEnumerator ShuttlecockSearch()
    {
        while (true)    // will keep searching until a shuttlecock is found
        {
            // F2 * 4 -- Scan & find 1/4 of court
            for (int i = 0; i < 4; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            // F1 * 2 -- Scan & find another 1/4 of court
            for (int i = 0; i < 2; i++)
            {
                // F1: move 1 step and scan
                yield return Move1StepAndScan();
            }

            // F2 * 3 -- Scan & find 1/4 of court
            for (int i = 0; i < 4; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            Debug.Log("ONE LOOP COMPLETED");
        }
    }

    /// <summary>
    /// Coroutine containing logic for searching. This coroutine needs to be stopped manually.
    /// </summary>
    private IEnumerator ShuttlecockSearchOriginal()
    {
        while (true)    // will keep searching until a shuttlecock is found
        {
            // F2 * 4 -- Scan & find 1/4 of court
            for (int i = 0; i < 4; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            // F1 * 2 -- Scan & find another 1/4 of court
            for (int i = 0; i < 2; i++)
            {
                // F1: move 1 step and scan
                yield return Move1StepAndScan();
            }

            // F2 * 3 -- Scan & find another 1/4 of court
            for (int i = 0; i < 3; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            // Rotate right x2
            yield return rotate(180f);

            // F2 * 4 -- Scan & find another 1/4 of court
            for (int i = 0; i < 4; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }

            // Rotate right
            yield return rotate(90f);

            // F2 * 3 -- Scan & find another 1/4 of court
            for (int i = 0; i < 3; i++)
            {
                // F2: move and scan 1 side at a time
                yield return MoveAndScan1Side();
            }
        }
    }

    /// <summary>
    /// Function 1: Move 1 step and scan
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private IEnumerator Move1StepAndScan()
    {
        Debug.Log("Now in: F1");
        Vector3 targetPos = transform.position + oneStep * transform.forward;
        yield return move(targetPos);
        yield return ScanTheArea();
    }

    /// <summary>
    /// Function 2: Moves 2 steps, each time scanning, then rotates and scans
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveAndScan1Side()
    {
        Debug.Log("Now in: F2");
        // F1: Move 1 step and scan
        yield return Move1StepAndScan();
        // F1: Move 1 step and scan
        yield return Move1StepAndScan();
        // rotate left by 90 degrees or any other number
        yield return rotate(-90f);
        // F3: Scan the area
        yield return ScanTheArea();
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
    /// <returns></returns>
    private IEnumerator ScanTheArea()
    {
        // send image to server

        // wait for response
        yield return new WaitForSeconds(2f);  // temporary wait

        // react to response
        ///if shuttlecock found
            ///stop the search coroutine
            ///move to shuttlecock
    }
}
