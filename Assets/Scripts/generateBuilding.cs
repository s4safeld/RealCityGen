﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class generateBuilding : MonoBehaviour
{

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public Material material;
    public Vector3 shapeMesh;
    public int seed;
    public int minCount;
    public int maxCount;
    public float minRange;
    public float maxRange;

    [SerializeField]
    public buildingStep[] steps;

    // Start is called before the first frame update
    void Start()
    {
        //Temporary
        if (seed == 0) {
            seed = Random.Range(-100000, 100000);
        }
        Random.InitState(seed);
        Debug.Log("seed: "+seed);
        //--------

        Mesh randMesh = randomMesh(maxCount,minRange,maxRange);
        randMesh = extrudeMesh(randMesh, new Vector3(0,-1,0));
        //Mesh building = generate();

        GameObject gameObject = new GameObject("randomMesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.GetComponent<MeshFilter>().mesh = randMesh;

        Mesh[] mshs = generate(steps, 8, 10, 10, 10);
        foreach (Mesh mesh in mshs) {
            gameObject = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        GameObject gameObject2 = new GameObject("building", typeof(MeshFilter), typeof(MeshRenderer));

    }

    private void Update()
    {
        Debug.DrawLine(new Vector3(0, -10, 0), new Vector3(0, 10, 0), Color.red);
    }
    bool containsPoint(Vector2[] polyPoints, Vector2 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }
    Mesh randomMesh(int vertCount, float minRange, float maxRange) {


        Vector2[] vertices2D = new Vector2[vertCount];

        float stepSize = 8 / (float)vertCount;

        //Loops around a central point using Sin and Cos functions to generate random vertices in clockwise order
        float steps = 0;
        for (int i = 0; i < vertCount; i++) {
            float x = Mathf.Sin(Random.Range(steps, steps + stepSize));
            float z = Mathf.Cos(Random.Range(steps, steps + stepSize));
            vertices2D[i] = new Vector2(x, z);
            vertices2D[i] *= Random.Range(minRange, maxRange);
            steps += stepSize;
            Debug.Log("Steps: " + steps);
        }

        //computes surface triangles for 2D Mesh
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        //casts 2D triangles to 3D
        Vector3[] vertices = new Vector3[vertCount];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
            Debug.Log("vertices[" + i + "]: " + vertices[i]);
        }

        //computes UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++) {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    Mesh extrudeMesh(Mesh mesh, Vector3 location) {

        int i;
        int j;
        int k;
        int pos;

        if (location.y > 0) {
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }

        int oldTrianglesLength = mesh.triangles.Length;

        //Copy Mesh to location
        Vector3[] vertices = new Vector3[mesh.vertices.Length*2];

        //Vertices----------
        i = 0;
        while (i < mesh.vertices.Length)    //old vertices to new array
        {
            vertices[i] = mesh.vertices[i];
            i++;
        }
        while (i < vertices.Length)         //copy vertices to new location
        {
            vertices[i] = mesh.vertices[i-mesh.vertices.Length] +location;
            i++;
        }
        //-----------------

        //Triangles--------
        int[] triangles = new int[mesh.triangles.Length*2 + (mesh.vertices.Length * 2) * 3];
        
        pos = 0;
        while (pos < oldTrianglesLength)    //Add old Triangles
        {
            triangles[pos] = mesh.triangles[pos++];
        }

        while (pos < oldTrianglesLength*2)  //generate Triangles of the copied vertices
        {
            triangles[pos] = mesh.triangles[pos - mesh.triangles.Length] + vertices.Length/2;
            pos++;
        }
        //-----------------

        //connect the two meshes
            //Explanation and Example:
            {/*This algorithm connects two meshes in the following way:

        The algorithm first loops around the vertices of the bottom mesh clockwise and generates one triangle each time connecting it to the top Mesh
        it then loops around the bottom vertices a second time generating another triangle thus finishing the squares to connect the meshes.

            top Mesh:   bottom Mesh:
            1----2      5----6
            |    |      |    |
            |    |      |    |
            0----3      4----7

        On the top example the triangles would look like this:
        
        1       4   5   1
        2       5   6   2
        3       6   7   3

        4       7   4   0

        5       4   1   0
        6       5   2   1
        7       6   3   2
        
        8       7   0   3
        */
        }
            //---------------------
        i = vertices.Length/2;
        j = vertices.Length/2 + 1;
        k = 1;
        //see example number 1-3
        while (i < (vertices.Length) - 1)
        {
            triangles[pos++] = i++;
            triangles[pos++] = j++;
            triangles[pos++] = k++;
        }
        //see example number 4
        triangles[pos++] = i;
        triangles[pos++] = vertices.Length / 2;
        triangles[pos++] = 0;

        i = vertices.Length / 2;
        j = 1;
        k = 0;
        //see example 5-7
        while (i < (vertices.Length) - 1)
        {
            triangles[pos++] = i++;
            triangles[pos++] = j++;
            triangles[pos++] = k++;
        }
        //see example 8
        triangles[pos++] = i;
        triangles[pos++] = 0;
        triangles[pos] = k;

        //Reverse Triangles to not be rendered upside down
        if (location.y > 0)
        {
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }


        //compute UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        while (i < uvs.Length)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            i++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        return mesh;
    }
    Mesh[] generate(buildingStep[] steps, float minWidth, float maxWidth, float minLength, float maxLength) {
        int totalFloors = 0;
        int totalSteps = steps.Length;
        int floorsGenerated = 0;
        Mesh[] meshes = new Mesh[steps.Length];
        for (int i = 0; i < steps.Length; i++){
            totalFloors += steps[i].floors;
        }
        for (int i = 0; i < steps.Length; i++) {
            if (steps[i].optional && Random.value >= 0.5f) {
                steps[i].generate = false;
                totalSteps--;
            }
            if (steps[i].generate && i == 0)
            {
                meshes[i] = randomMesh(steps[i].vertices, minWidth, Random.Range(minWidth, ((maxWidth / totalSteps) * (i + 1))));
                meshes[i] = extrudeMesh(meshes[i], new Vector3(0,-(floorsGenerated + steps[i].floors)*2,0));
            }
            else {
                if (steps[i].generate) {
                    meshes[i] = randomMesh(steps[i].vertices, (maxWidth / totalSteps) * i, Random.Range(minWidth, ((maxWidth / totalSteps) * (i + 1))));
                    meshes[i] = extrudeMesh(meshes[i], new Vector3(0, -(floorsGenerated + steps[i].floors) * 2, 0));
                }
            }
            for (int j = 0; j < meshes[i].vertices.Length; j++) {
                meshes[i].vertices[j] = new Vector3(meshes[i].vertices[j].x, -floorsGenerated*2, meshes[i].vertices[j].z);
            }
        }

        return meshes;
    }

}

[Serializable]
public class buildingStep{
    public bool optional;
    public bool generate = true;
    public int floors;
    public int vertices;

    public buildingStep(int minFloors, int maxFloors, int minVertices, int maxVertices, bool optional) {
        floors = Random.Range(minFloors, maxFloors);
        vertices = Random.Range(minVertices, maxVertices);
        optional = this.optional;
    }
}

//This class makes it possible to automatically create surface triangles for any sets of 2D vertices
//It is not written by me, but from runevision
//The source can be found here:
//http://wiki.unity3d.com/index.php?title=Triangulator&_ga=2.97540694.871866967.1597588282-744620994.1584369047
public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}