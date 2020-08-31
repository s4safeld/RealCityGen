using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimplePolygon : MonoBehaviour
{
    public static Mesh polygon(float size, int corners)
    {
        if(corners == 4)
        {
            Debug.LogWarning("Simple2DPolygon: This will just create a simple square, are you sure you dont want to use rectangle()?");
        }
        if(corners < 3)
        {
            Debug.LogError("SImple2DPolygon: The number of corners is lower than 3, which will not create a real Polygon");
        }
        float angle = 360 / corners;
        Vector2[] vertices2D = new Vector2[corners];
        vertices2D[0] = new Vector2(size, 0);
        for (int i = 1; i < vertices2D.Length; i++)
        {
            vertices2D[i] = Quaternion.Euler(0, 0, angle) * vertices2D[i - 1];
        }

        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        Vector3[] vertices = new Vector3[corners];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh rectangle(float sizeX, float sizeZ)
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-sizeX / 2, 0, -sizeZ / 2);
        vertices[1] = new Vector3(-sizeX / 2, 0,  sizeZ / 2);
        vertices[2] = new Vector3( sizeX / 2, 0,  sizeZ / 2);
        vertices[3] = new Vector3( sizeX / 2, 0, -sizeZ / 2);

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3; 

        triangles[3] = 1; 
        triangles[4] = 2;
        triangles[5] = 3;

        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
