using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{

    private static LineGenerator instance;

    public GameObject linePrefab;

    public GameObject pointPrefab;

    void Start()
    {
        instance = this;
    }

    public static LineGenerator GetInstance()
    {
        return instance;
    }

    public static void ClearLines()
    {
        // Clear all the children for the instance.
        foreach (Transform child in instance.transform)
        {
            // Destroy the children game objects.
            ClearRecursively(child);
        }
    }

    private static void ClearRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ClearRecursively(child);
        }

        Destroy(parent.gameObject);
    }

    public static GameObject GenerateLine(Vector3 pointA, Vector3 pointB)
    {
        return instance.CreateLineInstance(pointA, pointB);
    }

    public static GameObject CreatePoint(Vector3 vector)
    {
        // Call for the instance method to spawn the point game object.
        return instance.CreatePointInstance(vector);
    }

    /**
     * Create a collision object for the line renderer.
     */
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

    /**
     * Create a line connecting the two points provided.
     */
    private GameObject CreateLineInstance(Vector3 pointA, Vector3 pointB)
    {

        LineRenderer line;
        GameObject lineObject;

        if (linePrefab == null)
        {
            lineObject = new GameObject("Line");
            line = lineObject.AddComponent<LineRenderer>();
        }
        else
        {
            lineObject = Instantiate(linePrefab);
            line = lineObject.GetComponent<LineRenderer>();
        }

        // Create points to represent the line endings.
        CreatePoint(pointA).transform.parent = line.transform;
        CreatePoint(pointB).transform.parent = line.transform;

        // Create a collision object for the line renderer.
        GenerateLineCollision(pointA, pointB, line);

        // Set the position of the line.
        line.positionCount = 2;

        // Set the corresponding positions of the line.
        line.SetPosition(0, pointA);
        line.SetPosition(1, pointB);

        // Use the world space.
        line.useWorldSpace = true;

        // Set the parent for this line object.
        lineObject.transform.parent = gameObject.transform;

        return lineObject;
    }

    /**
     * Create a point object to represent the vector.
     */
    private GameObject CreatePointInstance(Vector3 vector)
    {
        GameObject point = pointPrefab == null ? new GameObject("Point") : Instantiate(pointPrefab);
        point.transform.position = vector;

        // Set the parent for this point.
        point.transform.parent = gameObject.transform;

        return point;
    }
}
