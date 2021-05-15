using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    private Vector3 rate;

    // Start is called before the first frame update
    void Start()
    {
        // At the start, choose a random direction move towards that.

        rate = new Vector3(0, 0, Random.Range(0, 2) == 0 ? -1f : 1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Continuously rotate the spinner.
        transform.Rotate(rate);
    }
}
