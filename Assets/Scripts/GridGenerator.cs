﻿using System.Collections;
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
    
    private GameObject cell;
    private GameObject grid;
    private GameObject terrain;
    private int chunkIterations;
    private float edgelength;
    private Vector3 spawnPosition;
    //private globalInformation globalInformation;
    private Vector3 initPos;
    private bool terrainGiven;
    private bool initialized;

    public void Initialise()
    {
        //globalInformation = GameObject.Find("globalInformation").GetComponent<globalInformation>();

            cell = globalInformation.cell;
            edgelength = globalInformation.edgelength;
            spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
            terrain = globalInformation.Terrain;
            chunkIterations = globalInformation.chunkIterations;
            grid = globalInformation.Grid;

            if (terrain)  //If Grid is not Null
            {
                Debug.Log("Terrain found, \nfilling Terrain...");
                fillTerrain(cell, grid, terrain);
            }
            else
            {
                generateChunk(chunkIterations, edgelength, new Vector3(transform.position.x,0,transform.position.z), grid.transform);
            }
            initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            //For now we assume that a terrain is always given, so the Realtime Generation of a Grid is put on hold for now.
            if (!terrain)
            {
                
            }
        }
        
    }

    void fillTerrain(GameObject cell, GameObject grid, GameObject terrain)
    {

        Debug.Log("width of terrain: " + globalInformation.terrainSize.x);
        Debug.Log("length of terrain: " + globalInformation.terrainSize.y);

        Vector3 pos = new Vector3(terrain.transform.position.x- globalInformation.terrainSize.x/2, 0, terrain.transform.position.z- globalInformation.terrainSize.y / 2);
        Debug.Log("corner of Grid: " + pos);

        for(float i = 0; i < globalInformation.terrainSize.x; i= i + edgelength * chunkIterations)
        {
            for (float j = 0; j < globalInformation.terrainSize.y; j = j + edgelength * chunkIterations)
            {
                //Debug.Log("Generating Chunk at: " + pos);
                generateChunk(chunkIterations, edgelength, pos, grid.transform);
                pos = new Vector3(pos.x+(edgelength * chunkIterations), 0, pos.z);
            }
            pos = new Vector3((terrain.transform.position.x - globalInformation.terrainSize.x / 2), 0, pos.z + (edgelength * chunkIterations));
        }
        //grid.transform.parent = terrain.transform;
    }

    /*
     * This Function fills a chunk of given size
     * going from bottom left to top right
     * 
     */
    public Vector3 generateChunk(int iterations, float step, Vector3 pos, Transform parent)
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
        chunk.transform.position = pos;
        chunk.name = "chunk" + pos;
        globalInformation.chunksTotal++;

        for (int i = 0; i<iterations; i++)
        {
            for (int j = 0; j < iterations; j++)
            {
                spawned = Instantiate(cell, pos, Quaternion.identity);   //Instantiating cell, at given position with Grid as parent
                spawned.name = (cell.name + pos);
                spawned.transform.parent = chunk.transform;
                globalInformation.cellsTotal++;
                pos = new Vector3(pos.x + step, 0, pos.z);  //step left
                
            }
            pos = new Vector3(pos.x-(iterations*step), 0, pos.z+step);    //return to zero and step up
        }
        chunk.transform.parent = parent;
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
