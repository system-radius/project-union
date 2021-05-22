using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{

    public static GameObject GenerateLine(Vector3 pointA, Vector3 pointB)
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Diffuse"));
        CreatePoint(pointA).transform.parent = line.transform;
        CreatePoint(pointB).transform.parent = line.transform;

        GenerateLineCollision(pointA, pointB, line);

        // Set the position of the line.
        line.positionCount = 2;

        line.SetPosition(0, pointA);
        line.SetPosition(1, pointB);

        // Set the starting properties.
        line.startWidth = 1f;
        line.startColor = Color.black;

        // Set the ending properties.
        line.endWidth = 1f;
        line.endColor = Color.black;

        // Use the world space.
        line.useWorldSpace = true;

        return line.gameObject;
    }

    public static GameObject CreatePoint(Vector3 vector)
    {
        GameObject point = new GameObject("Point");
        point.transform.position = vector;

        return point;
    }

    public static void GenerateLineCollision(Vector3 pointA, Vector3 pointB, LineRenderer line)
    {
        BoxCollider2D col = new GameObject("Collider").AddComponent<BoxCollider2D>();
        col.transform.parent = line.transform;

        float lineLength = Vector3.Distance(pointA, pointB);
        col.size = new Vector3(lineLength, 1f, 0.1f);

        // Set the position of the line as the middle coordinate between the two points.
        col.transform.position = (pointA + pointB) / 2;

        // Set the rotation of the collision as the angle between the two points.
        col.transform.Rotate(0, 0, DividerUtils.ComputeAngle(pointA, pointB));
        col.gameObject.layer = 30;
        col.gameObject.tag = "ColliderLimit";
    }
}
