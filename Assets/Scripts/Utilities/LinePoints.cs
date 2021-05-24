using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class designed to have two coordinates.
 * This will represent the points of a line.
 * The values of the points must be on world units.
 */
public class LinePoints
{
    private Vector2 pointA;

    private Vector2 pointB;

    public LinePoints(Vector2 pointA, Vector2 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
    }

    public Vector2 GetPointA()
    {
        return pointA;
    }

    public Vector2 GetPointB()
    {
        return pointB;
    }
}
