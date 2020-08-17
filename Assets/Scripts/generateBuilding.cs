using System;
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
    public int edges;
    public Vector3 shapeMesh;
    public int seed;
    public int minCount;
    public int maxCount;
    public float minRange;
    public float maxRange;

    // Start is called before the first frame update
    void Start()
    {
        /*float angle;
        float oldAngle = 0;


        Vector2 bounds = new Vector2(10,10);

        Vector3 startPos = new Vector3(Random.Range(0, bounds.x), 0, Random.Range(0, bounds.y));
        float distance = Random.Range(startPos.x, bounds.y);

        Vector3[] vertices = new Vector3[edges];
        int[] triangles = new int[edges*edges*edges];

        vertices[0] = new Vector3(startPos.x + distance, startPos.y, startPos.z);
        angle = Random.Range(90, 180);
        vertices[1] = vertices[0] * (angle + oldAngle);
        
        check this later:
        http://wiki.unity3d.com/index.php?title=Triangulator&_ga=2.97540694.871866967.1597588282-744620994.1584369047
         */


        //Temporary
        if (seed == 0) {
            seed = Random.Range(-100000, 100000);
        }
        Random.InitState(seed);
        Debug.Log("seed: "+seed);
        //--------

        Mesh randMesh = randomMesh(minCount,maxCount,minRange,maxRange);
        //Mesh mesh = compute2DMesh();
        //Mesh mesh3 = extrudeMesh(mesh, shapeMesh);
        

        //GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        //gameObject.GetComponent<MeshFilter>().mesh = mesh;

        //GameObject gameObject3 = new GameObject("Mesh3", typeof(MeshFilter), typeof(MeshRenderer));
        //gameObject3.GetComponent<MeshFilter>().mesh = mesh3;

        GameObject gameObject4 = new GameObject("randomMesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject4.GetComponent<MeshFilter>().mesh = randMesh;


        GameObject gameObject5 = new GameObject("randomMesh2", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject5.GetComponent<MeshFilter>().mesh = randMesh;



        Vector3 pos;

        float x = 0;

        /*GameObject temp;
        temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "Pi";
        temp.transform.position = new Vector3(Mathf.Sin(3.141f), 0, -1);

        temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temp.name = "Cos";
        temp.transform.position = new Vector3(Mathf.Cos(3.141f), 0, -2);
        for (float i = 0; i < 3.141f*2; i = i + 0.1f) {
            temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.name = "Sphere" + i;
            temp.transform.position = new Vector3(Mathf.Sin(i),0,Mathf.Cos(i));
        }*/
    }

    private void Update()
    {
        Debug.DrawLine(new Vector3(0, -10, 0), new Vector3(0, 10, 0), Color.red);
    }
    Mesh randomMesh(int minCount, int maxCount, float minRange, float maxRange) {

        maxCount = Mathf.RoundToInt(Random.Range(minCount, maxCount));

        Vector2[] vertices2D = new Vector2[maxCount];

        float stepSize = 8 / (float)maxCount;

        float steps = 0;
        for (int i = 0; i < maxCount; i++) {
            float x = Mathf.Sin(Random.Range(steps, steps + stepSize));
            float z = Mathf.Cos(Random.Range(steps, steps + stepSize));
            vertices2D[i] = new Vector2(x, z);
            vertices2D[i] *= Random.Range(minRange, maxRange);
            steps += stepSize;
            Debug.Log("Steps: " + steps);
        }

        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();


        Vector3[] vertices = new Vector3[maxCount];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
            Debug.Log("vertices[" + i + "]: " + vertices[i]);
        }

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


    Mesh compute2DMesh() {
        Vector3[] vertices = new Vector3[5];
        int[] triangles = new int[9];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0, 1);
        vertices[2] = new Vector3(0.5f, 0, 1.5f);
        vertices[3] = new Vector3(1, 0, 1);
        vertices[4] = new Vector3(1, 0, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;

        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 4;

        triangles[6] = 1;
        triangles[7] = 2;
        triangles[8] = 3;


        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

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
        //
        while (pos < oldTrianglesLength*2)  //generate Triangles of the copied vertices
        {
            triangles[pos] = mesh.triangles[pos - mesh.triangles.Length] + vertices.Length/2;
            pos++;
        }
        //-----------------

        //connect the two meshes
        /*This algorithm connects two meshes in the following way:

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

        //Mesh newMesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        if (location.y > 0) {
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }
        return mesh;
    }

}

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
