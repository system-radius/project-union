using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The main class that references how the Divider game object acts.
 * This class is only responsible for retrieving the area that will
 * be filled. The actual filling is to be handled by another controller.
 */
public class Divider : MonoBehaviour
{
    // Explosion prefab.
    public GameObject explosion;

    // There will always be two sides to the divider:
    // - one on the top and down (vertical).
    // - one on left and right (horizontal).
    private const int SIDES = 2;

    // An array of the divide sources, responsible for
    // producing a bullet which will mark the point of contact.
    private DivisionSource[] divisionSources = new DivisionSource[SIDES];

    // The animator to be used for changing the appearance of the divider.
    private Animator animator;

    // The status for whether the divide is in progress.
    private bool dividing = false;

    // A switch for when the line is complete.
    private bool lineComplete = false;

    // The alive/death state of the player object.
    private bool alive;

    /**
     * Initialize the things needed by this Divider.
     */
    void Start()
    {
        // Set the alive status to true.
        alive = true;

        // Retrieve the animator component.
        animator = GetComponent<Animator>();

        // Add the player collider to the game object.
        // This will ensure that the Divider class is already created
        // by the time that the player collider attempted to retrieve it.
        gameObject.AddComponent<PlayerCollider>();

        int divideCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Divide Source") && divideCounter < divisionSources.Length)
            {
                divisionSources[divideCounter] = child.gameObject.GetComponent<DivisionSource>();
                divideCounter++;
            }

            // Check if the division source array have been filled.
            if (divideCounter >= divisionSources.Length)
            {
                // exit from the loop if they are.
                break;
            }
        }
    }

    /**
     * Continuously poll for the status of the sources when dividing.
     */
    void FixedUpdate()
    {
        // The player is not allowed to do anything if they are dead.
        if (!alive)
        {
            return;
        }

        ProcessDivide();
    }

    /**
     * Process the ongoing divide mechanism.
     */
    private void ProcessDivide()
    {
        if (!dividing)
        {
            // Do not check for anything when not dividing.
            return;
        }

        if (IsSourcesComplete() && !lineComplete)
        {
            // Line complete!
            Debug.Log("Line Complete!");
            CreateLine();
            lineComplete = true;

            // Deactivate all sources.
            DeactivateSources();

            // But do not cancel the division just yet.
        }
    }

    /**
     * Create a line. Retrieve the line points from the sources.
     */
    private void CreateLine()
    {
        List<Vector3> contactPoints = new List<Vector3>();
        foreach (DivisionSource source in divisionSources)
        {
            contactPoints.Add(source.GetTargetLastPosition());
        }

        // Add all of the retrieved coords to the game controller.
        GameController.AddFillCoords(GridController.CreateLine(contactPoints));
    }

    /**
     * Check if all of the sources have completed their bullet shot.
     */
    private bool IsSourcesComplete()
    {
        bool result = true;

        foreach (DivisionSource source in divisionSources)
        {
            if (source == null) {
                return false;
            }

            result = result && source.IsComplete();
        }

        return result;
    }

    /**
     * Call for the sources to be activated.
     */
    private void ActivateSources()
    {
        foreach (DivisionSource source in divisionSources)
        {
            source.Activate();
        }
    }

    /**
     * Call for the sources to be deactivated.
     */
    private void DeactivateSources()
    {
        foreach (DivisionSource source in divisionSources)
        {
            source.Deactivate();
        }
    }

    /**
     * Start the dividing process.
     * This method will activate the divide sources, and the sources
     * will take care of the processing themselves.
     */
    public void Divide()
    {
        if (dividing || !alive)
        {
            // There is no need to get dividing set up all over again.
            // Also check if the divider is still alive.
            return;
        }

        dividing = true;
        ResetAnimator();

        ActivateSources();

        Debug.Log("Start division!");
    }

    /**
     * Stop the division process. This is called when the divider
     * instance has either completed the division (both sides have
     * hit the end points), or if the divider instance is destroyed.
     */
    public void StopDivide()
    {
        if (!dividing)
        {
            // If there is no divide in progress, return right away.
            return;
        }

        dividing = false;
        ResetAnimator();

        lineComplete = false;

        DeactivateSources();

        Debug.Log("Stop division!");
    }

    /**
     * Move the divider to the position as indicated.
     */
    public void Move(Vector3 position)
    {
        // Turn the current vector 3 to the "exact" coordinates as permitted by the grid.
        Vector2 gridCoords = DividerUtils.UnitToGridPoint(position.x, position.y);

        if ((gridCoords.x < 0 || gridCoords.x > DividerUtils.SIZE_X + 1)
            || (gridCoords.y < 0 || gridCoords.y > DividerUtils.SIZE_Y + 1)) {
            // Return right away if the coordinates are outside the playing field.
            return;
        }
        position = DividerUtils.GridToUnitPoint(gridCoords.x, gridCoords.y);

        // Always set the Z-coordinate to 0.
        position.z = 0f;

        transform.position = position;
    }

    /**
     * Change the current form of animation based on the dividing status.
     */
    public void ResetAnimator()
    {
        animator.SetBool("Dividing", dividing);
    }

    /**
     * Check if the current position of the divider will allow for divide.
     */
    public bool IsAllowedDivide()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        return DividerUtils.GetGridValue(GridController.GetField(), x, y) == GridValue.SPACE;
    }

    /**
     * Check if a division is in progress.
     */
    public bool IsDividing()
    {
        return dividing;
    }

    /**
     * Upon killing the player, automatically stop the divide as well.
     */
    public void KillPlayer()
    {
        alive = false;
        StopDivide();
    }
}
