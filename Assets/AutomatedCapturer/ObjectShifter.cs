using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShifter : MonoBehaviour
{
    [SerializeField]
    private Vector3[] positions;

    private int startIdx = 0;

    void Start()
    {
        // set position
        transform.position = positions[startIdx];
    }

    public void TogglePosition()
    {
        // get next position
        startIdx = (startIdx + 1) % positions.Length;

        // set position
        transform.position = positions[startIdx];
    }
}
