using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Map generation
    
    #region Tile GameObjects
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
    #endregion

    private Tile[,] _tileMap; //2D array of all spawned tiles
    public Texture2D HeightMap;//2d heightmap, controlls terrain generation
    public readonly float HexDisplace = (float)Math.Sqrt(3) * 0.5f;//constant used to figure out spacing in the corner direction of the Hex tile.
    public readonly float TileRadius = 10f;//span of tile: center to edge
    public float TileMaxHeight = 10f;//maximum displacement Height of Tiles, change to control slope and height of terrain
    //loads the playfield from the Heightmap, initializes the _tileMap
    void loadMap(Texture2D map)
    {
        _tileMap = new Tile[map.width,map.height];
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                //This section determines which Tile to use by its height
                float pixel = map.GetPixel(x, y).maxColorComponent;
                GameObject tile;
                switch (pixel)
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
                //once a Tile has been chosen, the Tile Object is noted in the _tileMap
                //afterwards, it is placed at the correct x,y coordinates, at the given height, and a random facing of the Tile is picked to vary the look of the landscape.
                _tileMap[x, y] = AddTile(tile, x - map.width / 2, y - map.height / 2, pixel * TileMaxHeight, Random.Range(0, 5));
            }
        }


    }
    //This script Loads a single Tile at Position x,y (on the Hexgrid) and height(ingame Coordinates)
    //additionally, it can rotate them by 60° Increments
    Tile AddTile(GameObject tile, int x, int y, float height, int orientation)
    {
        float angle = (float)(orientation % 6) * 60f;
        Quaternion ori = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 pos = new Vector3(x * TileRadius * HexDisplace, height, (y * TileRadius) + (x % 2) * TileRadius * 0.5f);
        Instantiate(tile, pos, ori);
        Tile t = tile.GetComponent<Tile>();
        return t;
    }
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion

    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    

    


    // Start is called before the first frame update
    void Start()
    {
        loadMap(HeightMap);
        PopulateResourceDictionary();
    }
        
    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //TODO: check if building can be placed and then istantiate it

        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();

        //TODO: put all neighbors in the result list

        return result;
    }
    #endregion


   

   
}
