using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class globalInformation : MonoBehaviour
{
    public int setViewDistance;
    public GameObject setCell;
    public GameObject setCellCollider;
    public int setChunkIterations;
    public GameObject setTerrain;

    public static int viewDistance;
    public static GameObject cell;
    public static GameObject cellCollider;
    public static int chunkIterations;
    public static float chunkSize;
    public static float edgelength;
    public static GameObject Terrain;
    public static bool initialised = false;
    public static GridGenerator GridGenerator;

    private void Start()
    {
        viewDistance = setViewDistance;
        cell = setCell;
        cellCollider = setCellCollider;
        chunkIterations = setChunkIterations;
        Terrain = setTerrain;

        GridGenerator = Camera.main.gameObject.GetComponent<GridGenerator>();
        edgelength = setCellCollider.GetComponent<BoxCollider>().size.x;
        chunkSize = chunkIterations * edgelength;

        GridGenerator.GetComponent<GridGenerator>().Initialise();

        Debug.Log("edgelength: "+edgelength);
    }

    public static Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    private void Update()
    {
        //Draw edgeline in World for debugging
        Debug.DrawLine(new Vector3(0, 1, 0), new Vector3(edgelength, 1, 0), Color.red);
        Debug.DrawLine(new Vector3(0, 1, 0), new Vector3(0, 1, 10), Color.red);
    }

}
