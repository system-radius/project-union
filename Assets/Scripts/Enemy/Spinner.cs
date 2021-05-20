using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float angle = 1f;

    private Vector3 rate;

    // Start is called before the first frame update
    void Start()
    {
        // At the start, choose a random direction move towards that.
        float multiplier = Random.Range(0, 2) == 0 ? -1f : 1f;
        angle *= multiplier;

        rate = new Vector3(0, 0, angle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Always update the rate (z-axis).
        rate.z = angle;

        // Continuously rotate the spinner.
        transform.Rotate(rate);
    }
}
