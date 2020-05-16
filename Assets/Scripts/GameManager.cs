using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //Tiles for Map Generation, B,M and T stand for Bottom, Middle and Top.
    //Top and Bottom Tiles will be used at the border between Tiles to smooth over Biome edges.
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
    //This is your Terrain Map, it'll be used to load the Terrain.
    //the generator and other scripts will look up the size of your map, scaling your playfield and number of tiles
    public Texture2D HeightMap;


    //constant to help with Hex Geometry. 
    //Is used to figure out spacing in the corner direction of the Hex tile.
    public readonly float HexDisplace =(float)Math.Sqrt(3)*0.5f;
    //Variables to tell the generator the dimensions and the displacement range of the Tiles
    public float TileRadius = 10f;
    public float TileMaxHeight = 10f;

    // Start is called before the first frame update
    void Start()
    {
        loadMap(HeightMap);
    }
        
    // Update is called once per frame
    void Update()
    {
        
    }

    //This script Loads a single Tile at Position x,y (on the Hexgrid) and height(ingame Coordinates)
    //additionally, it can rotate them by 60° Increments
    void AddTile(GameObject tile,int x,int y, float height,int orientation)
    {
        float angle = (float)(orientation % 6) * 60f;
        Quaternion ori = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 pos = new Vector3(x * TileRadius * HexDisplace, height, (y * TileRadius) + (x % 2) * TileRadius * 0.5f);
        Instantiate(tile,pos,ori);

    }

    //loads the playfield from the Heightmap
    void loadMap(Texture2D map)
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                //This section determines which Tile to use by its height
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
                //once a Tile has been chosen, it is placed at the correct x,y coordinates, at the given height, and a random facing of the Tile is picked to vary the look of the landscape.
                AddTile(tile, x-map.width/2, y-map.height/2, pixel * TileMaxHeight, Random.Range(0, 5));
            }
        }

        
    }
}
