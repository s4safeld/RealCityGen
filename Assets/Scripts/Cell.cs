using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Cell : MonoBehaviour
{
    private Camera mainCam;
    private GameObject cellCollider;
    private Transform camTransform;
    private Transform groundCursor;
    private int viewDistance;
    private float edgelength;
    public int localSeed;
    private GridGenerator gridGenerator;
    public BuildingGenerator buildingGenerator;
    public GameObject building;

    private bool generated = false;
    private bool generationAllowed = true;
    private bool disabled = false;
    private bool initialised = false;

    private float distance;

    public void Initialise(BuildingGenerator bg)
    {
        gridGenerator = bg.gridGenerator;
        edgelength = gridGenerator.cellSize;
        generationAllowed = true;   //Temporary
        cellCollider = gridGenerator.getCellCollider();
        mainCam = Camera.main;
        camTransform = mainCam.gameObject.transform;
        groundCursor = GlobalInformation.groundCursor.transform;
        viewDistance = GlobalInformation.viewDistance;
        buildingGenerator = bg;
        localSeed = GlobalInformation.hash((int)transform.position.x ^ GlobalInformation.hash(GlobalInformation.worldSeed ^ (int)transform.position.z));
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
                            GlobalInformation.cellsVisible++;
                            //Debug.DrawLine(transform.position, groundCursor.position, Color.white);
                            Generate();
                        }
                    }
                    else
                    {
                        if (generated)
                        {
                            GlobalInformation.cellsVisible--;
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
        /*if (!initialised)
        {
            cellCollider.transform.position = transform.position;
            generationAllowed = cellCollider.GetComponent<CellCollider>().ask();
        }
        else
        {*/
            if (generationAllowed && !generated)
            {
                generated = true;
                building = buildingGenerator.generate(localSeed);
                building.transform.position = new Vector3(transform.position.x, building.transform.position.y, transform.position.z);
                building.transform.parent = transform;
        }
        //}
    }
    void unGenerate()
    {
        if (generated)
        {
            Destroy(building);
        }
        StartCoroutine(DisableScript());
        generated = false;
    }

    bool isInView()
    {
        if (Vector3.Distance(groundCursor.position, transform.position) > 50) {
            Vector3 targetDir = transform.position - groundCursor.position;
            float angle = Vector3.Angle(targetDir, groundCursor.forward);
            if (angle < (mainCam.fieldOfView))
            {
                return true;
            }
            return false;
        }
        return true;
    }
    IEnumerator DisableScript()
    {
        this.enabled = false;
        yield return new WaitForSeconds(.1f);
        this.enabled = true;
    }
}