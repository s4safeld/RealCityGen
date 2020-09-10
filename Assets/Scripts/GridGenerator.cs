using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    public bool automaticCellsize;
    public int cellSize;
    public int roadWidth;
    public float perlinNoiseMultiplier;
    public float streetDensity;

    public GameObject street;
    public GameObject crossroad;
    public GameObject TJunction;
    public GameObject streetCorner;
    public GameObject streetEnd;
    public GameObject connection;

    private BuildingGenerator[] generators;
    private float areaLength;
    private float areaWidth;
    private GameObject[,] cellGrid;
    private Node[,] streetGrid;
    private GameObject cellCollider;
    private Quaternion cubeRotation;
    private float PerlinOffset;

    public GameObject getCellCollider()
    {
        return cellCollider;
    }

    void Start()
    {
        Random.InitState(GlobalInformation.hash(GlobalInformation.worldSeed ^ gameObject.name.GetHashCode()));
        PerlinOffset = Random.Range(0,100);

        cubeRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        cellCollider = new GameObject();
        generators = GetComponentsInChildren<BuildingGenerator>();
        ArrayList obstacles = new ArrayList();
        foreach (Collider col in FindObjectsOfType<Collider>())
        {
            if (!col.CompareTag("ignore"))
            {
                Debug.Log(col.name + " is added to obstacles by "+name);
                obstacles.Add(col);
            }
        }
        Debug.Log(name);
        //compute cellSize by computing maximum building dimensions
        if (automaticCellsize)
        {
            float biggestSize = 0;
            foreach (BuildingGenerator generator in generators)
            {
                foreach (buildingStep step in generator.steps)
                {
                    if (step.isPolygon)
                    {
                        if (biggestSize < step.maxRadius * 2)
                        {
                            Debug.Log("step.maxRadius * 2 = " + step.maxRadius * 2);
                            biggestSize = step.maxRadius * 2;
                        }
                    }
                    else
                    {
                        if (biggestSize < Mathf.Sqrt(step.maxWidth * step.maxWidth + step.maxLength * step.maxLength)*2)
                        {
                            Debug.Log(String.Format("Mathf.Sqrt({0}*{0}+{1}*{1}) = {2})*2", step.maxWidth, step.maxLength, Mathf.Sqrt(step.maxWidth * step.maxWidth + step.maxLength * step.maxLength)*2));
                            biggestSize = Mathf.Sqrt(step.maxWidth * step.maxWidth + step.maxLength * step.maxLength)*2;
                        }
                    }
                }
            }
            cellSize = Mathf.CeilToInt(biggestSize) + roadWidth / 4;
        }
        //---------------------------------------------------------

        areaLength = GlobalInformation.get2DBounds(gameObject).x;
        areaWidth = GlobalInformation.get2DBounds(gameObject).y;
        //Generate cellgrid
        GameObject cellGridParent = new GameObject();
        cellGridParent.name = "CellGrid"+name;
        cellGridParent.transform.parent = transform;
        cellGrid = generateCellGrid(cellGridParent.transform);
        //Generate streetGrid
        GameObject streetGridParent = new GameObject();
        streetGridParent.name = "StreetGrid"+name;
        streetGridParent.transform.parent = transform;
        streetGrid = generateStreetGrid(streetGridParent.transform);
        //Delete Grid where Obstacles are found
        foreach (Collider col in obstacles)
        {
            RaycastHit hit;
            Ray ray;
            foreach(Transform obj in streetGridParent.GetComponentsInChildren<Transform>())
            {
                //Check if obj is within obstacle
                try{ 
                    Vector3 offset = col.bounds.center - obj.position;
                    ray = new Ray(obj.position, offset.normalized);
                    if (!col.Raycast(ray, out hit, offset.magnitude * 1.1f))
                    {
                        Debug.Log(obj.name + "will be destroyed");
                        Destroy(obj.gameObject);
                    }
                } catch (Exception e) { Debug.LogWarning(e); }
            }
            foreach (Transform obj in cellGridParent.GetComponentsInChildren<Transform>())
            {
                //Check if obj is within obstacle
                try{
                    Vector3 offset = col.bounds.center - obj.position;
                    ray = new Ray(obj.position, offset.normalized);
                    if (!col.Raycast(ray, out hit, offset.magnitude * 1.1f))
                    {
                        Debug.Log(obj.name + "will be destroyed");
                        Destroy(obj.gameObject);
                    }
                        
                } catch (Exception e) { Debug.LogWarning(e); }
            }
        }
        //--------------------------------

        foreach(Collider col in GetComponentsInChildren<Collider>())
        {
            col.gameObject.tag = "ignore";
        }

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        transform.rotation = cubeRotation;
    }

    GameObject[,] generateCellGrid(Transform parent)
    {
        GameObject[,] grid = new GameObject[(int)areaLength/cellSize,(int)areaWidth/cellSize];
        float value = 0;
        for(int i = 0; i<(int)areaLength/cellSize; i++)
        {
            for(int j = 0; j<(int)areaWidth/cellSize; j++)
            {
                //Instantiate cell-----------------
                GameObject cell = new GameObject();
                cell.transform.parent = parent;
                cell.AddComponent<Cell>();
                cell.transform.position = new Vector3(
                    cellSize/2+(i*cellSize)+(transform.position.x - areaLength/2), 
                    0,
                    cellSize/2+(j*cellSize)+(transform.position.z - areaWidth/2)
                );
                cell.name = "cell" + cell.transform.position;
                //------------------------------

                //Initialise Cell by picking a generator using Perlin Noise
                value = Mathf.PerlinNoise(i*perlinNoiseMultiplier+PerlinOffset,j*perlinNoiseMultiplier+PerlinOffset);
                float step = 1 / (float)generators.Length;
                for(int k = 0; k<generators.Length; k++)
                {
                    if (value < step*k+step)
                    {
                        cell.GetComponent<Cell>().Initialise(generators[k]);
                        break;
                    }
                }
                //--------------------------------------------------------

                int l = 0;
                bool found = false;
                while (!found)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(cell.transform.position.x, l, cell.transform.position.z), transform.TransformDirection(-Vector3.up), out hit))
                    {
                        if (hit.collider.gameObject == GlobalInformation.terrain)
                        {
                            if (hit.point.y > cell.GetComponent<Cell>().spawnHeight)
                                cell.GetComponent<Cell>().spawnHeight = hit.point.y;
                            found = true;
                        }
                    }
                    l++;
                    if (l > 100)
                    {
                        Debug.LogError("Something is wrong, is there terrain?");
                        found = true;
                    }
                }
                grid[i, j] = cell;
            }
        }
        return grid;
    }
    Node[,] generateStreetGrid(Transform parent)
    {
        //Lay out grid of nodes
        Node[,] grid = new Node[(int)areaLength/cellSize, (int)areaWidth/cellSize];
        for(int i = 0; i<grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new Node(new Vector3(
                    cellGrid[i, j].transform.position.x + cellSize/2,
                    0,
                    cellGrid[i, j].transform.position.z + cellSize/2
                ));
            }
        }
        //-----------------------

        Random.InitState(GlobalInformation.hash(GlobalInformation.worldSeed ^ gameObject.name.GetHashCode()));

        //Instantiate street Grid in random fashion---------
        for (int i = 0; i < (int)areaLength / cellSize; i++)
        {
            for (int j = 0; j < (int)areaWidth / cellSize; j++)
            {
                //Instantiate street to the west
                if (Random.Range(0.0f, 1.0f) > streetDensity)
                {
                    GameObject streetWest = Instantiate(street, new Vector3(grid[i,j].position.x,0.1f,grid[i,j].position.z - cellSize / 2), street.transform.rotation);
                    streetWest.AddComponent<Street>();
                    streetWest.name = "Street_x="+grid[i, j].position.x+", z="+(grid[i, j].position.z-cellSize/2);
                    streetWest.transform.parent = parent;
                    grid[i, j].roadEast = true;
                    try { grid[i, j-1].roadWest = true; } catch (Exception) { }
                }
                //Instantiate street to the south
                if (Random.Range(0.0f, 1.0f) > streetDensity)
                {
                    GameObject streetSouth = Instantiate(street, new Vector3(grid[i, j].position.x - cellSize / 2, 0.1f, grid[i, j].position.z ), street.transform.rotation);
                    streetSouth.AddComponent<Street>();
                    streetSouth.name = "Street_x=" + (grid[i, j].position.x - cellSize / 2) + ", z=" + grid[i, j].position.z;
                    streetSouth.transform.parent = parent;
                    streetSouth.transform.Rotate(new Vector3(0,90,0));
                    grid[i, j].roadSouth = true;
                    try { grid[i-1, j].roadNorth = true; } catch (Exception) { }
                }
            }
        }
        //---------------------------------------------------
        
        //Connect the streets--------------------------------
        GameObject con = new GameObject();
        foreach (Node n in grid)
        {
            if (n.roadNorth && n.roadSouth && n.roadEast && n.roadWest)     //crossroad
                con = crossRoad(n);
            if (!n.roadNorth && n.roadSouth && n.roadEast && n.roadWest)    //T-junction w/o North
                con = tJunction(n);
            if (n.roadNorth && n.roadSouth && !n.roadEast && n.roadWest)    //T-junction w/o East
                con = tJunction(n);
            if (n.roadNorth && !n.roadSouth && n.roadEast && n.roadWest)    //T-junction w/o South
                con = tJunction(n);
            if (n.roadNorth && n.roadSouth && n.roadEast && !n.roadWest)    //T-junction w/o West
                con = tJunction(n);
            if (!n.roadNorth && !n.roadSouth && n.roadEast && n.roadWest)   //Connect 2 horizontal streets
                con = connectHorizontal(n);
            if (n.roadNorth && n.roadSouth && !n.roadEast && !n.roadWest)   //Connect 2 vertical streets
                con = connectVertical(n);
            if (n.roadNorth && !n.roadSouth && n.roadEast && !n.roadWest)   //Corner N/E
                con = corner(n);
            if (!n.roadNorth && n.roadSouth && n.roadEast && !n.roadWest)   //Corner E/S
                con = corner(n);
            if (!n.roadNorth && n.roadSouth && !n.roadEast && n.roadWest)   //Corner S/W
                con = corner(n);
            if(n.roadNorth && !n.roadSouth && !n.roadEast && n.roadWest)    //Corner W/N
                con = corner(n);
            if (n.roadNorth && !n.roadSouth && !n.roadEast && !n.roadWest)  //end N
                con = end(n);
            if (!n.roadNorth && !n.roadSouth && n.roadEast && !n.roadWest)  //end E
                con = end(n);
            if (!n.roadNorth && n.roadSouth && !n.roadEast && !n.roadWest)  //end S
                con = end(n);
            if (!n.roadNorth && !n.roadSouth && !n.roadEast && n.roadWest)  //end W
                con = end(n);

            //If node has got a gameobject
            try { con.GetComponent<Street>().setNode(n); n.inheritor = con; } catch (Exception) { }

            //add connection to street parent
            con.transform.parent = parent;
        }
        //---------------------------------------------------

        //Set height of all streets------------------------------
        float spawnHeight = 0;
        foreach(Node n in grid)
        {
            int i = 0;
            bool found = false;
            //Get height of terrain at every node
            while (!found)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(n.position.x, i, n.position.z), transform.TransformDirection(-Vector3.up), out hit))
                {
                    if (hit.collider.gameObject == GlobalInformation.terrain)
                    {
                        if (hit.point.y > spawnHeight)
                            spawnHeight = hit.point.y+0.1f;
                        found = true;
                    }
                }
                i++;
                if (i > 100)
                {
                    Debug.LogError("Something is wrong, is there terrain?");
                    found = true;
                }
            }
        }
        foreach(Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if(child.GetComponent<Street>())
                child.position = new Vector3(child.position.x, spawnHeight + 0.1f, child.position.z);
        }
        //------------------------------------------------------

        return grid;
    }

    //Support functions for Streetgeneration--------------------
    private GameObject end(Node node)
    {
        GameObject end = Instantiate(streetEnd, new Vector3(node.position.x, node.position.y+0.1f, node.position.z), streetEnd.transform.rotation);
        end.AddComponent<Street>();
        if (node.roadNorth)
        {
            end.name = "Street-End_North at: " + node.position;
            end.transform.Rotate(new Vector3(0, 0, 0));
        }
        if (node.roadEast)
        {
            end.name = "Street-End_East at: " + node.position;
            end.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (node.roadWest)
        {
            end.name = "Street-End_West at: " + node.position;
            end.transform.Rotate(new Vector3(0, -90, 0));
        }
        if (node.roadSouth)
        {
            end.name = "Street-End_South at: " + node.position;
            end.transform.Rotate(new Vector3(0, 180, 0));
        }

        return end;
    }
    private GameObject corner(Node node)
    {
        GameObject corner = Instantiate(streetCorner, new Vector3(node.position.x, node.position.y + 0.1f, node.position.z), streetCorner.transform.rotation);
        corner.AddComponent<Street>();
        if (node.roadNorth && node.roadEast)
        {
            corner.name = "Corner for North and East at: " + node.position;
            corner.transform.Rotate(new Vector3(0, 180, 0));
        }
        if (node.roadEast && node.roadSouth)
        {
            corner.name = "Corner for East and South at: " + node.position;
            corner.transform.Rotate(new Vector3(0, -90, 0));
        }
        if (node.roadSouth && node.roadWest)
        {
            corner.name = "Corner for South and West at: " + node.position;
            corner.transform.Rotate(new Vector3(0, 0, 0));
        }
        if (node.roadWest && node.roadNorth)
        {
            corner.name = "Corner for West and North at: " + node.position;
            corner.transform.Rotate(new Vector3(0, 90, 0));
        }

        return corner;
    }
    private GameObject tJunction(Node node)
    {
        GameObject t = Instantiate(TJunction, new Vector3(node.position.x, node.position.y + 0.1f, node.position.z), TJunction.transform.rotation);
        t.AddComponent<Street>();
        if (!node.roadWest)
        {
            t.name = "Tjunction excluding West at: " + node.position;
            t.transform.Rotate(new Vector3(0, -90, 0));
        }
        if (!node.roadNorth)
        {
            t.name = "Tjunction excluding North at: " + node.position;
            t.transform.Rotate(new Vector3(0, 0, 0));
        }
        if (!node.roadEast)
        {
            t.name = "Tjunction excluding East at: " + node.position;
            t.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (!node.roadSouth)
        {
            t.name = "Tjunction excluding South at: " + node.position;
            t.transform.Rotate(new Vector3(0, 180, 0));
        }

        return t;
    }
    private GameObject crossRoad(Node node)
    { 
        GameObject cross = Instantiate(crossroad, new Vector3(node.position.x, node.position.y + 0.1f, node.position.z), crossroad.transform.rotation);
        cross.AddComponent<Street>();
        cross.GetComponent<Street>().node = node;
        cross.name = "Crossroad at: " + node.position;

        return cross;
    }
    private GameObject connectVertical(Node node)
    {
        GameObject con = Instantiate(connection, new Vector3(node.position.x, node.position.y + 0.1f, node.position.z), connection.transform.rotation);
        con.AddComponent<Street>();
        con.GetComponent<Street>().node = node;
        con.transform.Rotate(new Vector3(0, 90, 0));
        con.name = "Connection for East and West at: " + node.position;

        return con;
    }
    private GameObject connectHorizontal(Node node)
    {
        GameObject con = Instantiate(connection, new Vector3(node.position.x, node.position.y + 0.1f, node.position.z), connection.transform.rotation);
        con.AddComponent<Street>();
        con.GetComponent<Street>().node = node;
        con.name = "Connection for North and South at: " + node.position;
        con.transform.Rotate(new Vector3(0, 0, 0));

        return con;
    }
    //-----------------------------------------------------------
}