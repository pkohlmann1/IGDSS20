using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCoordinate : MonoBehaviour
{
    public GameObject display;
    public Tile _tile;
    public Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = display.GetComponentInChildren<Text>();
        _tile = display.transform.parent.GetComponent<Tile>();
    }

    // Update is called once per frame
    void Update()
    {
        int x = _tile._coordinateWidth;
        int y = _tile._coordinateHeight;
        _text.text = x.ToString()+","+y.ToString();
    }
}
