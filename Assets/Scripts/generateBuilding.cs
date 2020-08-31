using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class generateBuilding : MonoBehaviour
{

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public Vector3 shapeMesh;
    public int minWidth;
    public int maxWidth;
    public float minLength;
    public float maxLength;
    public Material material;

    private int seed;

    [SerializeField]
    public buildingStep[] steps;

    // Start is called before the first frame update
    private void Start()
    {
        //If you see code in Start: It should not be here, delete it, now!!
        seed = Random.Range(0,10000000);
        Random.InitState(seed);
        Debug.Log(seed);
        fire(new Vector3(0,0,0));
    }

    public GameObject fire(Vector3 position)
    {
        //Random.InitState(globalInformation.seed + (int)(position.x - position.z));
        foreach (buildingStep step in steps)
        {
            step.initialise();
        }
        GameObject[] floors = generate(steps, minWidth, maxWidth, minLength, maxLength);
        GameObject building = new GameObject("building", typeof(MeshFilter), typeof(MeshRenderer));

        CombMeshes(floors,building);
        building.transform.position = position;
        building.GetComponent<Renderer>().material = material;
        return building;
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

        /*
        float offset = 0;

        //Alternative:
        int sumOfAngles = (vertCount - 2) * 180;
        float avgAngle = sumOfAngles / vertCount;

        vertices2D[0] = new Vector2(0,-1);
        vertices2D[1] = new Vector2(1*Mathf.Tan(avgAngle), 1);

        for(int i = 1; i < vertCount; i++)
        {
            vertices2D[i] = vertices2D[i-1] - vertices2D[i-1]            

        }

        //---------------
        */

        //Loops around a central point using Sin and Cos functions to generate random vertices in clockwise order
        float steps = 0;
        for (int i = 0; i < vertCount; i++) {
            float x = Mathf.Sin(Random.Range(steps, steps + stepSize));
            Debug.Log(i+".x = " + x);
            float z = Mathf.Cos(Random.Range(steps, steps + stepSize));
            Debug.Log(i + ".z = " + z);
            vertices2D[i] = new Vector2(x, z);
            //vertices2D[i] *= Random.Range(minRange, maxRange);

            if (i > 1)
            {
                Debug.Log("hier!");
                if(Vector3.Angle(vertices2D[i-2] - vertices2D[i-1], vertices2D[i] - vertices2D[i-1]) < 45)
                {
                    i--;
                }
                else
                {
                    steps += stepSize;
                }
            }
            else
            {
                steps += stepSize;
            }
        }

        //computes surface triangles for 2D Mesh
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        //casts 2D triangles to 3D
        Vector3[] vertices = new Vector3[vertCount];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
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
    GameObject[] generate(buildingStep[] steps, float minWidth, float maxWidth, float minLength, float maxLength) {
        int totalFloors = 0;
        int totalSteps = steps.Length;
        int floorsGenerated = 0;
        GameObject[] floors = new GameObject[steps.Length];
        Mesh[] meshes = new Mesh[steps.Length];
        for (int i = 0; i < steps.Length; i++){
            totalFloors += steps[i].getFloors();
        }
        for (int i = 0; i < steps.Length; i++) {
            if (steps[i].optional && Random.value >= 0.5f) {
                steps[i].generate = false;
                totalSteps--;
            }
            if (steps[i].generate && i == 0)
            {
                meshes[i] = randomMesh(steps[i].getVertices(), minWidth, Random.Range(minWidth, ((maxWidth / totalSteps) * (i + 1))));
                meshes[i] = extrudeMesh(meshes[i], new Vector3(0,-(totalFloors-floorsGenerated)*2,0));
            }
            else {
                if (steps[i].generate) {
                    meshes[i] = randomMesh(steps[i].getVertices(), (maxWidth / totalSteps) * i, Random.Range(minWidth, ((maxWidth / totalSteps) * (i + 1))));
                    meshes[i] = extrudeMesh(meshes[i], new Vector3(0, -(totalFloors - floorsGenerated) * 2, 0));
                }
            }
            if (steps[i].generate)
            {
                meshes[i] = moveMesh(meshes[i], new Vector3(0, (totalFloors - floorsGenerated) * 2, 0));//.vertices[j] = new Vector3(meshes[i].vertices[j].x, -floorsGenerated*2, meshes[i].vertices[j].z);
                floors[i] = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
                floors[i].GetComponent<MeshFilter>().mesh = meshes[i];
                floorsGenerated += steps[i].getFloors();
            }
            else
            {
                floors[i] = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
                floors[i].GetComponent<MeshFilter>().mesh = new Mesh();
            }
            //Debugging!!
            GameObject temp;
            for (int j = 0; j < meshes[i].vertices.Length; j++)
            {
                temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                temp.name = "meshes["+i+"].vertices["+j+"]";
                temp.transform.position = meshes[i].vertices[j];
                temp.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            //--------------

        }


        return floors;
    }
    Mesh moveMesh(Mesh mesh, Vector3 location) {

        Vector3[] vertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(mesh.vertices[i].x + location.x, mesh.vertices[i].y + location.y, mesh.vertices[i].z + location.z);
            Debug.Log("vertices[" + i + "]: " + vertices[i]);
        }

        mesh.vertices = vertices;
        return mesh;
    }
    void CombMeshes(GameObject[] floors, GameObject building) {
        MeshFilter[] meshFilters = new MeshFilter[floors.Length];
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < floors.Length; i++)
        {
            meshFilters[i] = floors[i].GetComponent<MeshFilter>();       
        }
        Debug.Log(meshFilters.Length);
        for (int i = 0;  i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
        }
        building.GetComponent<MeshFilter>().mesh = new Mesh();
        building.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }
}

[Serializable]
public class buildingStep{
    public bool optional;
    public bool generate = true;
    public int minFloors;
    public int maxFloors;
    public int minVertices;
    public int maxVertices;
    private int vertices;
    private int floors;

    public int getVertices() {
        return vertices;
    }
    public int getFloors() {
        return floors;
    }

    public void initialise() {
        floors = Random.Range(minFloors, maxFloors);
        vertices = Random.Range(minVertices, maxVertices);
    }
}

//This class makes it possible to automatically create surface triangles for any sets of 2D vertices
//It is not written by me, but from runevision
//The source can be found here:
//http://wiki.unity3d.com/index.php?title=Triangulator&_ga=2.97540694.871866967.1597588282-744620994.1584369047
/*public class Triangulator
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
*/