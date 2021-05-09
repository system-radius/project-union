using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideController : MonoBehaviour
{

    public GameObject bulletPrefab;

    private GridController gridController;

    private const int SIDES = 2;

    // There are only two divide sources.
    private GameObject[] divideSource = new GameObject[SIDES];

    private GameObject[] fillDetect = new GameObject[SIDES];

    private GameObject[] bullets = new GameObject[SIDES];

    private DivideSource[] sources = new DivideSource[SIDES];

    private Bullet[] bulletScript = new Bullet[SIDES];

    private Touch touch;

    private bool dividing = false;

    private bool lineComplete = false;

    void Start()
    {

        GameObject gridContainer = GameObject.FindGameObjectWithTag("GridController");
        gridController = gridContainer.GetComponent<GridController>();

        int divideCounter = 0;
        int fillCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Divide Source" && divideCounter < divideSource.Length)
            {
                divideSource[divideCounter] = child.gameObject;
                sources[divideCounter] = child.gameObject.GetComponent<DivideSource>();
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
        if (HasAllHit() && !lineComplete)
        {
            // While the division is in effect, both bullets becoming null
            // means that both have hit a collider.
            Debug.Log("Line created!");
            lineComplete = true;
            CreateLine();
        }
    }

    private void CreateLine()
    {
        gridController.CreateLine(bullets);
    }

    private bool HasAllHit()
    {
        bool result = true;

        for (int i = 0; i < SIDES; i++)
        {
            if (bulletScript[i] == null)
            {
                // Return false right away if a script is not available.
                return false;
            }

            result = result && bulletScript[i].HasHit();
        }

        return result;
    }

    /**
     * Stop the dividing state.
     */
    public void StopDivide()
    {
        lineComplete = false;
        dividing = false;

        for (int i = 0; i < SIDES; i++)
        {
            Destroy(bullets[i]);

            sources[i].Deactivate();
        }
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

        for (int i = 0; i < SIDES; i++)
        {
            // Get the source.
            GameObject divideSrc = divideSource[i];

            // Generate a bullet.
            bullets[i] = Instantiate(bulletPrefab, divideSrc.transform.position, divideSrc.transform.rotation);

            // Cache the bullet's script.
            bulletScript[i] = bullets[i].GetComponent<Bullet>();

            // Activate the script for the divide source.
            sources[i].Activate();
        }
    }
}
