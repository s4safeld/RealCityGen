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
    public Vector3 spawnPos;

    private void Start()
    {
        spawnPos = transform.position;
        node = new Node(new Vector3());
        groundCursor = GlobalInformation.groundCursor.transform;
        mainCam = Camera.main;
        mr = GetComponent<MeshRenderer>();
        viewDistance = GlobalInformation.viewDistance;
        light = GetComponentInChildren<Light>();
        try { light.enabled = false; } catch (Exception) { }

        MeshFilter[] mfs = transform.GetComponentsInChildren<MeshFilter>();
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
        if(Vector3.Distance(groundCursor.position, transform.position) < viewDistance && GlobalInformation.player)
        {
            if (isInView() && !mrEnabled)
            {
                mr.enabled = true;
                foreach (MeshRenderer obj in transform.GetComponentsInChildren<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().enabled = true;
                mrEnabled = true;
                try { light.enabled = true; } catch (Exception) { }
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
            if (GlobalInformation.player)
            {
                mr.enabled = false;
                mrEnabled = false;
                foreach (MeshRenderer obj in GetComponentsInChildren<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().enabled = false;
                try { light.enabled = false; } catch (Exception) { }
            }
            else
            {
                mr.enabled = true;
                foreach (MeshRenderer obj in transform.GetComponentsInChildren<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().enabled = true;
                mrEnabled = true;
                try { light.enabled = true; } catch (Exception) { }
            }
            
        }
    }

    bool isInView()
    {
        if (GlobalInformation.player)
        {
            if (Vector3.Distance(groundCursor.position, transform.position) > 50)
            {
                Vector3 targetDir = transform.position - groundCursor.position;
                float angle = Vector3.Angle(targetDir, groundCursor.forward);
                return angle < mainCam.fieldOfView;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        
    }
}
