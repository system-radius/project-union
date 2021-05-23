using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathChecker : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.collider.tag;

        if (tag == DividerUtils.TAG_ENEMY_KILLER)
        {
            // If the collision is hostile to the enemy,
            Destroy(gameObject);
        }
        
    }
}
