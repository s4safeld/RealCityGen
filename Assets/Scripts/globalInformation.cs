using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalInformation : MonoBehaviour
{
    public int viewDistance;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
