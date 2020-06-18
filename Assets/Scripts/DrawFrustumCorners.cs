using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFrustumCorners : MonoBehaviour
{
    public Camera cam;
    private Transform ct;

    public float upperDistance = 8.5f;
    public float lowerDistance = 12.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!cam)
        {
            cam = Camera.main;
        }
        ct = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        FindUpperCorners();
        FindLowerCorners();
    }

    private void FindUpperCorners()
    {
        Vector3[] corners = GetCorners(upperDistance);

        Debug.DrawLine(corners[0], corners[1], Color.yellow);
        Debug.DrawLine(corners[1], corners[3], Color.yellow);
        Debug.DrawLine(corners[3], corners[2], Color.yellow);
        Debug.DrawLine(corners[2], corners[0], Color.yellow);
    }
    private void FindLowerCorners()
    {
        Vector3[] corners = GetCorners(lowerDistance);

        Debug.DrawLine(corners[0], corners[1], Color.red);
        Debug.DrawLine(corners[1], corners[3], Color.red);
        Debug.DrawLine(corners[3], corners[2], Color.red);
        Debug.DrawLine(corners[2], corners[0], Color.red);
    }
    private Vector3[] GetCorners(float distance)
    {
        Vector3[] corners = new Vector3[4];
        float halfFOV = (cam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = cam.aspect;

        float height = distance * Mathf.Tan(Convert.ToSingle(halfFOV));
        float width = height * aspect;

        corners[0]  = ct.position - (ct.right * width);
        corners[0] += ct.up * height;
        corners[0] += ct.forward * distance;

        corners[1]  = ct.position + (ct.right * width);
        corners[1] += ct.up * height;
        corners[1] += ct.forward * distance;

        corners[2]  = ct.position - (ct.right * width);
        corners[2] -= ct.up * height;
        corners[2] += ct.forward * distance;

        corners[3]  = ct.position + (ct.right * width);
        corners[3] -= ct.up * height;
        corners[3] += ct.forward * distance;

        return corners;

    }
}
