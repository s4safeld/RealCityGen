using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class generateBuilding : MonoBehaviour
{

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public Material material;
    public int edges;
    public Vector3 shapeMesh;

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



        Mesh mesh = compute2DMesh();
        Mesh mesh3 = extrudeMesh(mesh, shapeMesh);
        

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        GameObject gameObject3 = new GameObject("Mesh3", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject3.GetComponent<MeshFilter>().mesh = mesh3;

        Vector3 pos;

        float x = 0;

        GameObject temp;
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
        }
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
