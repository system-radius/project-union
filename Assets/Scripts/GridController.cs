using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public const int MULTIPLIER = 1;
    
    public const int SIZE_X = 24 * MULTIPLIER;

    public const int SIZE_Y = 48 * MULTIPLIER;

    private int[,] field = new int[SIZE_X + 1, SIZE_Y + 1];

    private const int A = 0;

    private const int B = 1;

    private Transform lineContainer;

    private Transform maskContainer;

    // The Divider prefab to be created.
    public GameObject divider;

    public GameObject maskPrefab;

    private GameObject[,] masks = new GameObject[SIZE_X + 1, SIZE_Y + 1];

    public static float ComputeAngle(Vector3 startPos, Vector3 endPos)
    {
        Debug.Log(startPos + ", " + endPos);

        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }

        return Mathf.Rad2Deg * Mathf.Atan(angle);
    }

    // Called when the scene starts.
    void Awake()
    {
        lineContainer = GameObject.FindGameObjectWithTag("LineContainer").transform;
        maskContainer = GameObject.FindGameObjectWithTag("MaskContainer").transform;

        SpawnDivider();
        InitializeField();
    }

    public void SpawnDivider()
    {
        Instantiate(divider);
    }

    void InitializeField()
    {
        for (int x = 0; x <= SIZE_X; x++)
        {
            for (int y = 0; y <= SIZE_Y; y++)
            {
                field[x, y] = 0;
                Vector2 coord = GridToUnitPoint(x, y);

                GameObject mask = Instantiate(maskPrefab, maskContainer);
                mask.transform.position = new Vector3(coord.x, coord.y);
                masks[x, y] = mask;
            }
        }
    }

    int[,] GetField()
    {
        return field;
    }

    public Vector2 UnitToExactNormalized(float x, float y)
    {
        Vector2 exactCoords = UnitToExact(x, y);

        return new Vector2(exactCoords.x / MULTIPLIER, exactCoords.y / MULTIPLIER);
    }

    public Vector2 UnitToExact(float x, float y)
    {
        float xAdder = x < 0 ? -0.5f : 0.5f;
        float yAdder = y < 0 ? -0.5f : 0.5f;

        // Convert x and y to their nearest whole number.
        int cX = (int)((x * MULTIPLIER) + xAdder);
        int cY = (int)((y * MULTIPLIER) + yAdder);

        return new Vector2(cX, cY);
    }

    public Vector2 UnitToGridPoint(float x, float y)
    {
        // Convert x and y to their nearest whole number.
        Vector2 exactCoords = UnitToExact(x, y);

        // Adjust the coordinates to fit the field array.
        int cX = (int)(exactCoords.x + (SIZE_X / 2));
        int cY = (int)(exactCoords.y + (SIZE_Y / 2));
        
        return new Vector2(cX, cY);
    }

    public Vector2 GridToUnitPoint(int x, int y)
    {

        float cX = x - (SIZE_X / 2);
        float cY = y - (SIZE_Y / 2);

        return UnitToExactNormalized(cX, cY);
    }

    public int GetGridValue(float x, float y)
    {
        Vector2 coords = UnitToGridPoint(x, y);

        if (coords.x <= 0 || coords.x >= SIZE_X || coords.y <= 0 || coords.y >= SIZE_Y)
        {
            return -1;
        }

        return field[(int)coords.x, (int)coords.y];
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
        
        col.transform.Rotate(0, 0, ComputeAngle(startPos, endPos));

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
            gridCoords[i] = UnitToGridPoint(pos.x, pos.y);
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

        int down = CountSpaces(pointA, new Vector2(SIZE_X, 0), 1, -1);
        int up = CountSpaces(pointA, new Vector2(SIZE_X, SIZE_Y), 1, 1);

        Debug.Log(down + " > " + up);

        if (down > up)
        {
            FillSpaces(pointA, new Vector2(SIZE_X, SIZE_Y), 1, 1);
            Debug.Log("Up!");
        }
        else
        {
            FillSpaces(pointA, new Vector2(SIZE_X, 0), 1, -1);
            Debug.Log("Down!");
        }

        PrintField();
    }

    private void FillVertical(Vector2 pointA, Vector2 pointB)
    {
        for (int y = (int)pointB.y; y <= (int)pointA.y; y++)
        {
            field[(int)pointA.x, y] = 1;
        }

        int left = CountSpaces(pointB, new Vector2(0, SIZE_Y), -1, 1);
        int right = CountSpaces(pointB, new Vector2(SIZE_X, SIZE_Y), 1, 1);

        Debug.Log(left + " > " + right);

        if (left > right)
        {
            FillSpaces(pointB, new Vector2(SIZE_X, SIZE_Y), 1, 1);
            Debug.Log("Right!");
        }
        else
        {
            FillSpaces(pointB, new Vector2(0, SIZE_Y), -1, 1);
            Debug.Log("Left!");
        }

        PrintField();
    }

    private void PrintField()
    {
        string s = "\n";
        for (int y = SIZE_Y; y >= 0; y--)
        {
            for (int x = 0; x <= SIZE_X; x++)
            {
                s += field[x, y] + " ";
            }
            s += "\n";
        }

        Debug.Log(s);
    }

    private int CountSpaces(Vector2 pointA, Vector2 pointB, int xUpdate, int yUpdate)
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

    private void FillSpaces(Vector2 pointA, Vector2 pointB, int xUpdate, int yUpdate)
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
    }
}