using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInformation : MonoBehaviour
{
    public int setViewDistance;
    public GameObject setPlayer;
    public string setWorldSeed;
    public GameObject setTerrain;

    public static int viewDistance;
    public static GameObject groundCursor;
    public static GameObject player;
    public static GameObject terrain;
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
        Debug.Log("WorldSeed: "+worldSeed);

        viewDistance = setViewDistance;
        terrain = setTerrain;

        terrain.gameObject.tag = "ignore";


        if (setPlayer)
        {
            player = setPlayer;
            player.tag = "ignore";
        }
        groundCursor = new GameObject();
        groundCursor.name = "GroundCursor";
    }

    void Update()
    {
        if (player)
        {
            groundCursor.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            groundCursor.transform.rotation = player.transform.rotation;
        }
    }

    public static Vector2 get2DBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return new Vector2(obj.transform.localScale.x * bounds.size.x, obj.transform.localScale.z * bounds.size.z);
    }

    public static int hash(int key)
    {
        key += ~(key << 15);
        key ^= (key >> 10);
        key += (key << 3);
        key ^= (key >> 6);
        key += ~(key << 11);
        key ^= (key >> 16);
        return key;
    }
}

