using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class CellAlternative : MonoBehaviour
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
    private GridGeneratorAlternative gridGenerator;
    public Vector2 posInArray;

    private bool generated = false;
    private bool generationAllowed = true;
    private bool initialized = false;
    private bool disabled = false;
    private bool initialised = false;

    private float distance;

    public void Initialise(GridGeneratorAlternative parent)
    {
        generationAllowed = true;   //Temporary
        cellCollider = giAlternative.cellCollider;
        mainCam = Camera.main;
        camTransform = mainCam.gameObject.transform;
        groundCursor = giAlternative.groundCursor.transform;
        viewDistance = giAlternative.viewDistance;
        gridGenerator = parent;


        //temporary
        width = Random.Range(gridGenerator.minBuildingWidth, gridGenerator.maxBuildingWidth);
        height = Random.Range(gridGenerator.minBuildingHeigth, gridGenerator.maxBuildingHeigth);
        length = Random.Range(gridGenerator.minBuildingLength, gridGenerator.maxBuildingLength);
        //----------

        initialised = true;
    }
    public void Update()
    {
        if (initialised)
        {
            distance = Vector3.Distance(groundCursor.position, transform.position);
            if (distance < viewDistance)
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
                            giAlternative.cellsVisible++;
                            //Debug.DrawLine(transform.position, groundCursor.position, Color.white);
                            Generate();
                        }
                    }
                    else
                    {
                        if (generated)
                        {
                            giAlternative.cellsVisible--;
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
    }

    void Generate()
    {
        if (!initialized)
        {
            cellCollider.transform.position = transform.position;
            generationAllowed = cellCollider.GetComponent<CellCollider>().ask();
            initialized = true;
        }
        else
        {
            if (generationAllowed && !generated)
            {
                Debug.Log(name + " is generating cube");
                generated = true;
                //temporary--------
                spawnedBlock = Instantiate(spawnBlock, transform.position, Quaternion.identity, transform);
                tmp = spawnedBlock.transform.GetChild(0).GetComponent<TextMeshPro>();
                tmp.transform.SetParent(transform);
                spawnedBlock.transform.localScale = new Vector3(width, height, length);
                spawnedBlock.transform.position += new Vector3(0, getHeight(spawnedBlock) / 2, 0);
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
            //Debug.Log(name + ": is in View");
            return true;
        }
        return false;
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