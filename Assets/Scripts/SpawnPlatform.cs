using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnPlatform : MonoBehaviour
{
    public GameObject spawnBlock;
    public GameObject spawnedBlock;
    public Camera mainCam;

    public Material defMat;
    public Material blackMat;
    
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
                spawnedBlock = Instantiate(spawnBlock, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
                isSpawned = true;
            }
                
        }
    }

    void OnBecameVisible()
    {
        isVisible = true;
        if (Vector3.Distance(transform.position, mainCam.transform.position) <= 100 && isSpawned == false)
        {
            spawnedBlock = Instantiate(spawnBlock, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
            spawnedBlock.name = ("spawnblock" +spawnedBlock.transform.position);
            isSpawned = true;
            
        } 
    }
    void OnBecameInvisible()
    {
        isVisible = false;
        Destroy(spawnedBlock);
        isSpawned = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "GridCell")
        {
            Destroy(this);
        }
    }
}
