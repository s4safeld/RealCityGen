using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    public int cellSize;
    public int roadWidth;
    public float perlinNoiseMultiplier;
    public float streetDensity;

    private BuildingGenerator[] generators;
    private float areaLength;
    private float areaWidth;
    private GameObject[,] cellGrid;
    private Node[,] streetGrid;
    private GameObject cellCollider;
    public GameObject street;
    public GameObject crossroad;
    public GameObject TJunction;
    public GameObject getCellCollider()
    {
        return cellCollider;
    }

    void Start()
    {
        //cellCollider = (GameObject)Instantiate(Resources.Load("Assets/Prefabs/CellCollider", typeof(GameObject)),gameObject.transform);
        cellCollider = new GameObject();
        generators = GetComponentsInChildren<BuildingGenerator>();

        float biggestSize = 0;
        foreach(BuildingGenerator generator in generators)
        {
            foreach(buildingStep step in generator.steps)
            {
                if (step.isPolygon)
                {
                    if(biggestSize < step.maxRadius * 2)
                    {
                        Debug.Log("step.maxRadius * 2 = " + step.maxRadius * 2);
                        biggestSize = step.maxRadius * 2;
                    }
                }
                else
                {
                    if(biggestSize < Mathf.Sqrt(step.maxWidth* step.maxWidth + step.maxLength* step.maxLength))
                    {
                        Debug.Log(String.Format("Mathf.Sqrt({0}*{0}+{1}*{1}) = {2}", step.maxWidth, step.maxLength, Mathf.Sqrt(step.maxWidth * step.maxWidth + step.maxLength * step.maxLength)));
                        biggestSize = Mathf.Sqrt(step.maxWidth * step.maxWidth + step.maxLength * step.maxLength);
                    }
                }
            }
        }
        cellSize = Mathf.CeilToInt(biggestSize) + roadWidth / 4;

        areaLength = GlobalInformation.get2DBounds(gameObject).x;
        areaWidth = GlobalInformation.get2DBounds(gameObject).y;

        GameObject cellGridParent = new GameObject();
        cellGridParent.name = "CellGrid";
        cellGridParent.transform.parent = transform;
        cellGrid = generateCellGrid(cellGridParent.transform);

        GameObject streetGridParent = new GameObject();
        streetGridParent.name = "StreetGrid";
        streetGridParent.transform.parent = transform;
        streetGrid = generateStreetGrid();


        //Debugging
        GameObject temp;
        for (int i = 0; i < (int)areaLength/cellSize - 1; i++)
        {
            for (int j = 0; j < (int)areaWidth/cellSize - 1; j++)
            {
                temp = new GameObject();
                temp.transform.position = streetGrid[i, j].position;
                temp.transform.name = ""+ streetGrid[i, j].position;
                temp.transform.parent = streetGridParent.transform;
            }
        }
        //-------

        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        for (int i = 0; i < areaLength / cellSize; i++)
        {
            Debug.DrawLine(new Vector3((i * cellSize) + (transform.position.x - areaLength / 2),5,1000), new Vector3((i * cellSize) + (transform.position.x - areaLength / 2), 5, -1000), Color.red);
        }
        for (int i = 0; i < areaWidth/ cellSize; i++)
        {
            Debug.DrawLine(new Vector3(1000, 5, (i * cellSize) + (transform.position.y - areaWidth / 2)), new Vector3(-1000, 5, (i * cellSize) + (transform.position.y - areaWidth / 2)), Color.red);
        }
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
                Debug.Log(value+", i: "+i+", j:"+j);
                if(value <1.0f)
                    cell.GetComponent<Cell>().Initialise(generators[0]);
                if(value < 0.75f)
                    cell.GetComponent<Cell>().Initialise(generators[1]);
                if (value < 0.5f)
                    cell.GetComponent<Cell>().Initialise(generators[2]);
                if (value < 0.25f)
                    cell.GetComponent<Cell>().Initialise(generators[3]);
                grid[i, j] = cell;
            }
        }
        return grid;
    }

    Node[,] generateStreetGrid()
    {
        Node[,] grid = new Node[(int)areaLength/cellSize+1, (int)areaWidth/cellSize+1];
        for(int i = 0; i<(int)areaLength/cellSize; i++)
        {
            for (int j = 0; j < (int)areaWidth/cellSize; j++)
            {
                Debug.Log("i:" + i + "| j:" + j);
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
                    grid[i, j].roadWest = true;
                    try { grid[i - 1, j].roadEast = true; } catch (Exception) { }
                }
                if (Random.Range(0.0f, 1.0f) > streetDensity)
                {
                    GameObject streetSouth = Instantiate(street, new Vector3(grid[i, j].position.x - cellSize / 2, 0.1f, grid[i, j].position.z ), street.transform.rotation);
                    streetSouth.transform.Rotate(new Vector3(0,90,0));
                    grid[i, j].roadSouth = true;
                    try { grid[i, j-1].roadNorth = true; } catch (Exception) { }
                }
            }
        }

        return grid;
    }

}

public class Node
{
    public Vector3 position;
    public bool roadNorth = false;
    public bool roadSouth = false;
    public bool roadEast = false;
    public bool roadWest = false;

    public Node(Vector3 pos)
    {
        position = pos;
    }
}