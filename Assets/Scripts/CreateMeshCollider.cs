using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMeshCollider : MonoBehaviour
{
    Plane[] planes;
    BoxCollider bc;
    Vector3[] corners;
    Camera cam;
    private void Start()
    {
        bc = GetComponent<BoxCollider>();
        cam = Camera.main;
        corners = new Vector3[4];

        cam.CalculateFrustumCorners(new Rect(10,10,10,10), cam.depth, Camera.main.stereoActiveEye, corners);

        Instantiate(new GameObject(), corners[0], Quaternion.identity);
        Instantiate(new GameObject(), corners[1], Quaternion.identity);
        Instantiate(new GameObject(), corners[2], Quaternion.identity);
        Instantiate(new GameObject(), corners[3], Quaternion.identity);

        Debug.DrawLine(corners[0], corners[1], Color.blue);
        Debug.DrawLine(corners[1], corners[3], Color.blue);
        Debug.DrawLine(corners[3], corners[2], Color.blue);
        Debug.DrawLine(corners[2], corners[0], Color.blue);
    }
}
