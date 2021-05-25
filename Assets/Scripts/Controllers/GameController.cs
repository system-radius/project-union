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

    private static float TRANSITION_LIMIT = 2f;

    private static float SPAWN_TIME_LIMIT = 2f;

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

    // The array of enemy prefabrications to be spawned.
    public GameObject[] enemyPrefabs;

    // The prefabrication to be spawned as the divider instance.
    public GameObject dividerPrefab;

    // The masking prefabrication for the puzzle picture.
    public GameObject maskPrefab;

    // The prefab for the line generator object.
    public GameObject lineGenPrefab;

    // A created instance at runtime. This represents the player in the game.
    private GameObject divider;

    // The current image for the level.
    private GameObject levelImage;

    // The array of game objects that has the "Start" tag.
    private GameObject[] startingObjects;

    // The array of game objects that has the "Finish" tag.
    private GameObject[] completionObjects;

    // The 2D array of the masking objects.
    private GameObject[,] masks;

    // The list of the coordinates to be filled gradually.
    private List<Vector2> fillCoords;

    // The current unmask limit. Will increase gradually,
    // eventually will be enough to cover the whole area to be filled.
    private float unmaskLimit = 1f;

    // The current fill percentage that has been accomplished.
    private float fillPercent = 0f;

    // The current time since the transition has started.
    private float transitionTimer = 0f;

    // The timer responsible respawning the player.
    private float respawnTimer = 0f;

    // The current level.
    private int currentLevel = -1;

    // Status for whether the transition has begun.
    private bool transitionStart = false;

    // Indicates the completion of the level.
    // Will be true if the finishing UI are set to active.
    private bool levelComplete = false;

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

        // Create an instance for the enemy manager.
        EnemyManager.CreateInstance(enemyPrefabs);

        // Create the list container.
        fillCoords = new List<Vector2>();

        // Create the initial container for the masks.
        masks = new GameObject[DividerUtils.SIZE_X + 1, DividerUtils .SIZE_Y + 1];

        // Retrieve the starting objects.
        startingObjects = GameObject.FindGameObjectsWithTag("Start");

        // Retrieve the completion game objects.
        completionObjects = GameObject.FindGameObjectsWithTag("Finish");

        // Create the line generator. There should only be one instance at any point.
        Instantiate(lineGenPrefab);

        DisplayCompleteLevel(false);

        // Start with level completed.
        levelComplete = true;
    }

    /**
     * This class is also responsible for tracking the progress of the current
     * game. If other classes not affiliated to an object needs to be updated,
     * call them here.
     */
    void FixedUpdate()
    {
        if (levelComplete)
        {
            ProcessComplete();

            // If the level is already recorded as complete, no more processing is required.
            return;
        }

        if (divider == null)
        {
            // Begin counting towards the respawn of the player.
            respawnTimer += Time.deltaTime;

            // If the time has exceeded the specified limit,
            if (respawnTimer >= SPAWN_TIME_LIMIT)
            {
                // Spawn the player.
                respawnTimer = 0f;
                SpawnDivider();
            }
        }
        else
        {
            // Always resset the respawn timer to 0 otherwise.
            respawnTimer = 0f;
        }

        ProcessFill();
        ProcessQuota();
    }

    /**
     * Increase the current level.
     */
    public void AdvanceLevel() {

        currentLevel = currentLevel + 1 >= levelPrefabs.Length ? 0 : currentLevel + 1;
        Reset();
    }

    public void Reset()
    {
        // Reset the grid controller to its default state.
        GridController.StaticReset();

        // Create/reset the masks instances.
        CreateMasks();

        // Create the level image.
        CreateLevelImage();

        // Spawn the divider.
        SpawnDivider();

        // Cancel the display for the starting messages and UI.
        DisplayStartingObjects(false);

        // Cancel the display for the UI stuff for the completion of the level.
        DisplayCompleteLevel(false);

        // Reset the transition timer.
        transitionTimer = 0f;

        // Reset the transition status.
        transitionStart = false;

        // Reset the completion status.
        levelComplete = false;

        EnemyManager.ResetEnemies(currentLevel);
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
        // If the transition has not yet started and if the current
        // fill percent has reached or exceeded the fill quota
        if (!transitionStart && fillPercent >= DividerUtils.FILL_QUOTA)
        {
            // Display the full image.
            DisplayFullImage();
            transitionStart = true;
        }

        // If the transition is in progress
        if (transitionStart)
        {
            // Add to the transition timer.
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= TRANSITION_LIMIT)
            {
                transitionStart = false;

                // Display the completion level objects.
                DisplayCompleteLevel();
            }
        }
    }

    /**
     * Function responsible for processing the completion of the level.
     * Wait for the input from the user, then call to advance the level
     * and reset.
     */
    private void ProcessComplete()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                AdvanceLevel();
            }
        } else if (Input.GetMouseButtonUp(0)) {
            AdvanceLevel();
        }
    }

    private void DisplayStartingObjects(bool display = true)
    {
        DisplayUI(startingObjects, display);
    }

    private void DisplayCompleteLevel(bool display = true)
    {
        DisplayUI(completionObjects, display);
    }

    private void DisplayUI(GameObject[] uiObjects, bool display = true)
    {
        levelComplete = display;
        foreach (GameObject gameObject in uiObjects)
        {
            gameObject.SetActive(display);
        }
    }

    /**
     * Go through all of the masks and set them to true.
     */
    private void DisplayFullImage()
    {
        // Remove the divider.
        Destroy(divider);

        for (int x = 0; x <= DividerUtils.SIZE_X; x++)
        {
            for (int y = 0; y <= DividerUtils.SIZE_Y; y++)
            {
                masks[x, y].SetActive(true);
            }
        }

        // Reset the fill percentage.
        fillPercent = 0f;

        // On display of the full image, clear the lines as well.
        LineGenerator.ClearLines();
    }
}
