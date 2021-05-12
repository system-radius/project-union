using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public GameObject impactPrefab;

    public float speed = 15f;

    private Rigidbody2D rigidBody;

    private bool hit { get; set; }

    void Start()
    {
        hit = false;

        // Take the rigid body component for this object.
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ColliderLimit")
        {
            // Process the hit for ColliderLimit tag.
            ProcessColliderLimit();
        }
    }

    /**
     * This processes the hit if the bullet has collided with the bordering limits,
     * as well as the generated lines.
     */
    private void ProcessColliderLimit()
    {

        if (hit)
        {
            return;
        }

        // Instead of destroying this object, reset its velocity.
        rigidBody.velocity = new Vector2(0, 0);
        hit = true;

        transform.Rotate(new Vector3(180, 0, 0));
    }

    public bool HasHit()
    {
        return hit;
    }
}
