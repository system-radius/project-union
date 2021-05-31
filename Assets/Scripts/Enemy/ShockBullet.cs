using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockBullet : MonoBehaviour
{
    public float speed = 15f;

    private Rigidbody2D rigidBody;

    void Start()
    {
        //GameObject divider = GameController.GetInstance().GetDivider();

        //transform.Rotate(0, 0, DividerUtils.ComputeAngle(transform.position, divider.transform.position));
        transform.Rotate(new Vector3(180, 0, 0));

        // Take the rigid body component for this object.
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        //Debug.Log(tag);

        if (tag == (DividerUtils.TAG_LIMIT))
        {
            Destroy(gameObject);
        }
    }


}
