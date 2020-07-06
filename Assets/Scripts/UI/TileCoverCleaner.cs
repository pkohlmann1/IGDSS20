using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCoverCleaner : MonoBehaviour
{
    public List<GameObject> cover;
    // Start is called before the first frame update
    void Start()
    {
        cover = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.layer == 8)
            {
                cover.Add(child.gameObject);
            }
        }
    }

    public void setCoverVisible()
    {
        foreach (GameObject child in cover)
        { 
            MeshRenderer render = child.GetComponent<MeshRenderer>();
            render.enabled = true;
        }
    }

    public void setCoverInvisible()
    {
        foreach (GameObject child in cover)
        {
            MeshRenderer render = child.GetComponent<MeshRenderer>();
            render.enabled = false;
        }
    }

}
