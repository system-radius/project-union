using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Divider : MonoBehaviour
{

    private GridController gridController;

    private const int SIDES = 2;

    // There are only two divide sources.
    private GameObject[] divideSource = new GameObject[SIDES];

    private DivideSource[] sources = new DivideSource[SIDES];

    private List<GameObject> bullets;

    private CapsuleCollider2D capsuleCollider;

    private Animator animator;

    private bool dividing = false;

    private bool lineComplete = false;

    void Start()
    {
        bullets = new List<GameObject>();
        animator = GetComponent<Animator>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        capsuleCollider.enabled = false;

        GameObject gridContainer = GameObject.FindGameObjectWithTag("GridController");
        gridController = gridContainer.GetComponent<GridController>();

        int divideCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Divide Source" && divideCounter < divideSource.Length)
            {
                divideSource[divideCounter] = child.gameObject;
                sources[divideCounter] = child.gameObject.GetComponent<DivideSource>();
                divideCounter++;
            }

            // Check if the two arrays have been filled.
            if (divideCounter >= divideSource.Length)
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

        if (gameObject == null)
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
            DeactivateSources();
        }
    }

    private void CreateLine()
    {

        // Get the bullets from the source.
        foreach (DivideSource source in sources)
        {
            bullets.Add(source.GetBullet());
        }

        gridController.CreateLine(bullets);
        CompleteSources();
        /*
        foreach (GameObject bullet in bullets)
        {
            // Remove parent.
            bullet.transform.parent = null;
        }
        /**/
    }

    private bool HasAllHit()
    {
        bool result = true;

        for (int i = 0; i < SIDES; i++)
        {
            // Check the bullets from the source.
            if (sources[i] == null)
            {
                // Return false right away if a script is not available.
                return false;
            }

            result = result && sources[i].HasHit();
        }

        return result;
    }

    /**
     * Stop the dividing state.
     */
    public void StopDivide()
    {
        dividing = false;
        capsuleCollider.enabled = false;

        ResetAnimator();
        DeactivateSources();

        lineComplete = false;
    }

    private void DeactivateSources()
    {
        Debug.Log("Deactivated sources!");
        for (int i = 0; i < SIDES; i++)
        {
            sources[i].Deactivate();
        }
    }

    private void CompleteSources()
    {
        Debug.Log("Completed sources!");
        for (int i = 0; i < SIDES; i++)
        {
            sources[i].Complete();
        }
    }

    /**
     * Initialize the dividing state.
     */
    public void Divide()
    {
        if (dividing)
        {
            // Do not re-initialize if already dividing.
            return;
        }

        bullets.Clear();
        dividing = true;
        capsuleCollider.enabled = true;

        ResetAnimator();

        for (int i = 0; i < SIDES; i++)
        {
            // Activate the script for the divide source.
            sources[i].Activate();
        }
    }

    public void ResetAnimator()
    {
        animator.SetBool("Dividing", dividing);
    }

    public bool IsDividing()
    {
        return dividing;
    }

    public int GetBulletCount()
    {
        return bullets.Count;
    }
}
