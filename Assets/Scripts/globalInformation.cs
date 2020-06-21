using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

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

    public static bool initialised = false;
    public static int chunksTotal;
    public static int cellsTotal;
    public static int cellsVisible;
    public static int viewDistance;
    public static int chunkIterations;
    public static int MovementSpeed;
    public static int RotationSpeed;
    public static Vector2 terrainSize;
    public static float chunkSize;
    public static float edgelength;
    public static Vector3 position;
    public static GameObject cell;
    public static GameObject cellCollider;
    public static GameObject Terrain;
    public static Transform groundCursor;
    public static GridGenerator GridGenerator;

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

    private float fps;
    private bool fpsThreadRunning = false;

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

        //compute left statics
        GridGenerator = Player.GetComponent<GridGenerator>();
        edgelength = setCellCollider.GetComponent<BoxCollider>().size.x;
        chunkSize = chunkIterations * edgelength;
        groundCursor = Instantiate(new GameObject(), new Vector3(Player.transform.position.x, 0, Player.transform.position.z), Player.transform.rotation).transform;
        groundCursor.gameObject.name = "groundCursor";
        terrainSize = get2DBounds(Terrain);

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
        FPSUI.text = "aproximate FPS: " + fps;
    }
    IEnumerator calculateFPS()
    {
        fps = 1.0f / Time.deltaTime;
        yield return new WaitForSeconds(.5f);
        fpsThreadRunning = false;
    }
}
