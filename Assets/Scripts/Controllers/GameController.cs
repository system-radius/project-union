using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A static class responsible for controlling the whole game.
 * The only one to contain an awake method. Every class that do
 * not need to be spawned will rely on this class to be created.
 */
public class GameController : MonoBehaviour
{
    private static GameController instance;

    private static float UNMASK_RATE = 1.25f;

    /**
     * Allows for the retrieval of the held instance, statically.
     */
    public static GameController GetInstance()
    {
        return instance;
    }

    /**
     * Add the specified list of coords in the current list to be filled.
     */
    public static void AddFillCoords(List<Vector2> coords)
    {
        instance.fillCoords.AddRange(coords);
    }

    /**
     * A utility function used to change the masking
     * for the specified coordinate.
     */
    public static void ChangeMasking(Vector2 coord, bool forced = false)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;

        ChangeMasking(x, y, forced);
    }

    public static void ChangeMasking(int x, int y, bool forced = false)
    {
        // Change the mask active status, either forcibly
        // or if the corresponding grid value is filled.
        instance.masks[x, y].SetActive(forced || GridController.GetField()[x, y] == GridValue.FILLED);
    }

    //--- Non-static attributes ---

    // The array that contains all of the levels to be displayed.
    public GameObject[] levelPrefabs;

    // The prefabrication to be spawned as the divider instance.
    public GameObject dividerPrefab;

    // The masking prefabrication for the puzzle picture.
    public GameObject maskPrefab;

    // A created instance at runtime. This represents the player in the game.
    private GameObject divider;

    // The current image for the level.
    private GameObject levelImage;

    // The 2D array of the masking objects.
    private GameObject[,] masks;

    // The list of the coordinates to be filled gradually.
    private List<Vector2> fillCoords;

    // The current unmask limit. Will increase gradually,
    // eventually will be enough to cover the whole area to be filled.
    private float unmaskLimit = 1f;

    // The current fill percentage that has been accomplished.
    private float fillPercent = 0f;

    // The current level.
    private int currentLevel = -1;

    /**
     * On awake, initialize all that is needed. All other controllers and such.
     * Determine which classes are essential to be created only at level start.
     */
    void Awake()
    {
        // Assign this created instance to a static variable.
        instance = this;

        // Create an instnace for the grid controller.
        GridController.CreateInstance();

        // Create the list container.
        fillCoords = new List<Vector2>();

        // Create the initial container for the masks.
        masks = new GameObject[DividerUtils.SIZE_X + 1, DividerUtils .SIZE_Y + 1];

        // Reset this instance as a first step.
        Reset();
    }

    /**
     * This class is also responsible for tracking the progress of the current
     * game. If other classes not affiliated to an object needs to be updated,
     * call them here.
     */
    void FixedUpdate()
    {
        ProcessFill();
        ProcessQuota();
    }

    public void Reset()
    {
        // Reset the grid controller to its default state.
        GridController.StaticReset();

        // Create/reset the masks instances.
        CreateMasks();

        currentLevel = currentLevel + 1 >= levelPrefabs.Length ? 0 : currentLevel + 1;

        // Create the level image.
        CreateLevelImage();

        // Spawn the divider.
        SpawnDivider();
    }

    /**
     * Create the level image.
     */
    private void CreateLevelImage()
    {
        if (levelImage != null)
        {
            Destroy(levelImage);
        }

        levelImage = Instantiate(levelPrefabs[currentLevel]);
    }

    /**
     * Spawn the divider. If a divider is currently spawned, destroy it
     * and then spawn another.
     */
    private void SpawnDivider()
    {
        if (divider != null)
        {
            Destroy(divider);
        }

        divider = Instantiate(dividerPrefab);
    }

    /**
     * Create the contents for the masking array. All of them are set
     * to inactive initially. This method may also be used to reset the masks.
     */
    private void CreateMasks()
    {
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                Vector2 coord = DividerUtils.GridToUnitPoint(x, y);

                if (masks[x, y] == null)
                {
                    GameObject mask = Object.Instantiate(maskPrefab, transform);
                    mask.transform.position = new Vector3(coord.x, coord.y);
                    masks[x, y] = mask;
                }

                //masks[x, y].GetComponent<SpriteMask>().enabled = false;
                masks[x, y].SetActive(false);
            }
        }
    }

    /**
     * Process the fill command, based on the provided coordinates.
     * This function will not do anything if there are no new fill coordinates.
     */
    private void ProcessFill()
    {
        // If the fill coordinates list does not contain anything...
        if (fillCoords.Count <= 0)
        {
            // There is no need to proceed.
            return;
        }

        // If the fill coords list has content.
        // Increase the unmask limit every time the loop is hit.
        unmaskLimit *= UNMASK_RATE;

        // The unmask limit is not supposed to be greater than the amount of the current
        // coordinates to be filled.
        unmaskLimit = unmaskLimit > fillCoords.Count ? fillCoords.Count : unmaskLimit;

        for (int i = 0; i < (int)unmaskLimit; i++)
        {
            // The primary concern is always the first item in the list.
            Vector2 coord = fillCoords[0];
            fillCoords.RemoveAt(0);

            GridController.UpdateFieldValue(coord, GridValue.FILLED);
            ChangeMasking(coord);
        }

        // Once the fill coordinates are exhausted
        if (fillCoords.Count == 0)
        {
            // Reset the unmask limit.
            unmaskLimit = 1f;

            // Compute the fill percentage.
            fillPercent = GridController.ComputeFillPercent();
        }
    }

    /**
     * Function responsible for processing if the quota has been reached.
     */
    private void ProcessQuota()
    {
        // If the current fill percent has reached or exceeded the fill quota
        if (fillPercent >= DividerUtils.FILL_QUOTA)
        {
            // Display the full image.
            DisplayFullImage();
        }
    }

    /**
     * Go through all of the masks and set them to true.
     */
    private void DisplayFullImage()
    {
        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                masks[x, y].SetActive(true);
            }
        }
    }
}
