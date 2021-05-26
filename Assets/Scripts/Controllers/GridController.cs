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
        instance.ResetCrawlSpace();

        if (instance.fillableSpaces < 0)
        {
            instance.fillableSpaces = instance.CountFillableSpaces();
        }
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
     * When updating the value for the field, the corresponding
     * mask is also updated. The mask is handled by the game controller.
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

    /**
     * Create a line on the contact points.
     */
    public static List<Vector2> CreateLine(List<Vector3> contactPoints)
    {
        DividerUtils.PrintContactPoints(contactPoints);

        return instance.Fill(contactPoints);
        //instance.Fill(contactPoints);
    }

    /**
     * Compute the fill percentage for the current playing field.
     */
    public static float ComputeFillPercent()
    {
        // Retrieve the current count of fillable space.
        float newCount = instance.CountFillableSpaces();

        // Compute the percentage [0 - 1].
        return 1 - (newCount / instance.fillableSpaces);
    }

    private const int A = 0;

    private const int B = 1;

    // The main focus in this class. The storage for all values in the field.
    private GridValue[,] field;

    private GameObject container;

    // The amount of spaces available to be filled.
    private float fillableSpaces;

    /**
     * On create, initialize everything.
     */
    private GridController()
    {
        instance = this;
        
        // Initialize the field.
        field = new GridValue[DividerUtils.SIZE_X + 1, DividerUtils.SIZE_Y + 1];

        container = new GameObject("CrawlPoints");
    }

    /**
     * Reset the field table to contain spaces.
     */
    private void ResetField()
    {
        if (LevelManager.GetInstance() != null)
        {
            LevelManager.CreateLevel(field);
        }
        else
        {

            // Use this default behavior if no text file was loaded.
            for (int x = 0; x <= DividerUtils.SIZE_X; x++)
            {
                for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
                {
                    field[x, y] = GridValue.SPACE;

                    if (x == 0 || y == 0 || x == DividerUtils.SIZE_X || y == DividerUtils.SIZE_Y)
                    {
                        field[x, y] = GridValue.BOUNDS;
                    }

                    // For override, useful when testing.
                    //field[x, y] = GridValue.SPACE;
                }
            }
        }

        DividerUtils.PrintField(field);

        // Set the value of the fillable spaces to a negative value.
        // This will force for a recomputation of the fillable spaces.
        fillableSpaces = -1;
    }

    /**
     * Count all of the spaces that can be filled om a brute force manner.
     */
    private float CountFillableSpaces()
    {
        float result = 0f;

        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                result += field[x, y] == GridValue.SPACE ? 1f : 0f;
            }
        }

        return result;
    }

    /**
     * Reset the state of the crawl spaces.
     */
    private void ResetCrawlSpace()
    {
        /*
        // Destroy all of the existing crawl points instances.
        foreach (Transform crawlPoint in container.transform)
        {
            GameObject.Destroy(crawlPoint.gameObject);
        }

        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                if (field[x, y] == GridValue.CRAWL)
                {
                    GameObject crawlPoint = LineGenerator
                        .CreateCrawlPoint(DividerUtils.GridToUnitPoint(x, y));
                    crawlPoint.transform.parent = container.transform;
                }
            }
        }
        /**/

        List<LinePoints> boundingCoords = new List<LinePoints>();
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                if (field[x, y] == GridValue.BOUNDS)
                {
                    boundingCoords.AddRange(DividerUtils
                        .FindBoundingCoords(field, new Vector2(x, y)));
                }
            }
        }

        foreach (LinePoints linePoint in boundingCoords)
        {
            LineGenerator.GenerateLine(linePoint.GetPointA(), linePoint.GetPointB());
        }
    }

    /**
     * Execution point for changing the values on the grid.
     * This will return the list of coordinates to be filled gradually.
     */
    private List<Vector2> Fill(List<Vector3> contactPoints)
    {
        List<Vector2> result = new List<Vector2>();

        List<Vector2> gridCoords = new List<Vector2>();
        foreach (Vector2 contactPoint in contactPoints)
        {
            gridCoords.Add(DividerUtils.UnitToGridPoint(contactPoint.x, contactPoint.y));
        }

        // Retrieve the points from the grid coords list.
        Vector2 pointA = gridCoords[A];
        Vector2 pointB = gridCoords[B];

        LineGenerator.GenerateLine(
            DividerUtils.GridToUnitPoint((int)pointA.x, (int)pointA.y),
            DividerUtils.GridToUnitPoint((int)pointB.x, (int)pointB.y)
            );

        bool horizontal = pointA.y - pointB.y == 0;
        if (horizontal)
        {
            result.AddRange(FillHorizontal(pointA, pointB));
        }
        else
        {
            result.AddRange(FillVertical(pointA, pointB));
        }

        return result;
    }

    private List<Vector2> FillHorizontal(Vector2 pointA, Vector2 pointB)
    {
        List<Vector2> result = new List<Vector2>();

        for (int x = (int)pointA.x; x <= (int)pointB.x; x++)
        {
            field[x, (int)pointA.y] = GridValue.CRAWL;
        }

        List<Vector2> down = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(1, -1));
        List<Vector2> up = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(1, 1));

        result.AddRange(ProcessFill(down, up));


        return result;
    }

    private List<Vector2> FillVertical(Vector2 pointA, Vector2 pointB)
    {
        List<Vector2> result = new List<Vector2>();

        for (int y = (int)pointA.y; y <= (int)pointB.y; y++)
        {
            field[(int)pointA.x, y] = GridValue.CRAWL;
        }

        List<Vector2> left = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(-1, 1));
        List<Vector2> right = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(1, 1));

        result.AddRange(ProcessFill(left, right));

        return result;
    }

    private List<Vector2> ProcessFill(List<Vector2> sideA, List<Vector2> sideB)
    {
        List<Vector2> fillList = new List<Vector2>();

        int sideAEnemies = EnemyManager.CountOnPosition(sideA);
        int sideBEnemies = EnemyManager.CountOnPosition(sideB);

        if (sideAEnemies == 0 && sideBEnemies == 0)
        {
            // Fill everything if there are no more enemies.
            return DividerUtils.InterweaveLists(sideA, sideB);
        }

        if (sideA.Count > sideB.Count)
        {
            //ChangeValue(sideB, GridValue.FILLED);
            fillList.AddRange(sideB);
        }
        else if (sideA.Count < sideB.Count)
        {
            //ChangeValue(sideB, GridValue.FILLED);
            fillList.AddRange(sideA);
        }
        else
        {
            if (sideAEnemies > sideBEnemies)
            {
                //ChangeValue(sideB, GridValue.FILLED);
                fillList.AddRange(sideB);
            }
            else if (sideAEnemies < sideBEnemies)
            {
                //ChangeValue(sideA, GridValue.FILLED);
                fillList.AddRange(sideA);
            }
            else
            {
                // If everything is still equal, randomize.
                fillList.AddRange(Random.Range(0, 2) == 0 ? sideA : sideB);
            }
        }

        return fillList;
    }
    /*
    private void Fill(List<Vector3> contactPoints)
    {
        int length = contactPoints.Count;

        Vector2[] gridCoords = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            // Retrieve the grid version of the contact point values.
            Vector3 pos = contactPoints[i];
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

        LineGenerator.GenerateLine(
            DividerUtils.GridToUnitPoint((int)pointA.x, (int)pointA.y),
            DividerUtils.GridToUnitPoint((int)pointB.x, (int)pointB.y)
            );
        //).transform.parent = lineContainer.transform;
        // Fill the line along the y-axis.
        for (int x = (int)pointA.x; x <= (int)pointB.x; x++)
        {
            field[x, (int)pointA.y] = GridValue.CRAWL;
            ChangeMask(x, (int)pointA.y);
        }

        List<Vector2> down = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(1, -1));
        List<Vector2> up = DividerUtils.ProcessField(field, pointA, pointB, new Vector2(1, 1));

        ProcessFill(down, up);
        //DividerUtils.PrintField(field);
    }

    private void FillVertical(Vector2 pointA, Vector2 pointB)
    {

        LineGenerator.GenerateLine(
            DividerUtils.GridToUnitPoint((int)pointB.x, (int)pointB.y),
            DividerUtils.GridToUnitPoint((int)pointA.x, (int)pointA.y)
            );
        //).transform.parent = lineContainer.transform;
        for (int y = (int)pointB.y; y <= (int)pointA.y; y++)
        {
            field[(int)pointA.x, y] = GridValue.CRAWL;
            ChangeMask((int)pointA.x, y);
        }

        List<Vector2> left = DividerUtils.ProcessField(field, pointB, pointA, new Vector2(-1, 1));
        List<Vector2> right = DividerUtils.ProcessField(field, pointB, pointA, new Vector2(1, 1));

        ProcessFill(left, right);
        //DividerUtils.PrintField(field);
    }
    /**/
        }
