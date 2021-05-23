using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{

    private Divider divider;

    void Start()
    {
        // Retrieve the divider instance.
        divider = GetComponent<Divider>();
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

        // Make the divider stop its division process immediately.
        divider.StopDivide();

        Destroy(gameObject);
    }
}
