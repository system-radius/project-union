using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ephemeral : MonoBehaviour
{
    public float lifeTime = 2f;

    void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
