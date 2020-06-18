using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*  This Script needs to be attached to the main Camera.
*   It is going to create a Grid of Cells in a certain radius around the Camera
*   Those Cells are special Plane GameObjects with a size of exactly 2.5 Unit
*   The script will make sure that the Cells wont collide but will have a distance of exactly zero to each other
*
*/

public class GridGenerator : MonoBehaviour
{
    
    public GameObject cell;
    private GameObject grid;
    public GameObject terrain;
    public int chunkSize = 5;
    private float edgelength;
    private Vector3 spawnPosition;
    private globalInformation globalInformation;
    private Vector3 initPos;
    private bool terrainGiven;

    void Start()
    {
        globalInformation = GameObject.Find("globalInformation").GetComponent<globalInformation>();
       
        edgelength = get2DBounds(cell).x;
        
        spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
        terrain = GameObject.Find("Terrain");

        grid = new GameObject();
        grid.name = "Grid";
        Instantiate(grid);



        if (terrain)  //If Grid is not Null
        {
            Debug.Log("Terrain found, \nfilling Terrain...");
            fillTerrain(cell, grid, terrain);
            //generateChunk(chunkSize, edgelength, gameObject.transform.position, grid.transform);
        }
        else
        {
            //This is optional and will be done in the future, for now we will work on given Terrains
            Instantiate(cell, spawnPosition, Quaternion.identity, grid.transform);

            for (int i = 0; i < globalInformation.viewDistance / edgelength; i = i + 2)
            {
                spawnPosition = generateChunk(chunkSize, edgelength, spawnPosition, grid.transform);
            }

            initPos = transform.position;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        //For now we assume that a terrain is always given, so the Realtime Generation of a Grid is put on hold for now.
        if (!terrain)
        {
            Debug.Log("This should not yet run, stop this now!");
            if (Vector3.Distance(initPos, transform.position) > 50)
            {
                initPos = transform.position;
                Debug.Log("Loading point reached");
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    spawnPosition = getGridCell(hit.collider.gameObject).transform.position;
                    initPos = spawnPosition;
                }
                for (int i = 0; i < globalInformation.viewDistance / edgelength; i = i + 2)
                {
                    spawnPosition = generateChunk(chunkSize, edgelength, spawnPosition, grid.transform);
                }

            }
        }
        
    }

    Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    void fillTerrain(GameObject cell, GameObject grid, GameObject terrain)
    {
        float width = get2DBounds(terrain).x;
        Debug.Log("width of grid: " + width);
        float length = get2DBounds(terrain).y;
        Debug.Log("length of grid: " + length);

        Vector3 pos = new Vector3(terrain.transform.position.x-width/2, 0, terrain.transform.position.z-length/2);
        Debug.Log("corner of Grid: " + pos);

        for(float i = 0; i < width; i=i+edgelength*chunkSize)
        {
            for (float j = 0; j < length; j = j + edgelength*chunkSize)
            {
                //Debug.Log("Generating Chunk at: " + pos);
                generateChunk(chunkSize, edgelength, pos, grid.transform);
                pos = new Vector3(pos.x+(edgelength*chunkSize), 0, pos.z);
            }
            pos = new Vector3((terrain.transform.position.x - width / 2), 0, pos.z + (edgelength * chunkSize));
        }
        grid.transform.parent = terrain.transform;
    }

    /*
     * This Function fills a chunk of given size
     * going from bottom left to top right
     * 
     */
    Vector3 generateChunk(int size, float step, Vector3 pos, Transform parent)
    {
        //Debugging input variables
        //Debug.Log("size: " + size);
        //Debug.Log("step: " + step);
        //Debug.Log("obj: " + cell.name);
        //Debug.Log("pos old: " + pos);

        
        pos = new Vector3(pos.x+edgelength/2,0,pos.z+edgelength/2);
        //Debug.Log("pos new: " + pos);

        GameObject spawned;
        GameObject chunk = new GameObject();
        chunk.name = "chunk" + pos;
        chunk.transform.parent = parent;
        //Instantiate(chunk);

        for (int i = 0; i<size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                spawned = Instantiate(cell, pos, Quaternion.identity);   //Instantiating cell, at given position with Grid as parent
                spawned.name = (cell.name + pos);
                spawned.transform.parent = chunk.transform;
                //Debug.Log("i: " + i + " || j: " + j + " || pos: " + pos);
                pos = new Vector3(pos.x + step, 0, pos.z);  //step left
                
            }
            pos = new Vector3(pos.x-(size*step), 0, pos.z+step);    //return to zero and step up
        }
        
        return pos;
    }
    /*
    Vector3 loopSpawn(int rad, float step, GameObject obj, Vector3 pos, Transform parent)
    {
        GameObject spawned;
        pos = new Vector3(pos.x + step, 0, pos.z);
        spawned = Instantiate(obj, pos, Quaternion.identity, parent);
        spawned.name = (obj.name + pos);
        for (int i = 0; i <= rad; i++)
        {
            pos = new Vector3(pos.x, 0, pos.z + step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for (int i = 0; i <= rad / 2; i++)
        {
            pos = new Vector3(pos.x - step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);

            pos = new Vector3(pos.x - step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for (int i = 0; i <= rad / 2; i++)
        {
            pos = new Vector3(pos.x, 0, pos.z - step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);

            pos = new Vector3(pos.x, 0, pos.z - step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for (int i = 0; i <= rad / 2; i++)
        {
            pos = new Vector3(pos.x + step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);

            pos = new Vector3(pos.x + step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }

        return pos;

    }
    */
    GameObject getGridCell(GameObject obj)
    {
        Debug.Log(obj.name);
        if (obj.tag == "GridCell")
        {
            return obj;
        }
        else
        {
            return getGridCell(obj.transform.parent.gameObject);
        }
    }
}
