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
    public bool automaticStreetScale;

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
    
    public GameObject getCellCollider()
    {
        return cellCollider;
    }

    void Start()
    {
        //cellCollider = (GameObject)Instantiate(Resources.Load("Assets/Prefabs/CellCollider", typeof(GameObject)),gameObject.transform);
        cellCollider = new GameObject();
        generators = GetComponentsInChildren<BuildingGenerator>();

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
        if (automaticStreetScale)
        {
            //TODO
        }
        areaLength = GlobalInformation.get2DBounds(gameObject).x;
        areaWidth = GlobalInformation.get2DBounds(gameObject).y;

        GameObject cellGridParent = new GameObject();
        cellGridParent.name = "CellGrid";
        cellGridParent.transform.parent = transform;
        cellGrid = generateCellGrid(cellGridParent.transform);

        GameObject streetGridParent = new GameObject();
        streetGridParent.name = "StreetGrid";
        streetGridParent.transform.parent = transform;
        streetGrid = generateStreetGrid(streetGridParent.transform);

        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    GameObject[,] generateCellGrid(Transform parent)
    {
        GameObject[,] grid = new GameObject[(int)areaLength/cellSize,(int)areaWidth/cellSize];
        float value = 0;
        for(int i = 0; i<(int)areaLength/cellSize; i++)
        {
            for(int j = 0; j<(int)areaWidth/cellSize; j++)
            {
                GameObject cell = new GameObject();
                cell.transform.position = new Vector3(
                    cellSize/2+(i*cellSize)+(transform.position.x - areaLength/2), 
                    0,
                    cellSize/2+(j*cellSize)+(transform.position.z - areaWidth/2)
                );
                cell.transform.parent = parent;
                cell.name = "cell" + cell.transform.position;
                cell.AddComponent<Cell>();
                value = Mathf.PerlinNoise(i*perlinNoiseMultiplier,j*perlinNoiseMultiplier);
                //Debug.Log(value+", i: "+i+", j:"+j);
                float step = 1 / (float)generators.Length;
                for(int k = 0; k<generators.Length; k++)
                {
                    if (value < step*k+step)
                    {
                        cell.GetComponent<Cell>().Initialise(generators[k]);
                        break;
                    }
                }
                grid[i, j] = cell;
            }
        }
        return grid;
    }
    Node[,] generateStreetGrid(Transform parent)
    {
        Node[,] grid = new Node[(int)areaLength/cellSize, (int)areaWidth/cellSize];
        for(int i = 0; i<grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                //Debug.Log("i:" + i + "| j:" + j);
                grid[i, j] = new Node(new Vector3(
                    cellGrid[i, j].transform.position.x + cellSize/2,
                    0,
                    cellGrid[i, j].transform.position.z + cellSize/2
                ));
            }
        }

        Random.InitState(GlobalInformation.hash(GlobalInformation.worldSeed ^ gameObject.name.GetHashCode()));

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
        
        //Connect the streets
        GameObject con = new GameObject();
        foreach (Node n in grid)
        {
            if (n.roadNorth && n.roadSouth && n.roadEast && n.roadWest)
                con = crossRoad(n);
            if (n.roadNorth && n.roadSouth && n.roadEast && !n.roadWest)
                con = tJunction(n);
            if (!n.roadNorth && n.roadSouth && n.roadEast && n.roadWest)
                con = tJunction(n);
            if (n.roadNorth && n.roadSouth && !n.roadEast && n.roadWest)
                con = tJunction(n);
            if (n.roadNorth && !n.roadSouth && n.roadEast && n.roadWest)
                con = tJunction(n);
            if (!n.roadNorth && !n.roadSouth && n.roadEast && n.roadWest)
                con = connectHorizontal(n);
            if (n.roadNorth && n.roadSouth && !n.roadEast && !n.roadWest)
                con = connectVertical(n);
            if (n.roadNorth && !n.roadSouth && n.roadEast && !n.roadWest)
                con = corner(n);
            if (!n.roadNorth && n.roadSouth && n.roadEast && !n.roadWest)
                con = corner(n);
            if (!n.roadNorth && n.roadSouth && !n.roadEast && n.roadWest)
                con = corner(n);
            if(n.roadNorth && !n.roadSouth && !n.roadEast && n.roadWest)
                con = corner(n);
            if (n.roadNorth && !n.roadSouth && !n.roadEast && !n.roadWest)
                con = end(n);
            if (!n.roadNorth && n.roadSouth && !n.roadEast && !n.roadWest)
                con = end(n);
            if (!n.roadNorth && !n.roadSouth && n.roadEast && !n.roadWest)
                con = end(n);
            if (!n.roadNorth && !n.roadSouth && !n.roadEast && n.roadWest)
                con = end(n);
            con.transform.parent = parent;
            try { con.GetComponent<Street>().setNode(n); } catch (Exception) { }
        }
        return grid;
    }

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
}