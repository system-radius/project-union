using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
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
        Debug.Log("Dead player!");
        Destroy(gameObject);
    }
}
