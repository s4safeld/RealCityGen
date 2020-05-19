using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnPlatform : MonoBehaviour
{
    public GameObject spawnBlock;
    public GameObject spawnedBlock;

    private new Vector3 pos;

    private void Start()
    {
        pos = gameObject.transform.position;
    }

    void OnBecameVisible()
    {
        spawnedBlock = Instantiate(spawnBlock, pos + new Vector3(0, 0.5f, 0), Quaternion.identity);
    }
    void OnBecameInvisible()
    {
        Destroy(spawnedBlock);
    }
}
