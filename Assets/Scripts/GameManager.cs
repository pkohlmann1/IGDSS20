using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
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
        //first pass to load tiles
        _tileMap = new Tile[map.width, map.height];
        int xCenter = map.width / 2;
        int yCenter = map.height / 2;

        for (int x = 0; x < map.width; x++) for (int y = 0; y < map.height; y++)                 
            {
                float pixel = map.GetPixel(x, y).maxColorComponent;
                Tile t = AddTile( x - xCenter, y - yCenter, pixel, Random.Range(0, 5));
                t._coordinateHeight = y;
                t._coordinateWidth = x;
                _tileMap[x, y] = t;
            }
        
        //second pass to assign neighbor tiles
        for (int x = 0; x < map.width; x++) for (int y = 0; y < map.height; y++)
            {
                Tile t = _tileMap[x, y];
                t._neighborTiles = FindNeighborsOfTile(t);
            }
        
    }
    //This script Loads a single Tile at Position x,y (on the Hexgrid) and height(scaled by TileMaxHeight)
    //additionally, it can rotate them by 60° Increments
    Tile AddTile( int x, int y, float pixel, int orientation)
    {
        //This section determines which Tile to use by its height
        Tile.TileTypes tp;
        GameObject tile;
        switch (pixel)
        {
            case float p when p > 0.95:
                tile = TMountainTile;
                tp = Tile.TileTypes.Mountain;
                break;
            case float p when p > 0.85:
                tile = MMountainTile;
                tp = Tile.TileTypes.Mountain;
                break;
            case float p when p > 0.8:
                tile = BMountainTile;
                tp = Tile.TileTypes.Mountain;
                break;
            case float p when p > 0.75:
                tile = TStoneTile;
                tp = Tile.TileTypes.Stone;
                break;
            case float p when p > 0.65:
                tile = MStoneTile;
                tp = Tile.TileTypes.Stone;
                break;
            case float p when p > 0.6:
                tile = BStoneTile;
                tp = Tile.TileTypes.Stone;
                break;
            case float p when p > 0.55:
                tile = TForestTile;
                tp = Tile.TileTypes.Forest;
                break;
            case float p when p > 0.45:
                tile = MForestTile;
                tp = Tile.TileTypes.Forest;
                break;
            case float p when p > 0.4:
                tile = BForestTile;
                tp = Tile.TileTypes.Forest;
                break;
            case float p when p > 0.35:
                tile = TGrassTile;
                tp = Tile.TileTypes.Grass;
                break;
            case float p when p > 0.25:
                tile = MGrassTile;
                tp = Tile.TileTypes.Grass;
                break;
            case float p when p > 0.2:
                tile = BGrassTile;
                tp = Tile.TileTypes.Grass;
                break;
            case float p when p > 0.15:
                tile = TSandTile;
                tp = Tile.TileTypes.Sand;
                break;
            case float p when p > 0.05:
                tile = MSandTile;
                tp = Tile.TileTypes.Sand;
                break;
            case float p when p > 0.0:
                tile = BSandTile;
                tp = Tile.TileTypes.Sand;
                break;

            default:
                tile = OceanTile;
                tp = Tile.TileTypes.Water;
                break;
        }

        float angle = (float)(orientation % 6) * 60f;
        float height = pixel * TileMaxHeight;
        Quaternion ori = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 pos = new Vector3(x * TileRadius * HexDisplace, height, (y * TileRadius) + ((x*x) % 2) * TileRadius * 0.5f);
        //foreach(Tile ts in tile.GetComponents<Tile>()) { GameObject.DestroyImmediate(ts,true); }
        GameObject temp = Instantiate(tile, pos, ori);
        //adding TileCover Cleaner Component
        if (temp.GetComponent<TileCoverCleaner>() == null) temp.AddComponent(typeof(TileCoverCleaner));
        //adding Tile Component
        Tile t = temp.GetComponent<Tile>();
        if (t==null) t = temp.AddComponent(typeof(Tile)) as Tile;
        t._type = tp;
        return t;
    }
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion

    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType
    public float _money = 20f;
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
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 2);
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
            _selectedBuildingIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingIndex = 9;
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
        Tile t = _tileMap[width,height];
        try
        { PlaceBuildingOnTile(t); }
        catch(Exception e)
        {
            UnityEngine.Debug.LogException(e, this);
        }
        
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        GameObject selectedBuilding = _buildingPrefabs[_selectedBuildingIndex];
        //if there is building prefab for the number input
        if (_selectedBuildingIndex < _buildingPrefabs.Length)
        {
            bool placeable = true;
            Building.BuildingTypes bt = Building.BuildingTypes.Empty;
            //TODO: check if building can be placed and then istantiate it
            switch (selectedBuilding.tag) {
                case "Lumberjack":
                    bt = Building.BuildingTypes.Lumberjack;
                    break;

                case "Fishery":
                    bt = Building.BuildingTypes.Fishery;
                    break;

                case "SheepFarm":
                    bt = Building.BuildingTypes.Sheep_Farm;
                    break;

                case "PotatoFarm":
                    bt = Building.BuildingTypes.Potato_Farm;
                    break;

                case "Sawmill":
                    bt = Building.BuildingTypes.Sawmill;
                    break;

                case "FrameworkKnitter":
                    bt = Building.BuildingTypes.Framework_Knitters;
                    break;

                case "Distillery":
                    bt = Building.BuildingTypes.Schnapps_Distillery;
                    break;

                default:
                    bt = Building.BuildingTypes.Empty;
                    break;
            }
            UnityEngine.Debug.Assert(t._building is null, "This spot allready has a building.", this);
            placeable = placeable && (t._building is null);

            //determines if building is placed on right tile.
            UnityEngine.Debug.Assert(Building.TileOptions.ContainsKey(bt),"Building has no tile type assigned.",this);
            placeable = placeable && Building.TileOptions.ContainsKey(bt);
            UnityEngine.Debug.Assert(Building.TileOptions[bt].Contains(t._type),"Building can't be placed on this tile type.",this);
            placeable = placeable && Building.TileOptions[bt].Contains(t._type);

            float efficiency = 1f;
            //determines both if the building has enough neighbortiles of a certain type and how efficient the resulting building is
            if (Building.neighborsScaleEfficiency.Contains(bt))
            {
                Tile.TileTypes nt = Building.Neighbor_Type[bt];
                int m1 = Building.Neighbor_minmax[bt].Item1;
                int m2 = Building.Neighbor_minmax[bt].Item2;
                int neighborCount = 0;
                foreach (Tile n in t._neighborTiles) if (n._type == nt) neighborCount++;
                UnityEngine.Debug.Assert(neighborCount >= m1, "Not enough neighbors of the correct type.", this);
                if (neighborCount < m1) placeable = false;
                else
                {
                    efficiency = efficiency+(((Building.NeighborMaxScaleFactor-efficiency) / (float)(m2 - m1)) * (Math.Min(neighborCount, m2) - m1));//calculates efficiency factor
                }
            }
            UnityEngine.Debug.Assert(_resourcesInWarehouse[ResourceTypes.Planks] >= Building.cost_plank[bt], "Not enough planks.",this);
            placeable = placeable && (_resourcesInWarehouse[ResourceTypes.Planks] >= Building.cost_plank[bt]);//looks up if building has all the construction materials
            UnityEngine.Debug.Assert(_money >= Building.cost_money[bt],"Not enough money.", this);
            placeable = placeable && (_money >= Building.cost_money[bt]);//looks up if building can be afforded




            if (placeable) 
            {
                TileCoverCleaner clean = t.gameObject.GetComponent<TileCoverCleaner>();
                clean.setCoverInvisible();
                GameObject temp = Instantiate(selectedBuilding, t.transform.position, t.transform.rotation);
                Building b = temp.GetComponent<Building>();
                if (b == null) b = temp.AddComponent(typeof(Building)) as Building;
                b._type = bt;
                b._efficiency = efficiency;
                t._building = b;
            }
            
        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();
        int x = t._coordinateWidth;
        int y = t._coordinateHeight;
        int xBound = _tileMap.GetLength(0);
        int yBound = _tileMap.GetLength(1);
        if (x < 0 || x >= xBound || y < 0 || y >= yBound) throw new IndexOutOfRangeException("x,y: " + x.ToString() + "," + y.ToString());
        else
        {
            bool phase = x % 2 == 1;
            List<Tuple<int, int>> coords = new List<Tuple<int, int>>();
            coords.Add(new Tuple<int, int>(x - 1, y));
            coords.Add(new Tuple<int, int>(x + 1, y));
            coords.Add(new Tuple<int, int>(x, y + 1));
            coords.Add(new Tuple<int, int>(x, y - 1));

            if (phase)
            {
                coords.Add(new Tuple<int, int>(x + 1, y + 1));
                coords.Add(new Tuple<int, int>(x - 1, y + 1));
            }
            else
            {
                coords.Add(new Tuple<int, int>(x + 1, y - 1));
                coords.Add(new Tuple<int, int>(x - 1, y - 1));
            }

            foreach (Tuple<int, int> coord in coords)
            {
                int xs = coord.Item1;
                int ys = coord.Item2;
                if ((xs >= 0 && xs < xBound) && (ys >= 0 && ys < yBound))
                {
                    result.Add(_tileMap[xs, ys]);
                }
            }
        }
        return result;
    }
    #endregion


   

   
}
