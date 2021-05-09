using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 20f;

    private Rigidbody2D rigidBody;

    void Start()
    {
        // Take the rigid body component for this object.
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        Debug.Log(collision.name);
    }
}
