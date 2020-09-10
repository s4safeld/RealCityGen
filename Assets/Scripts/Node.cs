using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public bool roadNorth = false;
    public bool roadSouth = false;
    public bool roadEast = false;
    public bool roadWest = false;
    public GameObject inheritor;

    public Node(Vector3 pos)
    {
        position = pos;
    }
}
