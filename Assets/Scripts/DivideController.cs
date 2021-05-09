using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideController : MonoBehaviour
{

    public GameObject bulletPrefab;

    private const int A = 0;

    private const int B = 1;

    // There are only two divide sources.
    private GameObject[] divideSource = new GameObject[2];

    private GameObject[] fillDetect = new GameObject[2];

    private GameObject[] lasers = new GameObject[2];

    private GameObject[] bullets = new GameObject[2];

    private Touch touch;

    private bool dividing = false;

    private bool lineComplete = false;

    void Start()
    {
        int divideCounter = 0;
        int fillCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Divide Source" && divideCounter < divideSource.Length)
            {
                divideSource[divideCounter] = child.gameObject;
                divideCounter++;
            }
            else if (child.tag == "Fill Detection" && fillCounter < fillDetect.Length)
            {
                fillDetect[fillCounter] = child.gameObject;
                fillCounter++;
            }

            // Check if the two arrays have been filled.
            if (fillCounter >= fillDetect.Length && divideCounter >= divideSource.Length)
            {
                // exit from the loop if they are.
                break;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!dividing)
        {
            // Nothing to do here if there is no divide in progress.
            return;
        }

        if (touch.phase == TouchPhase.Ended)
        {
            // If the current touch is lifted, the divide is stopped.
            StopDivide();
            return;
        }

        // Do everything that needs to be done while dividing here.
        if (bullets[A] == null && bullets[B] == null && !lineComplete)
        {
            // While the division is in effect, both bullets becoming null
            // means that both have hit a collider.
            Debug.Log("Line created!");
            lineComplete = true;
        }
    }

    /**
     * Stop the dividing state.
     */
    public void StopDivide()
    {
        lineComplete = false;
        dividing = false;
        Destroy(bullets[A]);
        Destroy(bullets[B]);
    }

    /**
     * Initialize the dividing state.
     */
    public void Divide(Touch touch)
    {
        this.touch = touch;
        if (dividing)
        {
            // Do not re-initialize if already dividing.
            return;
        }

        dividing = true;

        GameObject sourceA = divideSource[A];
        GameObject sourceB = divideSource[B];

        bullets[A] = Instantiate(bulletPrefab, sourceA.transform.position, sourceA.transform.rotation);
        bullets[B] = Instantiate(bulletPrefab, sourceB.transform.position, sourceB.transform.rotation);
    }

    void CreateLine()
    {

    }
}
