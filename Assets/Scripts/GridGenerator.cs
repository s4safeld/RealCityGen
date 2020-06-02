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
    public GameObject Grid;
    private Vector2 cellSize;
    private float edgelength;
    private Vector3 spawnPosition;
    private globalInformation globalInformation;
    private Vector3 initPos;

    void Start()
    {
        globalInformation = GameObject.Find("globalInformation").GetComponent<globalInformation>();
        
        cellSize = get2DBounds(cell);
        edgelength = cellSize.x;
        
        spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);

        Grid = new GameObject("Grid");
        Instantiate(Grid);
        Instantiate(cell, spawnPosition, Quaternion.identity, Grid.transform);
        for (int i = 0; i < globalInformation.viewDistance / edgelength; i = i + 2)
        {
            spawnPosition = loopSpawn(i, edgelength, cell, spawnPosition, Grid.transform);
        }

        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(initPos, transform.position));
        Debug.Log(globalInformation.viewDistance / 2);
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
                spawnPosition = loopSpawn(i, edgelength, cell, spawnPosition, Grid.transform);
            }
            
        }
    }

    Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    //TODO: convert float to int
    /*This Function loops around the spawnposition and Instantiates a Grid
     *
     * */
    Vector3 loopSpawn(int rad, float step, GameObject obj, Vector3 pos, Transform parent)
    {
        GameObject spawned;
        pos = new Vector3(pos.x+step, 0, pos.z);
        spawned = Instantiate(obj, pos, Quaternion.identity, parent);
        spawned.name = (obj.name + pos);
        for(int i=0; i <= rad; i++)
        {
            pos = new Vector3(pos.x, 0, pos.z + step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for(int i=0; i <= rad/2; i++)
        {
            pos = new Vector3(pos.x - step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);

            pos = new Vector3(pos.x - step, 0, pos.z);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for (int i = 0; i <= rad/2; i++)
        {
            pos = new Vector3(pos.x, 0, pos.z-step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);

            pos = new Vector3(pos.x, 0, pos.z-step);
            spawned = Instantiate(obj, pos, Quaternion.identity, parent);
            spawned.name = (obj.name + pos);
        }
        for (int i = 0; i <= rad/2; i++)
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
