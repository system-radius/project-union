using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchContoller : MonoBehaviour
{

    // The amount of time to pass before the touch is considered as "hold."
    private const float HOLD_CONST = 0.75f;

    private const float ROTATION_CONST = 90f;

    // The current amount of time waiting to pass.
    private float touchTime = 0f;

    private bool horizontal;

    private Touch touch;

    public GameObject gridContainer;

    private GridController gridController;

    private DivideController divideController;

    void Start()
    {
        gridController = gridContainer.GetComponent<GridController>();
        divideController = GetComponent<DivideController>();
    }

    // Update is called once per frame
    void Update()
    {

        // If a touch is registered...
        if (Input.touchCount > 0)
        {
            // Just take the first touch instance.
            touch = Input.GetTouch(0);
            touchTime += touch.deltaTime;

            if (CheckHold())
            {
                divideController.Divide(touch);
            }
            else if (touch.tapCount == 2)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Rotate();
                }
            }
            else
            {
                MoveToTouch();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                // The touch has ended, reset the touch time.
                touchTime = 0;
            }
        }

    }

    void MoveToTouch()
    {
        // Convert the touch position from screen to world coordinates.
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

        // Do not consider changes along the z-axis.
        touchPos.z = 0;

        if (gridController.GetGridValue(touchPos.x, touchPos.y) != 0)
        {
            // Return prematurely.
            return;
        }

        Vector2 exactCoords = gridController.UnitToExactNormalized(touchPos.x, touchPos.y);
        touchPos.x = exactCoords.x;
        touchPos.y = exactCoords.y;

        Vector2 gridCoords = gridController.UnitToGridPoint(touchPos.x, touchPos.y);

        //Debug.Log("(" + touchPos.x + ", " + touchPos.y + ") = [" + gridCoords.x + ", " + gridCoords.y + "]");
        // Set the transform of this object to the position of the touch.
        transform.position = touchPos;
    }

    bool CheckHold()
    {
        return touchTime >= HOLD_CONST;
    }

    void Rotate()
    {
        horizontal = !horizontal;

        float rotation = horizontal ? ROTATION_CONST : -ROTATION_CONST;
        transform.Rotate(0, 0, rotation);

        //Debug.Log("Rotation: " + transform.rotation.z);
    }
}
