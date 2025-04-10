using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldBoxOutline : MonoBehaviour
{
    public float width;
    public float height;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 5;
        line.loop = true;
        line.useWorldSpace = false;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        Vector3[] points = new Vector3[5];
        points[0] = new Vector3(-width / 2, -height / 2, 0);
        points[1] = new Vector3(-width / 2, height / 2, 0);
        points[2] = new Vector3(width / 2, height / 2, 0);
        points[3] = new Vector3(width / 2, -height / 2, 0);
        points[4] = points[0];

        line.SetPositions(points);
    }
}
