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

        if (collision.tag == "Bullet")
        {
            return;
        }

        // Instead of destroying this object, reset its velocity.
        rigidBody.velocity = new Vector2(0, 0);
        hit = true;
    }

    public bool HasHit()
    {
        return hit;
    }
}
