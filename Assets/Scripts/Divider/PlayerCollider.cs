using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    public GameObject explosion;

    private Divider divider;

    private SpriteRenderer sr;

    private float deathTimer = 2f;

    private bool dead = false;

    void Start()
    {
        // Retrieve the divider instance.
        divider = GetComponent<Divider>();
        sr = GetComponent<SpriteRenderer>();
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
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;

        if (tag == (DividerUtils.TAG_ENEMY))
        {
            // An enemy has hit the player.
            KillPlayer();
        }
        else if (tag == (DividerUtils.TAG_SHOCK))
        {
            // A shockwave bullet has hit the player.
            Destroy(collision.gameObject);
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (divider == null || !divider.IsDividing())
        {
            // Ignore the collision if the divider is not enabled.
            // The player can only be killed while dividing.
            return;
        }
        Debug.Log("Dead player!");

        // Create the particles.
        Instantiate(explosion, transform);

        // Disable the sprite renderer.
        sr.enabled = false;

        // Make the divider stop its division process immediately.
        divider.KillPlayer();

        // Set the switch variable.
        dead = true;
    }
}
