using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The main class that references how the Divider game object acts.
 */
public class Divider : MonoBehaviour
{
    // There will always be two sides to the divider:
    // - one on the top and down (vertical).
    // - one on left and right (horizontal).
    private const int SIDES = 2;

    // An array of the divide sources, responsible for
    // producing a bullet which will mark the point of contact.

    // The collider tied to the game object.
    // This will be activated when the divide is in progress.
    // Probably best to have the collider always active.
    // The change is that the divider will not be affected
    // by enemies if the divide is not in progress.
    private CapsuleCollider2D capsuleCollider;

    // The animator to be used for changing the appearance of the divider.
    private Animator animator;

    // The status for whether the divide is in progress.
    private bool dividing = false;

    // A switch for when the line is complete.
    private bool lineComplete = false;

    /**
     * Initialize the things needed by this Divider.
     */
    void Start()
    {
        // Retrieve the animator component.
        animator = GetComponent<Animator>();

        // Retrieve the collider component.
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    /**
     * Start the dividing process.
     * This method will activate the divide sources, and the sources
     * will take care of the processing themselves.
     */
    public void Divide()
    {
        if (dividing)
        {
            // There is no need to get dividing set up all over again.
            return;
        }

        dividing = true;
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
     * Check if the current position of the divider will allow for divide.
     */
    public bool IsAllowedDivide()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        return DividerUtils.GetGridValue(GridController.GetField(), x, y) == GridValue.SPACE;
    }
}
