using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetColliders : MonoBehaviour
{
    public ArrayList colliders = new ArrayList();
    public bool generationAllowed = true;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collider found");
        if(!(collision.collider.tag == "ignore"))
        {
            Debug.Log("Collider found");
            generationAllowed = false;
            colliders.Add(collision.collider);
        }   
    }
}
