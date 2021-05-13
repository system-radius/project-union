using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{

    private int[,] field = new int[DividerUtils.SIZE_X + 1, DividerUtils.SIZE_Y + 1];

    private const int A = 0;

    private const int B = 1;

    private Transform lineContainer;

    private Transform maskContainer;

    public GameObject maskPrefab;

    private GameObject[,] masks = new GameObject[DividerUtils.SIZE_X + 1, DividerUtils.SIZE_Y + 1];

    private float fillPercent = 0;

    private int fillableSpaces = 0;

    // Called when the scene starts.
    void Awake()
    {
        // Retrieve the container for the generated lines and bullets.
        lineContainer = GameObject.FindGameObjectWithTag("LineContainer").transform;

        // Retrieve the container for the masks.
        maskContainer = GameObject.FindGameObjectWithTag("MaskContainer").transform;

        Reset();
    }

    public float GetFillPercent()
    {
        return fillPercent;
    }

    public void Reset()
    {

        if (lineContainer == null || maskContainer == null)
        {
            return;
        }

        InitializeField();
        fillableSpaces = CountSpaces(new Vector2(0, 0), new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);
        ComputeFillPercent();

        ClearLines();
    }

    public void ClearLines()
    {
        if (lineContainer == null)
        {
            return;
        }

        foreach (Transform child in lineContainer.transform)
        {
            // Remove all of the children in the container.
            Destroy(child.gameObject);
        }
    }

    void InitializeField()
    {
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                field[x, y] = 0;
                Vector2 coord = DividerUtils.GridToUnitPoint(x, y);

                if (masks[x, y] == null)
                {
                    GameObject mask = Instantiate(maskPrefab, maskContainer);
                    mask.transform.position = new Vector3(coord.x, coord.y);
                    masks[x, y] = mask;
                }

                masks[x, y].GetComponent<SpriteMask>().enabled = false;
            }
        }
    }

    public int[,] GetField()
    {
        return field;
    }

    public void CreateLine(GameObject[] contactPoints)
    {
        Fill(contactPoints);

        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Diffuse"));

        // The number of vertices.
        line.positionCount = 2;

        // Starting and ending widths.
        line.startWidth = 1f;
        line.endWidth = 1f;

        // Starting and ending colors.
        line.startColor = Color.black;
        line.endColor = Color.black;

        // Use the world space.
        line.useWorldSpace = true;

        line.gameObject.transform.SetParent(lineContainer.transform);
        for (int i = 0; i < contactPoints.Length; i++)
        {
            contactPoints[i].transform.SetParent(lineContainer.transform);
        }

        CreateCollision(contactPoints[A], contactPoints[B], line);
    }

    private void CreateCollision(GameObject a, GameObject b, LineRenderer line)
    {

        Vector3 startPos = a.transform.position;
        Vector3 endPos = b.transform.position;

        BoxCollider2D col = new GameObject("Collider").AddComponent<BoxCollider2D>();
        col.transform.parent = line.transform;

        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        
        col.transform.Rotate(0, 0, DividerUtils.ComputeAngle(startPos, endPos));

        col.gameObject.layer = 30;
        col.gameObject.tag = "ColliderLimit";
    }

    private void Fill(GameObject[] contactPoints)
    {
        int length = contactPoints.Length;

        Vector2[] gridCoords = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            // Retrieve the grid version of the contact point values.
            Vector3 pos = contactPoints[i].transform.position;
            gridCoords[i] = DividerUtils.UnitToGridPoint(pos.x, pos.y);
        }

        Vector2 pointA = gridCoords[A];
        Vector2 pointB = gridCoords[B];

        // Check which axis did not change to determine how to fill (vertically or horizontally).

        // For now, there can only be two points of contact.
        bool horizontal = pointA.y - pointB.y == 0;

        if (horizontal)
        {
            FillHorizontal(pointA, pointB);
        }
        else
        {
            FillVertical(pointA, pointB);
        }
    }

    private void FillHorizontal(Vector2 pointA, Vector2 pointB)
    {
        // Fill the line along the y-axis.
        for (int x = (int)pointA.x; x <= (int)pointB.x; x++)
        {
            field[x, (int)pointA.y] = 1;
        }

        int down = CountSpaces(pointA, new Vector2(DividerUtils.SIZE_X, 0), 1, -1);
        int up = CountSpaces(pointA, new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);

        //Debug.Log(down + " > " + up);

        if (down > up)
        {
            FillSpaces(pointA, new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);
            //Debug.Log("Up!");
        }
        else
        {
            FillSpaces(pointA, new Vector2(DividerUtils.SIZE_X, 0), 1, -1);
            //Debug.Log("Down!");
        }

        //PrintField();
    }

    private void FillVertical(Vector2 pointA, Vector2 pointB)
    {
        for (int y = (int)pointB.y; y <= (int)pointA.y; y++)
        {
            field[(int)pointA.x, y] = 1;
        }

        int left = CountSpaces(pointB, new Vector2(0, DividerUtils.SIZE_Y), -1, 1);
        int right = CountSpaces(pointB, new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);

        //Debug.Log(left + " > " + right);

        if (left > right)
        {
            FillSpaces(pointB, new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);
            //Debug.Log("Right!");
        }
        else
        {
            FillSpaces(pointB, new Vector2(0, DividerUtils.SIZE_Y), -1, 1);
            //Debug.Log("Left!");
        }

        //PrintField();
    }

    public void ComputeFillPercent()
    {
        int currentSpaces = CountSpaces(new Vector2(0, 0), new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);
        fillPercent = (float)(fillableSpaces - currentSpaces) / (float)fillableSpaces;

        Debug.Log("fill: " + fillPercent * 100f);
    }

    public int CountSpaces(Vector2 pointA, Vector2 pointB, int xUpdate, int yUpdate)
    {
        int total = 0;

        int xA = (int)pointA.x;
        int yA = (int)pointA.y;

        int xB = (int)pointB.x;
        int yB = (int)pointB.y;

        for (int x = xA; x != xB + xUpdate; x += xUpdate)
        {
            for (int y = yA; y != yB + yUpdate; y += yUpdate)
            {
                // Add to the total if the field value is 0.
                total += field[x, y] == 0 ? 1 : 0;
            }
        }

        return total;
    }

    public void FillSpaces(Vector2 pointA, Vector2 pointB, int xUpdate, int yUpdate)
    {
        int xA = (int)pointA.x;
        int yA = (int)pointA.y;

        int xB = (int)pointB.x;
        int yB = (int)pointB.y;

        for (int x = xA; x != xB + xUpdate; x += xUpdate)
        {
            for (int y = yA; y != yB + yUpdate; y += yUpdate)
            {
                // Fill the coordinate with 1 if it is 0.
                field[x, y] = field[x, y] == 0 ? 1 : field[x, y];

                masks[x, y].GetComponent<SpriteMask>().enabled = field[x, y] == 1;
            }
        }

        ComputeFillPercent();
    }
}