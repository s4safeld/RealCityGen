using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
    [SerializeField]
    public buildingStep[] steps;
    public Material material;
    public GridGenerator gridGenerator;
    // Start is called before the first frame update
    void Start()
    {
        gridGenerator = GetComponentInParent<GridGenerator>();
        Debug.Log(gridGenerator);
    }

    // Update is called once per frame
    public GameObject generate(int localSeed)
    {
        Random.InitState(localSeed);
        int height = 0;
        int heightGenerated = 0;

        foreach (buildingStep step in steps)
        {
            step.initialise();
            height += (step.getFloors()*2);
        }
        GameObject[] floors = new GameObject[steps.Length];
        for (int i = 0; i<steps.Length; i++)
        {
            if (steps[i].isPolygon)
            {
                Mesh polygon = SimplePolygon.polygon(steps[i].getRadius(), steps[i].getVertices());
                Mesh mesh;
                mesh = extrudeMesh(polygon, new Vector3(0, -height, 0));
                mesh = moveMesh(mesh, new Vector3(
                    Random.Range(0, gridGenerator.cellSize/2 - steps[i].getRadius()),
                    -heightGenerated,
                    Random.Range(0, gridGenerator.cellSize/2 - steps[i].getRadius())
                ));
                heightGenerated += steps[i].getFloors() * 2;
                floors[i] = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
                floors[i].GetComponent<MeshFilter>().mesh = mesh;
                floors[i].transform.Rotate(new Vector3(0, steps[i].getRotation(),0 ));
            }
            else
            {
                Mesh polygon = SimplePolygon.rectangle(steps[i].getWidth(), steps[i].getLength());
                Mesh mesh;
                mesh = extrudeMesh(polygon, new Vector3(0, -height, 0));
                mesh = moveMesh(mesh, new Vector3(
                    Random.Range(0, gridGenerator.cellSize/2 - steps[i].getLength()),
                    -heightGenerated,
                    Random.Range(0, gridGenerator.cellSize/2 - steps[i].getWidth())
                ));
                heightGenerated += steps[i].getFloors() * 2;
                floors[i] = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
                floors[i].GetComponent<MeshFilter>().mesh = mesh;
                floors[i].transform.Rotate(new Vector3(0, steps[i].getRotation(), 0));
            }
        }

        GameObject building = new GameObject("building", typeof(MeshFilter), typeof(MeshRenderer));
        CombMeshes(floors, building);
        building.transform.position = new Vector3(0, building.transform.position.y + height, 0);
        building.GetComponent<Renderer>().material = material;
        building.AddComponent<MeshCollider>();

        return building;
    }
    Mesh extrudeMesh(Mesh mesh, Vector3 location)
    {

        int i;
        int j;
        int k;
        int pos;

        if (location.y > 0)
        {
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }

        int oldTrianglesLength = mesh.triangles.Length;

        //Copy Mesh to location
        Vector3[] vertices = new Vector3[mesh.vertices.Length * 2];

        //Vertices----------
        i = 0;
        while (i < mesh.vertices.Length)    //old vertices to new array
        {
            vertices[i] = mesh.vertices[i];
            i++;
        }
        while (i < vertices.Length)         //copy vertices to new location
        {
            vertices[i] = mesh.vertices[i - mesh.vertices.Length] + location;
            i++;
        }
        //-----------------

        //Triangles--------
        int[] triangles = new int[mesh.triangles.Length * 2 + (mesh.vertices.Length * 2) * 3];

        pos = 0;
        while (pos < oldTrianglesLength)    //Add old Triangles
        {
            triangles[pos] = mesh.triangles[pos++];
        }

        while (pos < oldTrianglesLength * 2)  //generate Triangles of the copied vertices
        {
            triangles[pos] = mesh.triangles[pos - mesh.triangles.Length] + vertices.Length / 2;
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
        i = vertices.Length / 2;
        j = vertices.Length / 2 + 1;
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
    Mesh moveMesh(Mesh mesh, Vector3 location)
    {

        Vector3[] vertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(  mesh.vertices[i].x + location.x, 
                                        mesh.vertices[i].y + location.y, 
                                        mesh.vertices[i].z + location.z);
            //Debug.Log("vertices[" + i + "]: " + vertices[i]);
        }

        mesh.vertices = vertices;
        return mesh;
    }
    void CombMeshes(GameObject[] floors, GameObject building)
    {
        MeshFilter[] meshFilters = new MeshFilter[floors.Length];
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < floors.Length; i++)
        {
            meshFilters[i] = floors[i].GetComponent<MeshFilter>();
        }
        Debug.Log(meshFilters.Length);
        for (int i = 0; i < meshFilters.Length; i++)
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
public class buildingStep
{
    public bool optional;
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
        floors   = Random.Range(minFloors,   maxFloors);
        vertices = Random.Range(minVertices, maxVertices);
        radius   = Random.Range(minRadius,   maxRadius);
        width    = Random.Range(minWidth,    maxWidth);
        length   = Random.Range(minLength,   maxLength);
        rotation = Random.Range(minRotation, maxRotation);
    }
}
