using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    // a series of points to follow

    Vector3[] points;
    public Ellipse ellipse;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("PopulatePoints", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPoint(int index)
    {
        index %= points.Length;
        if (index >= 0 && index < points.Length)
        {
            return points[index];
        }
        // shouldn't get here. I guess if it's negative, it will
        Debug.Log($"index out of range: {index} / {points.Length}");
        return Vector3.zero;
    }

    void PopulatePoints()
    {
        points = ellipse.GetPoints();
        System.Array.Reverse(points);
    }

    public bool Ready()
    {
        bool ready = false;
        if (points != null)
        {
            if (points.Length > 0)
            {
                ready = true;
            }
        }
        return ready;
    }
}
