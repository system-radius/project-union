using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A component to be given to objects that shots forward.
 * Objects with this component must have a rigid body.
 */
public class Bullet : MonoBehaviour
{
    // Particles for on connect.
    public GameObject connect;

    // Attribute to control the speed of this object flying forward.
    public float speed = 15f;

    // The rigid body to be used as means of movement for the object.
    private Rigidbody2D rigidBody;

    /**
     * Right at the start of the bullet, make it move forward.
     */
    void Start()
    {
        // Retrieve the rigid body component.
        rigidBody = GetComponent<Rigidbody2D>();

        // Set the velocity to move forward.
        rigidBody.velocity = transform.up * speed;
    }

    /**
     * Process the collision event.
     */
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(DividerUtils.TAG_LIMIT))
        {
            // Process the hit for ColliderLimit tag.
            ProcessColliderLimit();
        }
    }

    private void ProcessColliderLimit()
    {
        Debug.Log("Hit a collider limit!");
        rigidBody.velocity = new Vector2(0, 0);
        Destroy(gameObject);

        /*
        Vector3 angles = transform.rotation.eulerAngles;

        // Remove the rotation on other angles aside from Z-Axis.
        // Swap the values for the vector.
        float x = angles.x;
        angles.x = angles.y;
        angles.y = angles.z;
        angles.z = x;

        Transform temp = Instantiate(connect, transform.position, transform.rotation).transform;
        temp.Rotate(angles);
        /**/
    }
}
