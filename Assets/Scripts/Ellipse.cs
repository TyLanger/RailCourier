using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Ellipse : MonoBehaviour
{
    // stolen from stack overflow
    // made it work in xz instead of xy

    public float a = 5;
    public float b = 3;
    public float h = 1;
    public float k = 1;
    public float theta = 45;
    public int resolution = 1000;

    private Vector3[] positions;

    void Start()
    {
        positions = CreateEllipse(a, b, h, k, theta, resolution);
        LineRenderer lr = GetComponent<LineRenderer>();
        //lr.SetVertexCount(resolution + 1);
        lr.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++)
        {
            lr.SetPosition(i, positions[i]);
        }
    }

    Vector3[] CreateEllipse(float a, float b, float h, float k, float theta, int resolution)
    {

        positions = new Vector3[resolution + 1];
        Quaternion q = Quaternion.AngleAxis(theta, Vector3.up);
        Vector3 center = new Vector3(h, 0.0f, k);

        for (int i = 0; i <= resolution; i++)
        {
            float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
            positions[i] = new Vector3(a * Mathf.Cos(angle), 0.0f, b * Mathf.Sin(angle));
            positions[i] = q * positions[i] + center;
        }

        return positions;
    }

    public Vector3[] GetPoints()
    {
        return positions;
    }
}