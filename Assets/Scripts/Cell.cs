using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Cell : MonoBehaviour
{
    public GameObject spawnBlock;
    private GameObject spawnedBlock;
    private Camera mainCam;
    private GameObject cellCollider;
    private Transform camTransform;
    private Transform groundCursor;
    private ArrayList colliders;

    private bool generated = false;
    private bool generationAllowed;
    private bool initialized = true;

    private float distance;


    private void Start()
    {
        generationAllowed = true;   //Temporary
        cellCollider = globalInformation.cellCollider;
        mainCam = Camera.main;
        camTransform = mainCam.gameObject.transform;
        groundCursor = mainCam.GetComponent<PlayerMovement>().groundCursor.transform;
    }
    public void Update()
    {
        distance = Vector3.Distance(groundCursor.position, transform.position);

        if(generationAllowed && distance < globalInformation.viewDistance)
        {
            if(distance < globalInformation.edgelength*2)
            {
                Generate();
            }
            else
            {
                if (isInView())
                {
                    //Debug.DrawLine(transform.position, groundCursor.position, Color.white);
                    Generate();
                }
                else
                {
                    unGenerate();
                }
            }
        }
        else
        {
            unGenerate();
        }
        StartCoroutine(DisableScript());
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
                spawnedBlock = Instantiate(spawnBlock,transform.position, Quaternion.identity , transform);
                generated = true;
            }
        }
    }
    void unGenerate()
    {
        Destroy(spawnedBlock);
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
        yield return new WaitForSeconds(.01f);
        generationAllowed = cellCollider.GetComponent<GetColliders>().generationAllowed;
        colliders = cellCollider.GetComponent<GetColliders>().colliders;
        Destroy(cellCollider);
        initialized = true;      
    }
    IEnumerator DisableScript()
    {
        this.enabled = false;
        yield return new WaitForSeconds(.1f);
        this.enabled = true;
    }
}