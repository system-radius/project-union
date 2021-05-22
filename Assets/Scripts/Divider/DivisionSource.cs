using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The class handling the division source's behavior.
 * A division source is responsible for shooting a bullet
 * while the division is in progress. The bullet is attached
 * to source using a line renderer.
 */
public class DivisionSource : MonoBehaviour
{
    // The prefabricated object for the trail, the line
    // renderer to attach the bullet.
    public GameObject trailPrefab;

    // The prefabricated object for the bullet, the object
    // that is shot out from the source.
    public GameObject bulletPrefab;

    // The source for the line renderer.
    private const int SOURCE = 0;

    // The target for the line renderer.
    private const int TARGET = 1;

    // The line renderer, taken from the instantiated trail.
    private LineRenderer lineRenderer;

    // The capsule-shaped collision for the trail.
    private CapsuleCollider2D lineCollider;

    // The transform component of the bullet.
    private Transform targetTransform;

    private Vector3 lastPosition;

    // The source script for the bullet.
    private Bullet bulletSource;

    // The status of this source, whether it is active or not.
    private bool active;

    // A checker for whether the bullet has already hit something.
    private bool complete;

    /**
     * The update method will be responsible for updating the size
     * of the collider, as well as the position of the bullet
     * which is referenced as the target for the line.
     */
    void FixedUpdate()
    {
        if (!active)
        {
            // Return right away if the division source is not active.
            return;
        }

        lineRenderer.SetPosition(TARGET, targetTransform.position);

        lineCollider.size = new Vector2(0.5f, (targetTransform.position - transform.position).magnitude);
        lineCollider.transform.position = transform.position + (targetTransform.position - transform.position) / 2;

        if (!complete && lastPosition == targetTransform.position)
        {
            // If the current bullet has not hit anything yet,
            // and the last known position and the current target's position
            // are the same, then mark this source as complete.
            complete = true;
        }

        // Record the last known position.
        lastPosition = targetTransform.position;
    }

    /**
     * Create a bullet instance for this division source.
     */
    private void CreateBullet()
    {
        // Create a bullet instance that shots out from this source.
        // Make the bullet follow the direction of the current source.
        targetTransform = Instantiate(bulletPrefab, transform.position, transform.rotation).transform;

        // Retrieve the source script from the bullet instance.
        bulletSource = targetTransform.GetComponent<Bullet>();
    }

    /**
     * Create a line to connect the bullet to this source.
     */
    private void CreateLine()
    {
        // Create a line connecting this source to the bullet.
        // Cache the trail's line renderer.
        lineRenderer = Instantiate(trailPrefab, transform.position, transform.rotation).GetComponent<LineRenderer>();

        // Create a capsule collider attached to the line renderer.
        lineCollider = lineRenderer.gameObject.GetComponent<CapsuleCollider2D>();
        lineCollider.transform.rotation = transform.rotation;
    }

    /**
     * On activate, this source will fire a bullet. This will also
     * set the active status to true, so that only one instance
     * of activation can happen at one time.
     */
    public void Activate()
    {
        if (active)
        {
            // Return right away if the source is already active.
            return;
        }

        // Set the status.
        active = true;
        CreateBullet();
        CreateLine();

        lineRenderer.SetPosition(SOURCE, transform.position);
        lineRenderer.SetPosition(TARGET, targetTransform.position);
    }

    /**
     * On deactivation, destroy the trail and bullet instances.
     */
    public void Deactivate()
    {
        if (!active)
        {
            // Return right away if the source is not active.
            return;
        }

        // Set the status.
        active = false;

        Destroy(lineRenderer.gameObject);
        Destroy(targetTransform.gameObject);
    }

    public bool IsComplete()
    {
        return complete;
    }
}
