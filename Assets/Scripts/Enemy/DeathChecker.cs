using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathChecker : MonoBehaviour
{
    public GameObject explosion;

    private Rigidbody2D rb;

    private SpriteRenderer sr;

    private float deathTimer = 2f;

    private bool dead = false;

    void Start()
    {
        FindSpriteRenderer(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            // Do nothing if not dead.
            return;
        }

        deathTimer -= Time.deltaTime;
        if (deathTimer <= 0f)
        {
            // Destroy the game object if it has passed the death timer.
            Destroy(gameObject);
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.collider.tag;

        if (tag == DividerUtils.TAG_ENEMY_KILLER && !dead)
        {
            // If the collision is hostile to the enemy,
            // Spawn the explosion particles.
            Instantiate(explosion, transform);

            // Remove the sprite.
            sr.enabled = false;

            // Stop movement.
            rb.velocity = Vector2.zero;

            // "Kill" the current game object.
            dead = true;
        }
        
    }

    private bool FindSpriteRenderer(GameObject gameObject)
    {
        sr = gameObject.GetComponent<SpriteRenderer>();

        bool result = sr != null;

        if (!result)
        {
            foreach (Transform child in transform)
            {
                result = FindSpriteRenderer(child.gameObject);
                if (result) break;
            }
        }

        return result;
    }
}
