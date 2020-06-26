using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetColliders : MonoBehaviour
{
    public ArrayList colliders = new ArrayList();
    public bool generationAllowed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "ignore")
        {
            Debug.Log("Collision enter: " + collision.collider.name);
            generationAllowed = false;
            colliders.Add(collision.collider);
        }   
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.tag != "ignore")
        {
            Debug.Log("Collision exit: " + collision.collider.name);
            colliders.Remove(collision.collider);
            if(colliders.Count == 0)
            {
                generationAllowed = true;
            }
        }
    }
    public bool ask()
    {
        if (generationAllowed)
        {
            Debug.Log("no Collision found");
        }
        else
        {
            Debug.Log("Collision found");
        }
        return generationAllowed;
    }
}
