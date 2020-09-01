using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInformation : MonoBehaviour
{
    public int setViewDistance;
    public GameObject setPlayer;
    public string setWorldSeed;

    public static int viewDistance;
    public static GameObject groundCursor;
    public static GameObject player;
    public static ArrayList GenerationCubes;
    public static int worldSeed;
    public static int cellsVisible = 0;

    void Awake()
    {
        if (!setWorldSeed.Contains("null"))
        {
            try { worldSeed = int.Parse(setWorldSeed); } catch (Exception) { worldSeed = setWorldSeed.GetHashCode(); }
        }
        else
        {
            worldSeed = UnityEngine.Random.Range(0, 10000000).GetHashCode();
        }
        Debug.Log(worldSeed);

        viewDistance = setViewDistance;
        player = setPlayer;

        groundCursor = new GameObject();
        groundCursor.name = "GroundCursor";
    }

    void Update()
    {
        groundCursor.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        groundCursor.transform.rotation = player.transform.rotation;
    }

    public static Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }
}
