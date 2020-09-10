using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCollider : MonoBehaviour
{
    public ArrayList colliders = new ArrayList();
    private bool generationAllowed = true;

    //if()

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision detected!");
        if (other.gameObject.CompareTag("ignore"))
        {
            Debug.Log("Trigger enter: " + other.gameObject.name);
            generationAllowed = false;
            colliders.Add(other);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag != "ignore")
        {
            Debug.Log("Trigger Exit: " + other.gameObject.name);
            colliders.Remove(other);
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
            //Debug.Log("no Collision found");
        }
        else
        {
            //Debug.Log("Collision found");
        }
        return generationAllowed;
    }
}
