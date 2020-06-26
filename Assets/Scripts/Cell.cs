using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Cell : MonoBehaviour
{
    public GameObject spawnBlock;
    private GameObject spawnedBlock;
    private Camera mainCam;
    private GameObject cellCollider;
    private Transform camTransform;
    private Transform groundCursor;
    private ArrayList colliders;
    private int viewDistance;
    private float edgelength;
    private int localSeed;
    private float height;
    private float width;
    private float length;
    private TextMeshPro tmp;

    private bool generated = false;
    private bool generationAllowed = true;
    private bool initialized = false;
    private bool disabled = false;

    private float distance;

    private void Start()
    {
        generationAllowed = true;   //Temporary
        cellCollider = globalInformation.cellCollider;
        mainCam = Camera.main;
        camTransform = mainCam.gameObject.transform;
        groundCursor = globalInformation.groundCursor.transform;
        viewDistance = globalInformation.viewDistance;
        edgelength = globalInformation.edgelength;
        localSeed = hash((int)transform.position.x ^ hash((int)transform.position.z ^ globalInformation.seed));

        //temporary
        height = localSeed;
        width = localSeed;
        length = localSeed;

        while (width > globalInformation.maxBuildingWidth)
        {
            width = width / 2;
        }
        while (height > globalInformation.maxBuildingHeigth)
        {
            height = height / 2;
        }
        while (length > globalInformation.maxBuildingLength)
        {
            length = length / 2;
        }
        //----------
    }
    public void Update()
    {
            distance = Vector3.Distance(groundCursor.position, transform.position);
            if (generationAllowed && distance < viewDistance)
            {
                if (distance < edgelength * 2)
                {
                    Generate();
                }
                else
                {
                    if (isInView())
                    {
                        if (!generated)
                        {
                            globalInformation.cellsVisible++;
                            //Debug.DrawLine(transform.position, groundCursor.position, Color.white);
                            Generate();
                        }
                    }
                    else
                    {
                        if (generated)
                        {
                            globalInformation.cellsVisible--;
                            unGenerate();
                        }
                    }
                }
            }
            else
            {
                unGenerate();
            }
    }

    void Generate()
    {
        if (!initialized)
        {
            StartCoroutine(Initialise());
        }
        else
        {
            if (generationAllowed && !generated)
            {
                generated = true;
                //temporary--------
                spawnedBlock = Instantiate(spawnBlock,transform.position, Quaternion.identity , transform);
                tmp = spawnedBlock.transform.GetChild(0).GetComponent<TextMeshPro>();
                tmp.transform.SetParent(transform);
                spawnedBlock.transform.localScale = new Vector3(width,height,length);
                spawnedBlock.transform.position += new Vector3(0, getHeight(spawnedBlock)/2, 0);
                tmp.transform.position = tmp.transform.position + new Vector3(0, 4, -5);
                tmp.transform.SetParent(spawnedBlock.transform);
                tmp.text = localSeed + "\n" + width + "\n" + height + "\n" + length;
                //-------------------
            }
        }
    }
    void unGenerate()
    {
        if (generated)
        {
            Destroy(spawnedBlock);
        }
        StartCoroutine(DisableScript());
        generated = false;
    }

    bool isInView()
    {
        Vector3 targetDir = transform.position - groundCursor.position;
        float angle = Vector3.Angle(targetDir, groundCursor.forward);
        if (angle < (mainCam.fieldOfView))
        {
            return true;
        }
        return false;
    }
    IEnumerator Initialise()
    {
        Debug.Log("Initialising");
        Instantiate(cellCollider);
        cellCollider.transform.position = transform.position;
        //cellCollider.transform.parent = transform;
        yield return new WaitForSeconds(.1f);
        generationAllowed = cellCollider.GetComponent<GetColliders>().ask();
        colliders = cellCollider.GetComponent<GetColliders>().colliders;
        //Destroy(cellCollider);
        initialized = true;      
    }
    IEnumerator DisableScript()
    {
        this.enabled = false;
        yield return new WaitForSeconds(.1f);
        this.enabled = true;
    }

    int hash(int key)
    {
        key += ~(key << 15);
        key ^= (key >> 10);
        key += (key << 3);
        key ^= (key >> 6);
        key += ~(key << 11);
        key ^= (key >> 16);
        return key;
    }
    float getHeight(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
        return obj.transform.localScale.y * bounds.size.y;
    }
}