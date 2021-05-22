using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class is supposed to handle all of the inputs to control
 * the divider. This class should only send commands to the divider
 * and the divider will act on those commands. The controller
 * will not act directly on behalf of the divider. This means
 * that all checks are to be accomplished in the divider class itself.
 */
public class TouchController : MonoBehaviour
{

    // The amount of time to pass before the touch is considered as "hold."
    private const float HOLD_CONST = 0.5f;

    // The angle to adjust the rotation of the divider.
    private const float ROTATION_CONST = 90f;

    // The current amount of time waiting to pass.
    private float touchTime = 0f;

    // The state of the divider, whether it is horizontal (true) or vertical (false).
    private bool horizontal;

    // The current touch object.
    private Touch touch;

    // The divider class instance. Necessary to call for commands on the divider itself.
    private Divider divideController;

    void Start()
    {
        // The divider game object needs to have the Divider class as component.
        divideController = GetComponent<Divider>();
    }

    // Update is called once per frame
    void Update()
    {

        // If a touch is registered...
        if (Input.touchCount > 0)
        {
            ProcessTouch(Input.GetTouch(0));
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            touch.position = Input.mousePosition;
            MoveToTouch();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (divideController.IsAllowedDivide())
            {
                touch.phase = TouchPhase.Stationary;
                divideController.Divide();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            divideController.StopDivide();
        }


    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            touchTime += Input.GetTouch(0).deltaTime;
        }
    }

    private void ProcessTouch(Touch tempTouch)
    {
        // Just take the first touch instance.
        touch = tempTouch;

        if (CheckHold())
        {
            //Debug.Log("(" + transform.position.x + ", " + transform.position.y + ") = " + gridController.GetGridValue(transform.position.x, transform.position.y));
            divideController.Divide();
        }
        else if (touch.tapCount == 2)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Rotate();
                touch.phase = TouchPhase.Ended;
            }
        }
        else
        {
            if (touch.phase == TouchPhase.Began)
            {
                MoveToTouch();
            }
        }

        if (touch.phase.Equals(TouchPhase.Ended))
        {
            divideController.StopDivide();
            touchTime = 0;
        }
    }

    void MoveToTouch()
    {
        // Convert the touch position from screen to world coordinates.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        divideController.Move(touchPos);
    }

    bool CheckHold()
    {
        return touchTime >= HOLD_CONST && divideController.IsAllowedDivide();
    }

    void Rotate()
    {
        horizontal = !horizontal;

        float rotation = horizontal ? ROTATION_CONST : -ROTATION_CONST;
        transform.Rotate(0, 0, rotation);

        //Debug.Log("Rotation: " + transform.rotation.z);
    }
}
