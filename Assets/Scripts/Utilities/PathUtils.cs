using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtils
{
    private static PathUtils instance;

    // The open list, will contain the frontier nodes.
    private readonly List<Node> openList = new List<Node>();

    // The closed list, will contain the visited nodes.
    private readonly List<Node> closedList = new List<Node>();

    private Vector2 target;

    private GridValue[,] maze;

    private PathUtils()
    {

    }

    public static void CreateInstance()
    {
        if (instance != null)
        {
            return;
        }

        instance = new PathUtils();
    }

    public static List<Vector2> FindShortestPath(GridValue[,] field, Vector2 pointA, Vector2 pointB)
    {
        return instance.FindPathInternal(field, pointA, pointB);
    }

    private List<Vector2> FindPathInternal(GridValue[,] field, Vector2 pointA, Vector2 pointB)
    {
        List<Vector2> result = new List<Vector2>();
        maze = field;

        // Start by clearing the lists.
        openList.Clear();
        closedList.Clear();

        // Create the current node.
        Node now = new Node(pointA, 0, 0);
        target = pointB;

        for (openList.Add(now); openList.Count > 0;)
        {
            now = openList[0];
            openList.RemoveAt(0);

            if (now.GetCoord() == pointB)
            {
                return FixPath(now);
            }

            closedList.Add(now);
            AddToOpenList(now);
        }

        return result;
    }

    private List<Vector2> FixPath(Node node)
    {
        List<Vector2> result = new List<Vector2>();

        result.Add(node.GetCoord());
        for (Node parent = node.GetParent(); parent != null; parent = parent.GetParent())
        {
            if (parent.GetParent() == null)
            {
                break;
            }

            // Always insert at the front.
            result.Insert(0, parent.GetCoord());
        }

        return result;
    }

    private void AddToOpenList(Node parent)
    {
        float g = parent.g;
        List<Vector2> frontier = DividerUtils.FindNextFrontier(maze, parent.GetCoord());

        foreach (Vector2 vector in frontier)
        {
            Node child = new Node(vector, g + 1, Mathf.Abs((vector - target).magnitude), parent);
            if (!(FindInList(openList, child) || FindInList(closedList, child)))
            {
                openList.Add(child);
            }

        }

        openList.Sort();
    }

    private bool FindInList(List<Node> list, Node node)
    {
        Vector2 coord = node.GetCoord();

        foreach (Node inList in list)
        {
            if (inList.GetCoord() == coord)
            {
                return true;
            }
        }

        return false;
    }
}
