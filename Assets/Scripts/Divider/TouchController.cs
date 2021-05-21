using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{

    // The amount of time to pass before the touch is considered as "hold."
    private const float HOLD_CONST = 0.5f;

    private const float ROTATION_CONST = 90f;

    // The current amount of time waiting to pass.
    private float touchTime = 0f;

    private bool horizontal;

    private Touch touch;

    private GridController gridController;

    private Divider divideController;

    void Start()
    {
        GameObject gridContainer = GameObject.FindGameObjectWithTag("GridController");
        gridController = gridContainer.GetComponent<GridController>();
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
            if (DividerUtils.GetGridValue(
                gridController.GetField(), transform.position.x, transform.position.y) == GridValue.SPACE)
            {
                touch.phase = TouchPhase.Stationary;
                divideController.Divide();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (divideController.IsDividing())
            {
                divideController.StopDivide();
            }
        }


    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            // Just take the first touch instance.
            touch = Input.GetTouch(0);
            touchTime += touch.deltaTime;
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
            if (divideController.IsDividing())
            {
                divideController.StopDivide();
            }
            touchTime = 0;
        }
    }

    void MoveToTouch()
    {
        // Convert the touch position from screen to world coordinates.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

        // Do not consider changes along the z-axis.
        touchPos.z = 0;

        if (DividerUtils.GetGridValue(gridController.GetField(), touchPos.x, touchPos.y) != GridValue.SPACE)
        {
            // Return prematurely.
            return;
        }

        Vector2 exactCoords = DividerUtils.UnitToExactNormalized(touchPos.x, touchPos.y);
        touchPos.x = exactCoords.x;
        touchPos.y = exactCoords.y;

        Vector2 gridCoords = DividerUtils.UnitToGridPoint(touchPos.x, touchPos.y);

        //Debug.Log("(" + touchPos.x + ", " + touchPos.y + ") = [" + gridCoords.x + ", " + gridCoords.y + "]");
        // Set the transform of this object to the position of the touch.
        transform.position = touchPos;
    }

    bool CheckHold()
    {

        return touchTime >= HOLD_CONST && DividerUtils.GetGridValue(
            gridController.GetField(), transform.position.x, transform.position.y
            ) == GridValue.SPACE;
    }

    void Rotate()
    {
        horizontal = !horizontal;

        float rotation = horizontal ? ROTATION_CONST : -ROTATION_CONST;
        transform.Rotate(0, 0, rotation);

        //Debug.Log("Rotation: " + transform.rotation.z);
    }
}
