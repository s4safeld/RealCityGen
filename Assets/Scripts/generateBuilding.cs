using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateBuilding : MonoBehaviour
{

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = compute2DMesh();
        Mesh mesh2 = compute2ndMesh(mesh);
        Mesh mesh3 = connectMeshes(mesh, mesh2);
        

        GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        GameObject gameObject2 = new GameObject("Mesh2", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject2.GetComponent<MeshFilter>().mesh = mesh2;

        GameObject gameObject3 = new GameObject("Mesh3", typeof(MeshFilter), typeof(MeshRenderer));
        gameObject3.GetComponent<MeshFilter>().mesh = mesh3;

    }
    Mesh compute2DMesh() {
        Vector3[] vertices = new Vector3[5];
        int[] triangles = new int[9];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 0, 1);
        vertices[3] = new Vector3(1, 0, 1);
        vertices[4] = new Vector3(0.5f, 0, 1.5f);

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 3;

        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 1;

        triangles[6] = 2;
        triangles[7] = 4;
        triangles[8] = 3;


        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    Mesh compute2ndMesh(Mesh input) {

        Vector3[] vertices = input.vertices;
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(vertices[i].x, vertices[i].y - 1, vertices[i].z);
        }
        int[] triangles = input.triangles;

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    Mesh connectMeshes(Mesh mesh1, Mesh mesh2) {

        Vector3[] vertices = new Vector3[mesh1.vertices.Length + mesh2.vertices.Length];
        for (int i = 0; i < mesh1.vertices.Length; i++) {
            vertices[i] = mesh1.vertices[i];
        }
        for (int i = mesh1.vertices.Length; i < mesh2.vertices.Length; i++)
        {
            vertices[i] = mesh2.vertices[i- mesh1.vertices.Length];
        }

        int[] triangles = new int[mesh1.triangles.Length + mesh2.triangles.Length];
        for (int i = 0; i < mesh1.triangles.Length; i++)
        {
            triangles[i] = mesh1.triangles[i];
        }
        for (int i = mesh1.triangles.Length; i < mesh2.triangles.Length; i++)
        {
            triangles[i] = mesh2.triangles[i - mesh1.triangles.Length];
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

}
