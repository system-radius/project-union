using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

    public float speed = 15f;

    private Rigidbody2D rigidBody;

    private Vector3 pastVelocity;

    // Start is called before the first frame update
    void Start()
    {
        // Decide on a random direction.
        transform.Rotate(0, 0, Random.Range(0, 360));

        // Cache the rigid body.
        rigidBody = GetComponent<Rigidbody2D>();

        // Move towards the decided direction.
        rigidBody.velocity = transform.up * speed;

        // Remove the ability for further rotate.
        rigidBody.freezeRotation = true;
    }

    void FixedUpdate()
    {
        pastVelocity = rigidBody.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string name = collision.collider.name;

        ContactPoint2D contact = collision.contacts[0];
        rigidBody.velocity = Vector3.Reflect(pastVelocity, contact.normal);

        /*
        if (name.Equals("ColliderLeft"))
        {
            transform.position = Vector3.Reflect(-transform.position, Vector3.right);
        }
            
        else if (name.Equals("ColliderRight"))
        {
            transform.position = Vector3.Reflect(-transform.position, Vector3.right);
            //transform.Rotate(0, 0, (180 - transform.rotation.z));
        }

        else if (name.Equals("ColliderTop"))
        {
            transform.position = Vector3.Reflect(transform.position, Vector3.up);
        }

        else if (name.Equals("ColliderBot"))
        {
            transform.position = Vector3.Reflect(transform.position, Vector3.up);
            //transform.Rotate(0, 0, (180 - transform.rotation.z));
        }
        */
        // rotate the object by the same ammount we changed its velocity
        Quaternion rotation = Quaternion.FromToRotation(pastVelocity, rigidBody.velocity);
        transform.rotation = rotation * transform.rotation;
    }

}
