using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMinMax : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private MeshFilter meshFilter;

    [Header("Max Lines")]
    [SerializeField]
    private Transform maxHorizontalLine;
    [SerializeField]
    private Transform maxVerticalLine;

    [Header("Min Lines")]
    [SerializeField]
    private Transform minHorizontalLine;
    [SerializeField]
    private Transform minVerticalLine;

    private const float EPSILON = 0.00001f;

    private float maxX;
    public int MaxX
    {
        get {
            int value = (int)maxX;
            if (maxX - value > EPSILON)
                return value + 1;
            else
                return value;
        }
    }
    private float minX;
    public int MinX
    {
        get {
            return (int)minX;
        }
    }
    private float maxY;
    public int MaxY
    {
        get {
            int value = (int)maxY;
            if (maxY - value > EPSILON)
                return value + 1;
            else
                return value;
        }
    }
    private float minY;
    public int MinY
    {
        get {
            return (int)minY;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (meshFilter == null)
            meshFilter = this.GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CalculateMinMax();
        transform.hasChanged = false;
    }

    public void CalculateMinMax()
    {
        // find min max for x and y
        maxX = float.NegativeInfinity;
        minX = float.PositiveInfinity;

        maxY = float.NegativeInfinity;
        minY = float.PositiveInfinity;
        
        foreach (Vector3 vert in meshFilter.sharedMesh.vertices)
        {
            Vector3 transformVert = transform.TransformPoint(vert);
            Vector3 projected = camera.WorldToScreenPoint(transformVert);
            if (projected.y > maxY)
                maxY = projected.y;
            if (projected.y < minY)
                minY = projected.y;

            if (projected.x > MaxX)
                maxX = projected.x;
            if (projected.x < minX)
                minX = projected.x;
        }

        // set min and max visualisation
        if (maxHorizontalLine != null)
            maxHorizontalLine.position = new Vector3(maxHorizontalLine.position.x, MaxY);
        if (minHorizontalLine != null)
            minHorizontalLine.position = new Vector3(minHorizontalLine.position.x, MinY);

        if (maxVerticalLine != null)
            maxVerticalLine.position = new Vector3(MaxX, maxVerticalLine.position.y);
        if (minVerticalLine != null)
            minVerticalLine.position = new Vector3(MinX, minVerticalLine.position.y);
    }
}
