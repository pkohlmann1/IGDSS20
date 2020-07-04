using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBaker : MonoBehaviour
{

    [SerializeField]
    NavMeshSurface[] navMeshSurfaces;
    // Start is called before the first frame update

    Boolean meshBuild = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!meshBuild)
        {
            navMeshSurfaces = GameObject.FindObjectsOfType<NavMeshSurface>();

            UnityEngine.Debug.Log(navMeshSurfaces.Length);
            if(navMeshSurfaces.Length == 256)
            {
                for (int i = 0; i < navMeshSurfaces.Length; i++)
                {
                    navMeshSurfaces[i].BuildNavMesh();
                }
                   meshBuild = true;
            }
  
        }

    }
}
