using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnPlatform : MonoBehaviour
{
    public GameObject spawnBlock;
    public GameObject spawnedBlock;
    public Camera mainCam;
    
    private bool isSpawned = false;
    private bool isVisible = false;
    //private int viewDistance;

    private void Start()
    {
        mainCam = Camera.main;
    }
    public void Update()
    {
        if (Vector3.Distance(transform.position, mainCam.transform.position) >= 100)
        {
            Destroy(spawnedBlock);
            isSpawned = false;
        }
        else
        {
            if (!isSpawned && isVisible)
            {
                spawnedBlock = Instantiate(spawnBlock, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                isSpawned = true;
            }
                
        }
    }

    void OnBecameVisible()
    {
        isVisible = true;
        if (Vector3.Distance(transform.position, mainCam.transform.position) <= 100 && isSpawned == false)
        {
            spawnedBlock = Instantiate(spawnBlock, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            isSpawned = true;
        } 
    }
    void OnBecameInvisible()
    {
        isVisible = false;
        Destroy(spawnedBlock);
        isSpawned = false;
    }
}
