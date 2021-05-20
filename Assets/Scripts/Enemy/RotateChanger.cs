using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateChanger : MonoBehaviour
{

    // The rate of change per step.
    public float rate = 1f;

    // The number of steps for changing.
    public int steps = 5;

    // The amount of time dedicated for spinning.
    public float intervalSpin = 0.5f;

    // The amount of time dedicated for flying.
    public float intervalRest = 3f;

    // Whether to go back to the starting rate.
    public bool reversible = true;

    // Needs the spinner component.
    private Spinner spinner;

    private int currentStep;

    private float currentInterval;

    private float currentTime;

    private bool checkReset;

    private bool spinning;

    // Start is called before the first frame update
    void Start()
    {
        spinner = GetComponent<Spinner>();
        spinning = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (checkReset)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= currentInterval)
            {
                ReverseAngle();
            }
            // Avoid further processing.
            return;
        }

        if (currentStep < steps)
        {
            // While there are steps remaining, increment the current angle by the rate.
            spinner.angle += rate;
            currentStep++;
        }

        if (currentStep >= steps && reversible)
        {
            checkReset = true;
        }
    }

    private void ReverseAngle()
    {
        // Reset the step counter.
        currentStep = 0;

        // Reverse the increment.
        rate *= -1;

        checkReset = false;
        currentTime = 0;

        spinning = !spinning;

        currentInterval = spinning ? intervalSpin : intervalRest;
    }
}
