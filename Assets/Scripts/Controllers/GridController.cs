using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class responsible for maintaining handle over grid data.
 * This clas is meant as a storage, may be updated at any time.
 * But, this class will not be responsible for creating lines.
 */
public class GridController
{
    private static GridController instance;

    /**
     * Create an instance.
     */
    public static void CreateInstance()
    {
        if (instance != null)
        {
            return;
        }

        new GridController();
    }

    /**
     * Reset or clear the contents of the things that need to be reset.
     * But do not reinitialize.
     */
    public static void StaticReset()
    {
        instance.ResetField();
    }

    /**
     * Change the values for the list of grid points provided.
     */
    public static void UpdateFieldValues(List<Vector2> gridPoints, GridValue value, bool forced = false)
    {
        foreach (Vector2 gridPoint in gridPoints)
        {
            UpdateFieldValue(gridPoint, value, forced);
        }
    }

    /**
     * Change the value for the current grid point provided.
     */
    public static void UpdateFieldValue(Vector2 gridPoint, GridValue value, bool forced = false)
    {
        int x = (int)gridPoint.x;
        int y = (int)gridPoint.y;

        // If the value change is enforeced, change the value right away.
        // Otherwise, only change if the value is a space.
        instance.field[x, y] = forced || instance.field[x, y] == GridValue.SPACE ? value : instance.field[x, y];
    }

    /**
     * Get the field array. Typically used to read the current values, and not change them.
     */
    public static GridValue[,] GetField()
    {
        return instance.field;
    }

    // The main focus in this class. The storage for all values in the field.
    private GridValue[,] field;

    /**
     * On create, initialize everything.
     */
    private GridController()
    {
        instance = this;
        
        // Initialize the field.
        field = new GridValue[DividerUtils.SIZE_X + 1, DividerUtils.SIZE_Y + 1];
    }

    /**
     * Reset the field table to contain spaces.
     */
    private void ResetField()
    {
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                field[x, y] = GridValue.SPACE;
            }
        }
    }
}
