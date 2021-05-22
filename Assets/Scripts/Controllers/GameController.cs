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

    /**
     * Allows for the retrieval of the held instance, statically.
     */
    public static GameController GetInstance()
    {
        return instance;
    }

    //--- Non-static attributes ---

    // The prefabrication to be spawned as the divider instance.
    public GameObject dividerPrefab;

    // A created instance at runtime. This represents the player in the game.
    private GameObject divider;

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
        
    }

    public void Reset()
    {
        // Reset the grid controller to its default state.
        GridController.StaticReset();

        // Spawn the divider.
        SpawnDivider();   
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


}
