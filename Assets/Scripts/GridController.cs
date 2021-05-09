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

    // The Divider prefab to be created.
    public GameObject divider;

    // Called when the scene starts.
    void Awake()
    {
        SpawnDivider();
        InitializeField();
    }

    // Update is called once per frame
    void Update()
    {

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

        int length = contactPoints.Length;

        Vector2[] gridCoords = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            // Retrieve the grid version of the contact point values.
            Vector3 pos = contactPoints[i].transform.position;
            gridCoords[i] = UnitToGridPoint(pos.x, pos.y);
        }

        Fill(gridCoords[A], gridCoords[B]);
    }

    private void Fill(Vector2 pointA, Vector2 pointB)
    {
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

        PrintField();
    }

    private void FillVertical(Vector2 pointA, Vector2 pointB)
    {
        for (int y = (int)pointB.y; y <= (int)pointA.y; y++)
        {
            field[(int)pointA.x, y] = 1;
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
}