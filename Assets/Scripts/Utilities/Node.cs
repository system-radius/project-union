using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    // The parent of this node.
    private Node parent;

    // The coordinate for this node.
    private Vector2 coord;

    // The distance from the starting node.
    public float g { get; }

    // The estimated cost going to the destination.
    private float h;

    public Node(Vector2 coord, float g, float h, Node parent = null)
    {
        this.coord = coord;
        this.g = g;
        this.h = h;
        this.parent = parent;
    }

    public int CompareTo(Node other)
    {
        return this.GetF() > other.GetF() ? 1 : -1;
    }

    public Vector2 GetCoord()
    {
        return coord;
    }

    public float GetF()
    {
        return g + h;
    }

    public Node GetParent()
    {
        return parent;
    }
}
