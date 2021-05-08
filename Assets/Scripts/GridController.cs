using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public const int MULTIPLIER = 2;
    
    public const int SIZE_X = 24 * MULTIPLIER;

    public const int SIZE_Y = 48 * MULTIPLIER;

    private int[,] field = new int[SIZE_X + 1, SIZE_Y + 1];


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
        // Convert x and y to their nearest whole number.
        int cX = (int)((x * MULTIPLIER) + 0.5f);
        int cY = (int)((y * MULTIPLIER) + 0.5f);

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
}