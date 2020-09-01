using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int cellSize;
    public int roadWith;

    private BuildingGenerator[] generators;
    private float areaLength;
    private float areaWidth;
    private Cell[,] grid;
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

        cellSize = cellSize - roadWith / 4;

        areaLength = GlobalInformation.get2DBounds(gameObject).x;
        areaWidth = GlobalInformation.get2DBounds(gameObject).y;

        GameObject cellGrid = new GameObject();
        cellGrid.name = "CellGrid";
        cellGrid.transform.parent = transform;
        grid = generateGrid(cellGrid.transform);

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

    Cell[,] generateGrid(Transform parent)
    {
        Cell[,] grid = new Cell[(int)areaLength,(int)areaWidth];

        for(int i = 0; i<areaLength/cellSize; i++)
        {
            for(int j = 0; j<areaWidth/cellSize; j++)
            {
                GameObject cell = new GameObject();
                cell.transform.position = new Vector3(
                    cellSize/2+(j*cellSize)+(transform.position.x - areaLength/2), 
                    0,
                    cellSize/2+(i*cellSize)+(transform.position.z - areaWidth/2)
                );
                cell.transform.parent = parent;
                cell.name = "cell" + cell.transform.position;
                cell.AddComponent<Cell>();
                //TODO: assignement of generator
                cell.GetComponent<Cell>().Initialise(generators[0]);
                grid[i,j] = cell.GetComponent<Cell>();
            }
        }

        return grid;
    }

}
