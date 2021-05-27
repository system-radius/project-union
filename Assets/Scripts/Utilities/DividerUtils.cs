using System;
using System.Linq;
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

    public const string TAG_ENEMY = "Enemy";

    public const string TAG_LIMIT = "ColliderLimit";

    public const string TAG_PLAYER = "Player";

    public const string TAG_SHOCK = "ShockWave";

    public const string TAG_ENEMY_KILLER = "EnemyKiller";

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

        if (float.IsNaN(angle))
        {
            // Automatically return 0 if the angle is not a number.
            return 0;
        }

        return Mathf.Rad2Deg * Mathf.Atan(angle);
    }

    /**
     * A utility method for printing the integer grid.
     */
    public static void PrintField(GridValue[,] field, int maxX = SIZE_X, int maxY = SIZE_Y)
    {
        string s = "\n";
        for (int y = maxY; y >= 0; y--)
        {
            for (int x = 0; x <= maxX; x++)
            {
                s += ((int)field[x, y]) + " ";
            }
            s += "\n";
        }

        Debug.Log(s);
    }

    public static void PrintContactPoints(List<Vector3> contactPoints)
    {
        string text = "";

        foreach (Vector2 contact in contactPoints)
        {
            text += " " + contact;
        }

        Debug.Log(text);
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

    public static Vector2 GridToUnitPoint(Vector2 vector)
    {
        return GridToUnitPoint(vector.x, vector.y);
    }

    public static Vector2 GridToUnitPoint(float x, float y)
    {
        return GridToUnitPoint((int)x, (int)y);
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

    public static List<Vector2> ProcessField(
        GridValue[,] field,     // The field to be processed.
        Vector2 startPos,       // The starting coordinate in the field.
        Vector2 endPos,         // The ending coordinate in the field.
        Vector2 update          // The x and y update for the crawler.
    )
    {

        // The list of visited coordinates.
        List<Vector2> visited = new List<Vector2>();

        // The list of the coordinates to be visited.
        List<Vector2> frontier = new List<Vector2>();
        frontier.Add(FindStartPos(field, startPos, endPos, ComputeUpdateVector(startPos, endPos, update)));
        frontier.Add(FindStartPos(field, endPos, startPos, ComputeUpdateVector(endPos, startPos, update)));

        while (frontier.Count > 0)
        {
            // Always get the first element.
            Vector2 coord = frontier[0];

            // Also, remove the first element.
            frontier.RemoveAt(0);

            if (visited.Contains(coord))
            {
                // If the coord is already visited, continue.
                continue;
            }

            visited.Add(coord);

            List<Vector2> newFrontier = FindNextFrontier(field, coord);
            frontier.AddRange(newFrontier);
        }

        return visited;
    }

    public static Vector2 ComputeUpdateVector(Vector2 pointA, Vector2 pointB, Vector2 update)
    {
        Vector2 result = new Vector2();

        Vector2 diff = pointB - pointA ;

        result.x = diff.x == 0 ? update.x : diff.x < 0 ? -1 : 1;
        result.y = diff.y == 0 ? update.y : diff.y < 0 ? -1 : 1;

        return result;
    }

    /**
     * Find the starting position along the line.
     */
    public static Vector2 FindStartPos(GridValue[,] field, Vector2 pointA, Vector2 pointB, Vector2 update)
    {
        // Set the current point as the starting point.
        Vector2 result = new Vector2(pointA.x, pointA.y);

        // Update on the x-axis only.
        Vector2 updateX = new Vector2(update.x, 0);

        // Update on the y-axis only.
        Vector2 updateY = new Vector2(0, update.y);

        float currentMagnitude = (pointA - pointB).magnitude;

        // Move from point A to point B, detecting the possible start point on the respective side.
        do
        {
            // From the current reference point, select the possible starting point.
            // The possible movements would be as specified by the update vector.

            // Attempt the update on the x-axis.
            Vector2 tempX = pointA + updateX;

            // If the grid value is space, return that.
            if ((tempX.x >= 0 && tempX.x <= SIZE_X) && field[(int)tempX.x, (int)tempX.y] == GridValue.SPACE)
            {
                return tempX;
            }

            // Attempt the update on the y-axis.
            Vector2 tempY = pointA + updateY;

            if ((tempY.y >= 0 && tempY.y <= SIZE_Y) && field[(int)tempY.x, (int)tempY.y] == GridValue.SPACE)
            {
                return tempY;
            }

            // If none of the points contain the space value, update the reference point.
            // If the current magnitude is greater then the new magnitude on x-update...
            if (currentMagnitude > (tempX - pointB).magnitude)
            {
                pointA += updateX;

                // Reset the current magnitude.
                currentMagnitude = (tempX - pointB).magnitude;
            }
            else
            {
                // Otherwise, update along the y-axis.
                pointA += updateY;

                currentMagnitude = (tempY - pointB).magnitude;
            }

        } while (currentMagnitude != 0);

        return result;
    }

    /**
     * Retrieve the next set of coordinates neighboring the current.
     */
    public static List<Vector2> FindNextFrontier(GridValue[,] field, Vector2 coord)
    {
        List<Vector2> frontier = new List<Vector2>();

        int coordX = (int)coord.x;
        int coordY = (int)coord.y;

        // Get all the coordinates around the current coordinate.
        for (int x = - 1; x <= 1; x++)
        {
            int gridX = coordX + x;

            if (gridX < 0 || gridX > SIZE_X)
            {
                // Skip if the X is an invalid value.
                continue;
            }

            for (int y = -1; y <= 1; y++)
            {
                int gridY = coordY + y;
                if (gridY < 0 || gridY > SIZE_Y)
                {
                    // Skip if the Y is an invalid value.
                    continue;
                }

                //if ((x == y) || (x + y == 0))
                /*
                if (x == 0 && y == 0)
                {
                    // Also continue if the x and y coords are on the diagonal.
                    continue;
                }/**/

                if (field[gridX, gridY] == GridValue.SPACE)
                {
                    // Take the coordinate if the grid value is a space.
                    frontier.Add(new Vector2(gridX, gridY));
                }
            }
        }

        return frontier;
    }

    /**
     * Compiles two lists, so that an item from List A is followed by an item in List B.
     */
    public static List<Vector2> InterweaveLists(List<Vector2> listA, List<Vector2> listB, int weave = 2)
    {
        List<Vector2> weavedList = new List<Vector2>();

        bool other = false;
        int weaveCounter = 0;
        while (listA.Count > 0 || listB.Count > 0)
        {
            if (other)
            {
                if (listB.Count == 0)
                {
                    other = !other;
                    continue;
                }
                weavedList.Add(listB[0]);
                listB.RemoveAt(0);
            }
            else
            {
                if (listA.Count == 0)
                {
                    other = !other;
                    continue;
                }
                weavedList.Add(listA[0]);
                listA.RemoveAt(0);
            }

            if (weaveCounter + 1 == weave)
            {
                weaveCounter = 0;
                other = !other;
            }

            weaveCounter++;
        }

        return weavedList;
    }

    /**
     * Gets the bounding coordinates from the provided field.
     */
    public static List<LinePoints> FindBoundingCoords(GridValue[,] field, Vector2 startCoord)
    {
        List<LinePoints> result = new List<LinePoints>();

        // Starting from the provided point, move towards the four cardinal directions.
        result.AddRange(FindDirectionalBoundingCoords(field, startCoord, new Vector2(1, 0)));
        result.AddRange(FindDirectionalBoundingCoords(field, startCoord, new Vector2(0, 1)));
        result.AddRange(FindDirectionalBoundingCoords(field, startCoord, new Vector2(-1, 0)));
        result.AddRange(FindDirectionalBoundingCoords(field, startCoord, new Vector2(0, -1)));

        return result;
    }

    public static List<LinePoints> FindDirectionalBoundingCoords(GridValue[,] field, Vector2 startCoord, Vector2 update)
    {
        List<LinePoints> result = new List<LinePoints>();

        Vector2 coord = startCoord;
        Vector2 stepCoord = startCoord + update;

        // Repeat the updates to the coord while the field value is equal to the bound value.
        while (IsValidCoord(stepCoord) && field[(int)stepCoord.x, (int)stepCoord.y] == GridValue.BOUNDS)
        {
            coord = stepCoord;
            stepCoord += update;

            // Change the field value to crawl.
            field[(int)coord.x, (int)coord.y] = GridValue.CRAWL;
        }

        if (coord == startCoord) {
            // This means that the search has not progressed. Return right away.
            return result;
        }

        // If the step coord goes out of bounds, or the value is invalid, then use the last coord.
        result.Add(new LinePoints(GridToUnitPoint(startCoord), GridToUnitPoint(coord)));

        // Find other bounds, based on the new found coord.
        result.AddRange(FindBoundingCoords(field, coord));

        return result;
    }

    private static bool IsValidCoord(Vector2 coord) {
        return !(coord.x < 0 || coord.y < 0 || coord.x > SIZE_X || coord.y > SIZE_Y);
    }
}
