using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class globalInformation : MonoBehaviour
{
    public int setViewDistance;
    public GameObject setCell;
    public GameObject setCellCollider;
    public int setChunkIterations;
    public GameObject setTerrain;
    public int setMovementSpeed;
    public int setRotationSpeed;
    public GameObject Player;
    public Canvas CanvasUI;
    public string SetSeed;
    public GameObject SetgroundCursor;
    public float setMaxBuildingHeigth;
    public float setMaxBuildingWidth;
    public float setMaxBuildingLength;

    public static bool initialised = false;
    public static int chunksTotal = 0;
    public static int cellsTotal = 0;
    public static int cellsVisible = 0;
    public static int viewDistance = 0;
    public static int chunkIterations = 0;
    public static int MovementSpeed = 0;
    public static int RotationSpeed = 0;
    public static Vector2 terrainSize;
    public static float chunkSize = 0;
    public static float edgelength = 0;
    public static Vector3 position;
    public static GameObject cell;
    public static GameObject cellCollider;
    public static GameObject Terrain;
    public static Transform groundCursor;
    public static GridGenerator GridGenerator;
    public static GameObject Grid;
    public static Vector3[] chunks;
    public static int seed;
    public static float maxBuildingHeigth;
    public static float maxBuildingWidth;
    public static float maxBuildingLength;

    private Text ChunksTotalUI;
    private Text CellsTotalUI;
    private Text CellsVisibleUI;
    private Text ViewDistanceUI;
    private Text ChunkIterationsUI;
    private Text MovementSpeedUI;
    private Text RotationSpeedUI;
    private Text TerrainSizeUI;
    private Text ChunkSizeUI;
    private Text EdglengthUI;
    private Text PositionUI;
    private Text FPSUI;
    private Text seedUI;

    private float fps;
    private bool fpsThreadRunning = false;

    //Debugging
    public static bool chunksFull = false;

    private void Start()
    {
        //set all statics
        viewDistance = setViewDistance;
        cell = setCell;
        cellCollider = setCellCollider;
        chunkIterations = setChunkIterations;
        Terrain = setTerrain;
        MovementSpeed = setMovementSpeed;
        RotationSpeed = setRotationSpeed;
        groundCursor = SetgroundCursor.transform;
        maxBuildingHeigth = setMaxBuildingHeigth;
        maxBuildingLength = setMaxBuildingLength;
        maxBuildingWidth = setMaxBuildingWidth;
            
        //compute left statics
        GridGenerator = Player.GetComponent<GridGenerator>();
        edgelength = setCellCollider.GetComponent<BoxCollider>().size.x;
        chunkSize = chunkIterations * edgelength;
        terrainSize = get2DBounds(Terrain);
        Grid = new GameObject();
        Grid.name = "Grid";
        Instantiate(Grid);
        chunks = new Vector3[(int)((viewDistance/chunkSize)*(viewDistance/chunkSize))];
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i] = new Vector3(0,1,0);
        }
        if (SetSeed != null || SetSeed != " ")
        {
            try { seed = int.Parse(SetSeed); } catch (Exception) { seed = SetSeed.GetHashCode(); }
        }
        else
        {
            seed = UnityEngine.Random.Range(0, 10000000).GetHashCode();
        }

        //Call Initialize Functions for all scripts that need globalInformationValues
        Player.GetComponent<PlayerMovement>().Initialise();
        GridGenerator.GetComponent<GridGenerator>().Initialise();

        //UI stuff
        ChunksTotalUI       = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Text>();
        CellsTotalUI        = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Text>();
        CellsVisibleUI      = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>();
        ViewDistanceUI      = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(3).GetComponent<Text>();
        ChunkIterationsUI   = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(4).GetComponent<Text>();
        MovementSpeedUI     = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(5).GetComponent<Text>();
        RotationSpeedUI     = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(6).GetComponent<Text>();
        TerrainSizeUI       = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(7).GetComponent<Text>();
        ChunkSizeUI         = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(8).GetComponent<Text>();
        EdglengthUI         = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(9).GetComponent<Text>();
        PositionUI          = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(10).GetComponent<Text>();
        FPSUI               = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(11).GetComponent<Text>();
        seedUI              = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(12).GetComponent<Text>();
    }

    public static Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    private void Update()
    {
        if (!fpsThreadRunning)
        {
            fpsThreadRunning = true;
            StartCoroutine(calculateFPS());
        }
            
        position = Player.transform.position;
        ChunksTotalUI.text = "Chunks Total: "+chunksTotal;
        CellsTotalUI.text = "Cells Total: " + cellsTotal;
        CellsVisibleUI.text = "Cells Visible: " + cellsVisible;
        ViewDistanceUI.text = "View Distance: " + viewDistance;
        ChunkIterationsUI.text = "Chunk Iterations: " + chunkIterations;
        MovementSpeedUI.text = "Movement Speed: " + MovementSpeed;
        RotationSpeedUI.text = "Rotation Speed: " + RotationSpeed;
        TerrainSizeUI.text = "Terrain Size: X:" + terrainSize.x + " Z: " + terrainSize.y;
        ChunkSizeUI.text = "ChunkSize" + chunkSize;
        EdglengthUI.text = "Edglength: " + edgelength;
        PositionUI.text = "Position: X:" + position.x.ToString("0.00") + " Y: " + position.y.ToString("0.00") + " Z: " + position.z.ToString("0.00");
        FPSUI.text = "aproximate FPS: " + fps.ToString("0.00");
        seedUI.text = "Seed: " + seed;

        //Debugging stuff
        
    }
    IEnumerator calculateFPS()
    {
        fps = 1.0f / Time.deltaTime;
        yield return new WaitForSeconds(.5f);
        fpsThreadRunning = false;
    }
    public static void addChunk(Vector3 input)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i] == input)
            {
                Debug.Log("Chunk at: " + input + " is Already in Array");
                return;
            }
        }
        for (int i = 0; i<chunks.Length; i++)
        {
            if (chunks[i] == new Vector3(0,1,0))
            {
                chunks[i] = input;
                return;
            }
        }
        Debug.LogError("globalInformation.addChunk(): chunk Array is Full. GridGeneration can not continue!");
    }
    public static void removeChunk(Vector3 input)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i] == input)
            {
                chunks[i] = new Vector3(0,1,0);
            }
        }
        Debug.LogWarning("globalInformation.removeChunk(): chunk at: " + input + " not found!");
    }
}