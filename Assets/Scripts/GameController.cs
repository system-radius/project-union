using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{

    private static GameController instance;

    // An array containing the pictures to be used as levels.
    public GameObject[] levelPrefabs;

    // The divider object to be created.
    public GameObject dividerPrefab;

    // The prompt text that displays "Tap to continue."
    private GameObject continueTap;

    private GameObject lastText;

    // The current image for the level.
    private GameObject levelImage;

    // The divider instance, main object controlled by the player.
    private GameObject divider;

    // The grid controller script, mainly used to gain insight
    // about the state of the game.
    private GridController gridController;

    // The timer for when the player dies for whatever reason.
    private float deathTimer = 0f;

    // The time limit for spawning. May be lengthen or shorten.
    private float deathLimiter = 2f;

    // The current time that has elapsed since the level has been completed.
    private float levelCompleteTimer = 0;

    // Whether the transition text can be displayed or not.
    private bool transitionStart = false;

    private int currentLevel = -1;

    public static GameController GetInstance()
    {
        // No need to worry about the instance being null.
        // On scene start, the instance is initialized in the awake method.
        return instance;
    }

    public static void DisplayStats()
    {
        // Reset the contents.
        string stats = "";

        if (instance.divider != null)
        {
            Divider dividerSource = instance.divider.GetComponent<Divider>();

            stats += "Divider status: alive\n";
            stats += "Division: " + dividerSource.IsDividing() + "\n";
            stats += "Bullets hit: " + dividerSource.GetBulletCount() + "\n";
        }
        else
        {
            stats += "Divider status: dead\n";
        }

        instance.lastText.GetComponent<TextMeshProUGUI>().text = stats;
    }

    /**
     * Called on scene start.
     */
    void Awake()
    {
        // Cache the instance of this game object to be accessible to every other object.
        instance = this;

        gameObject.AddComponent<EnemyManager>();

        // Get the grid controller script from the grid container object.
        gridController = GameObject.FindGameObjectWithTag("GridController").GetComponent<GridController>();

        // Get the continue prompt instance.
        continueTap = GameObject.FindGameObjectWithTag("Finish");

        lastText = GameObject.FindGameObjectWithTag("EditorOnly");

        // Call to reset every thing as first step.
        Reset();
    }

    /**
     * Framerate-independent update.
     */
    void FixedUpdate()
    {
        DisplayStats();
        if (divider == null) {

            // Check for the respawn time.
            deathTimer += Time.fixedDeltaTime;

            if (deathTimer >= deathLimiter)
            {
                SpawnDivider();
            }

            // While the player is dead, do not do anything else.
            return;
        }

        // If the quota has been reached, and the transition has not started yet...
        if (gridController.GetFillPercent() >= DividerUtils.FILL_QUOTA && !transitionStart)
        {
            // display the full image. Displaying the full image will set the transition start.
            DisplayFullImage();
        }

        // If the level is under transition and the prompt has not been displayed yet...
        if (transitionStart && !continueTap.activeSelf)
        {
            // Continuously add to the amount of time that has passed since the level completion.
            levelCompleteTimer += Time.deltaTime;

            // If the accumulated time has reached the alloted time for transition,
            if (levelCompleteTimer >= DividerUtils.TRANSITION_TIME)
            {
                // make the prompt appear.
                continueTap.SetActive(true);
            }
        }

        // If the prompt has appeared...
        if (continueTap.activeSelf)
        {
            // Continuously listen for a tap from the user.
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    // Reset the whole level.
                    Reset();
                }

                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Reset();
            }
        }
    }

    public void Reset()
    {

        transitionStart = false;
        levelCompleteTimer = 0;

        gridController.Reset();
        SpawnDivider();

        currentLevel = currentLevel + 1 >= levelPrefabs.Length ? 0 : currentLevel + 1;

        EnemyManager.ResetEnemies();

        if (levelImage != null)
        {
            Destroy(levelImage);
        }
        levelImage = Instantiate(levelPrefabs[currentLevel]);

        continueTap.SetActive(false);
    }

    /**
     * Force the game to display the complete version of the level image.
     */
    private void DisplayFullImage()
    {

        if (transitionStart)
        {
            return;
        }

        // Fill the remaining spaces.
        gridController.ForceFillSpaces(new Vector2(0, 0), new Vector2(DividerUtils.SIZE_X, DividerUtils.SIZE_Y), 1, 1);

        // Remove the divider.
        Destroy(divider);

        // Remove the lines created.
        gridController.ClearLines();

        transitionStart = true;
    }

    private void SpawnDivider()
    {
        if (divider != null)
        {
            Destroy(divider);
        }

        divider = Instantiate(dividerPrefab);
        deathTimer = 0f;
    }

    public GameObject GetDivider()
    {
        return divider;
    }

    public int GetLevel()
    {
        return currentLevel;
    }
}
