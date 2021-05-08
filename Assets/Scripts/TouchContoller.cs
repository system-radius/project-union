using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchContoller : MonoBehaviour
{

    // The amount of time to pass before the touch is considered as "hold."
    private const float HOLD_CONST = 0.75f;

    // The current amount of time waiting to pass.
    private float touchTime = 0f;

    private bool horizontal = false;

    private Touch touch;

    public GameObject gridContainer;

    private GridController gridController;

    void Start()
    {
        gridController = gridContainer.GetComponent<GridController>();
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
                Divide();
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

        Vector2 exactCoords = gridController.UnitToExact(touchPos.x, touchPos.y);
        touchPos.x = exactCoords.x;
        touchPos.y = exactCoords.y;

        Debug.Log(touchPos.x + ", " + touchPos.y);
        // Set the transform of this object to the position of the touch.
        transform.position = touchPos;
    }

    bool CheckHold()
    {
        return touchTime >= HOLD_CONST;
    }

    /*
    bool CheckDoubleTap(Touch touch)
    {

        bool result = false;

        if (touch.phase == TouchPhase.Began)
        {
            float deltaTime = touch.deltaTime;
            float deltaPos = touch.deltaPosition.magnitude;

            Debug.Log("DeltaTime = " + deltaTime + ", delta pos: " + deltaPos);

            result = deltaTime > 0 && deltaTime < doubleTapConst && deltaPos < tapVarianceConst;
        }

        return result;
    }/**/

    void Divide()
    {
        Vector3 touchPos = transform.position;
        Debug.Log("Hold on: " + touchPos.x + ", " + touchPos.y);
    }

    void Rotate()
    {
        horizontal = !horizontal;
        transform.Rotate(0, 0, 90);
    }
}
