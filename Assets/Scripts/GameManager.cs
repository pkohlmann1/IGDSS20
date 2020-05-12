using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public GameObject OceanTile;
    public GameObject BSandTile;
    public GameObject MSandTile;
    public GameObject TSandTile;
    public GameObject BGrassTile;
    public GameObject MGrassTile;
    public GameObject TGrassTile;
    public GameObject BForestTile;
    public GameObject MForestTile;
    public GameObject TForestTile;
    public GameObject BStoneTile;
    public GameObject MStoneTile;
    public GameObject TStoneTile;
    public GameObject BMountainTile;
    public GameObject MMountainTile;
    public GameObject TMountainTile;

    public Texture2D HeightMap;



    private float HexDisplace =(float)Math.Sqrt(3)*0.5f;
    private float TileRadius = 10f;
    private float TileMaxHeight = 10f;

    // Start is called before the first frame update
    void Start()
    {
        loadMap(HeightMap);
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }

    void AddTile(GameObject tile,int x,int y, float height)
    {
        Vector3 pos = new Vector3(x*TileRadius*HexDisplace, height,y*TileRadius + (x%2)*TileRadius*0.5f);
        Instantiate(tile,pos,Quaternion.identity);

    }

    void loadMap(Texture2D map)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                float pixel = map.GetPixel(x, y).maxColorComponent;
                GameObject tile;
                switch(pixel)
                {
                    case float p when p > 0.95:
                        tile = TMountainTile;
                        break;
                    case float p when p > 0.85:
                        tile = MMountainTile;
                        break;
                    case float p when p > 0.8:
                        tile = BMountainTile;
                        break;
                    case float p when p > 0.75:
                        tile = TStoneTile;
                        break;
                    case float p when p > 0.65:
                        tile = MStoneTile;
                        break;
                    case float p when p > 0.6:
                        tile = BStoneTile;
                        break;
                    case float p when p > 0.55:
                        tile = TForestTile;
                        break;
                    case float p when p > 0.45:
                        tile = MForestTile;
                        break;
                    case float p when p > 0.4:
                        tile = BForestTile;
                        break;
                    case float p when p > 0.35:
                        tile = TGrassTile;
                        break;
                    case float p when p > 0.25:
                        tile = MGrassTile;
                        break;
                    case float p when p > 0.2:
                        tile = BGrassTile;
                        break;
                    case float p when p > 0.15:
                        tile = TSandTile;
                        break;
                    case float p when p > 0.05:
                        tile = MSandTile;
                        break;
                    case float p when p > 0.0:
                        tile = BSandTile;
                        break;

                    default:
                        tile = OceanTile;
                        break;
                }


                AddTile(tile, x-map.width/2, y-map.height/2, pixel * TileMaxHeight);
            }
        }

        
    }
}
