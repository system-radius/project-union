using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividerUtils : MonoBehaviour
{

    // A multiplier variable which can modify the size of the playing field
    // in terms of the grid.
    public const int MULTIPLIER = 1;

    // The size of the playing field along the x-axis.
    public const int SIZE_X = 24 * MULTIPLIER;

    // The size of the playing field along the y-axis.
    public const int SIZE_Y = 48 * MULTIPLIER;

    // The amount of time required to pass before the prompt to continue appears.
    public const float TRANSITION_TIME = 2f;

    // The amount of space needed to be filled.
    public const float FILL_QUOTA = 0.85f;

    /**
     * Compute the angle between two points. The angle returned is useful for rotations.
     */
    public static float ComputeAngle(Vector3 startPos, Vector3 endPos)
    {

        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }

        return Mathf.Rad2Deg * Mathf.Atan(angle);
    }

    /**
     * A utility method for printing the integer grid.
     */
    public static void PrintField(int[,] field)
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

    public static Vector2 UnitToExactNormalized(float x, float y)
    {
        Vector2 exactCoords = UnitToExact(x, y);

        return new Vector2(exactCoords.x / MULTIPLIER, exactCoords.y / MULTIPLIER);
    }

    public static Vector2 UnitToExact(float x, float y)
    {
        float xAdder = x < 0 ? -0.5f : 0.5f;
        float yAdder = y < 0 ? -0.5f : 0.5f;

        // Convert x and y to their nearest whole number.
        int cX = (int)((x * MULTIPLIER) + xAdder);
        int cY = (int)((y * MULTIPLIER) + yAdder);

        return new Vector2(cX, cY);
    }

    public static Vector2 UnitToGridPoint(float x, float y)
    {
        // Convert x and y to their nearest whole number.
        Vector2 exactCoords = UnitToExact(x, y);

        // Adjust the coordinates to fit the field array.
        int cX = (int)(exactCoords.x + (SIZE_X / 2));
        int cY = (int)(exactCoords.y + (SIZE_Y / 2));

        return new Vector2(cX, cY);
    }

    public static Vector2 GridToUnitPoint(int x, int y)
    {

        float cX = x - (SIZE_X / 2);
        float cY = y - (SIZE_Y / 2);

        return UnitToExactNormalized(cX, cY);
    }

    public static GridValue GetGridValue(GridValue[,] field, float x, float y)
    {
        Vector2 coords = UnitToGridPoint(x, y);

        if (coords.x <= 0 || coords.x >= SIZE_X || coords.y <= 0 || coords.y >= SIZE_Y)
        {
            return GridValue.VOID;
        }

        return field[(int)coords.x, (int)coords.y];
    }
}
