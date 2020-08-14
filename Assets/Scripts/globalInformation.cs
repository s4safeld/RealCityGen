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
    public int setMovementSpeed;
    public int setRotationSpeed;
    public GameObject Player;
    public Canvas CanvasUI;
    public string SetSeed;
    public GameObject SetgroundCursor;
    public GameObject setTerrain;

    public static bool initialised = false;
    public static int cellsTotal = 0;
    public static int cellsVisible = 0;
    public static int viewDistance = 0;
    public static int MovementSpeed = 0;
    public static int RotationSpeed = 0;
    public static Vector2 terrainSize;
    public static float edgelength = 0;
    public static Vector3 position;
    public static GameObject cell;
    public static GameObject cellCollider;
    public static GameObject Terrain;
    public static Transform groundCursor;
    public static GameObject Grid;
    public static int seed;
    public static float fov;
    public static int possibleCellsInViewDistance;

    private Text CellsTotalUI;
    private Text CellsVisibleUI;
    private Text ViewDistanceUI;
    private Text MovementSpeedUI;
    private Text RotationSpeedUI;
    private Text EdglengthUI;
    private Text PositionUI;
    private Text FPSUI;
    private Text seedUI;

    private float fps;
    private bool fpsThreadRunning = false;

    //Debugging
    public static bool chunksFull = false;

    private void Awake()
    {
        //set all statics
        viewDistance = setViewDistance;
        cell = setCell;
        cellCollider = Instantiate(setCellCollider);
        MovementSpeed = setMovementSpeed;
        RotationSpeed = setRotationSpeed;
        groundCursor = SetgroundCursor.transform;
        Terrain = setTerrain;

        //compute left statics
        edgelength = setCellCollider.GetComponent<BoxCollider>().size.x;
        try { terrainSize = get2DBounds(Terrain); } catch (Exception) { Debug.LogWarning("It seems that Terrain has not been initialized, continuing with realtime generation Algorithm"); }
        Grid = new GameObject();
        Grid.name = "Grid";
        Instantiate(Grid);
        fov = Camera.main.fieldOfView;
        possibleCellsInViewDistance = (int)(viewDistance / edgelength);
        Debug.Log("SetSeed contains null: " + SetSeed.Contains("null"));
        if (!SetSeed.Contains("null"))
        {
            try { seed = int.Parse(SetSeed); } catch (Exception) { seed = SetSeed.GetHashCode(); }
        }
        else
        {
            seed = UnityEngine.Random.Range(0, 10000000).GetHashCode();
        }
        UnityEngine.Random.InitState(seed);


        //Call Initialize Functions for all scripts that need globalInformationValues
        Player.GetComponent<PlayerMovement>().Initialise();

        //UI stuff
        CellsTotalUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Text>();
        CellsVisibleUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(2).GetComponent<Text>();
        ViewDistanceUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(3).GetComponent<Text>();
        MovementSpeedUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(5).GetComponent<Text>();
        RotationSpeedUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(6).GetComponent<Text>();
        EdglengthUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(9).GetComponent<Text>();
        PositionUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(10).GetComponent<Text>();
        FPSUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(11).GetComponent<Text>();
        seedUI = CanvasUI.transform.GetChild(0).gameObject.transform.GetChild(12).GetComponent<Text>();
    }

    public static Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    private void Update()
    {
        //Debugging stuff
        
        //---------------

        if (!fpsThreadRunning)
        {
            fpsThreadRunning = true;
            StartCoroutine(calculateFPS());
        }

        position = Player.transform.position;
        CellsTotalUI.text = "Cells Total: " + cellsTotal;
        CellsVisibleUI.text = "Cells Visible: " + cellsVisible;
        ViewDistanceUI.text = "View Distance: " + viewDistance;
        MovementSpeedUI.text = "Movement Speed: " + MovementSpeed;
        RotationSpeedUI.text = "Rotation Speed: " + RotationSpeed;
        EdglengthUI.text = "Edglength: " + edgelength;
        PositionUI.text = "Position: X:" + position.x.ToString("0.00") + " Y: " + position.y.ToString("0.00") + " Z: " + position.z.ToString("0.00");
        FPSUI.text = "aproximate FPS: " + fps.ToString("0.00");
        seedUI.text = "Seed: " + seed;

    }
    IEnumerator calculateFPS()
    {
        fps = 1.0f / Time.deltaTime;
        yield return new WaitForSeconds(.5f);
        fpsThreadRunning = false;
    }
    bool isInView(Vector3 pos)
    {
        Vector3 targetDir = pos - groundCursor.position;
        float angle = Vector3.Angle(targetDir, groundCursor.forward);
        if (angle < fov)
        {
            return true;
        }
        return false;
    }
}