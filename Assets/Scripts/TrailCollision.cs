using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollision : MonoBehaviour
{
    public GameObject shockPrefab;

    private LineRenderer lineRenderer;

    private CapsuleCollider2D capsuleCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == DividerUtils.TAG_ENEMY)
        {
            ProcessEnemyCollision(collision);
        }
    }

    /**
     * If the enemy touches the trail as divide is being conducted,
     * send a shockewave back to the divider.
     */
    private void ProcessEnemyCollision(Collider2D c)
    {

        // Get the contact point from the collider.
        Vector2 contactPoint = c.ClosestPoint(capsuleCollider.transform.position);

        Vector3 position = new Vector3(transform.position.x, contactPoint.y, 0);

        // Spawn the shockwave.
        Instantiate(shockPrefab, position, transform.rotation);

    }
}
