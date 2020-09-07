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

        //Debugging
        //Debug.DrawLine(new Vector3(0, 100, 0), new Vector3(0, 100, 30));
        //-----------
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

