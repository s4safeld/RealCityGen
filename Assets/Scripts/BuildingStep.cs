using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class buildingStep
{
    public static bool generate = true;

    //Floors
    public int minFloors;
    public int maxFloors;
    private int floors;
    //--------

    //Rotation Value
    public float minRotation;
    public float maxRotation;
    private float rotation;
    //-------

    public bool isPolygon;

    //Polygon
    public int minVertices;
    public int maxVertices;
    public float minRadius;
    public float maxRadius;
    private int vertices;
    private float radius;
    //-------

    //Rectangle
    public float minWidth;
    public float maxWidth;
    public float minLength;
    public float maxLength;
    private float width;
    private float length;
    //--------


    public int getFloors()
    {
        return floors;
    }
    public float getRadius()
    {
        return radius;
    }
    public int getVertices()
    {
        return vertices;
    }
    public float getWidth()
    {
        return width;
    }
    public float getLength()
    {
        return length;
    }
    public float getRotation()
    {
        return rotation;
    }


    public void initialise()
    {
        floors = Random.Range(minFloors, maxFloors);
        vertices = Random.Range(minVertices, maxVertices);
        radius = Random.Range(minRadius, maxRadius);
        width = Random.Range(minWidth, maxWidth);
        length = Random.Range(minLength, maxLength);
        rotation = Random.Range(minRotation, maxRotation);
    }
}