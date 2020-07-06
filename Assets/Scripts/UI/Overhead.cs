using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overhead : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        
    }
    // This script will make a GUITexture follow a transform.
    

    void Update()
    {
        Vector3 wantedPos = Camera.main.WorldToScreenPoint(target.position);
        transform.position = wantedPos;
    }
}
