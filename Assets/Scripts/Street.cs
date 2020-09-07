using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street : MonoBehaviour
{
    public Transform groundCursor;
    public Camera mainCam;
    public MeshRenderer mr;
    public bool mrEnabled = false;
    public Node node;
    private int viewDistance;
    private Light light;
    public bool streetNorth;
    public bool streetEast;
    public bool streetSouth;
    public bool streetWest;

    private void Start()
    {
        node = new Node(new Vector3());
        groundCursor = GlobalInformation.groundCursor.transform;
        mainCam = Camera.main;
        mr = GetComponent<MeshRenderer>();
        viewDistance = GlobalInformation.viewDistance;
        light = GetComponentInChildren<Light>();
        try { light.enabled = false; } catch (Exception) { }
        mr.enabled = false;
    }

    public void setNode(Node input)
    {
        node = input;
        streetNorth = node.roadNorth;
        streetEast = node.roadEast;
        streetSouth = node.roadSouth;
        streetWest = node.roadWest;
    }

    private void Update()
    {
        if(Vector3.Distance(groundCursor.position, transform.position) < viewDistance)
        {
            if (isInView() && !mrEnabled)
            {
                mr.enabled = true;
                foreach (MeshRenderer obj in transform.GetComponentsInChildren<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().enabled = true;
                mrEnabled = true;
                try { light.enabled = false; } catch (Exception) { }
            }
            else
            {
                if (!isInView() && mrEnabled)
                {
                    mr.enabled = false;
                    foreach (MeshRenderer obj in GetComponentsInChildren<MeshRenderer>())
                        obj.GetComponent<MeshRenderer>().enabled = false;
                    mrEnabled = false;
                    try { light.enabled = false; } catch (Exception) { }
                }

            }
        }
        else
        {
            mr.enabled = false;
            foreach (MeshRenderer obj in GetComponentsInChildren<MeshRenderer>())
                obj.GetComponent<MeshRenderer>().enabled = false;
            try { light.enabled = false; } catch (Exception) { }
        }
    }

    bool isInView()
    {
        if (Vector3.Distance(groundCursor.position, transform.position) > 50)
        {
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
}
