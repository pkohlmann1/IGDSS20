using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingTypes _type;
    public Tile _tile;
    public float _efficiency;
    public GameManager GM;


    #region Time
    private float timer = 0.0f;
    public float waitTime;
    #endregion

    #region enumeration
    public enum BuildingTypes {Empty,Fishery, Lumberjack, Sawmill, Sheep_Farm, Framework_Knitters, Potato_Farm, Schnapps_Distillery};
    #endregion

    #region dictionaries
    public static Dictionary<BuildingTypes, float> resourceGeneration = new Dictionary<BuildingTypes, float>() {
        {BuildingTypes.Fishery, 30f},
        {BuildingTypes.Lumberjack,15f },
        {BuildingTypes.Sawmill,15f },
        {BuildingTypes.Sheep_Farm,30f },
        {BuildingTypes.Framework_Knitters,30f },
        {BuildingTypes.Potato_Farm,30f },
        {BuildingTypes.Schnapps_Distillery,30f } };
    public static Dictionary<BuildingTypes, float> outputCount = new Dictionary<BuildingTypes, float>(){
        {BuildingTypes.Fishery,1f },
        { BuildingTypes.Lumberjack, 1f },
        { BuildingTypes.Sawmill, 2f },
        {BuildingTypes.Sheep_Farm,1f },
        {BuildingTypes.Framework_Knitters,1f },
        {BuildingTypes.Potato_Farm,1f },
        {BuildingTypes.Schnapps_Distillery,1f }};
    public static Dictionary<BuildingTypes, Tuple<int, int>> Neighbor_minmax = new Dictionary<BuildingTypes, Tuple<int, int>>() { 
        {BuildingTypes.Fishery,new Tuple<int, int>(1,3) }, 
        { BuildingTypes.Lumberjack, new Tuple<int, int>(1, 6) }, 
        { BuildingTypes.Sheep_Farm, new Tuple<int, int>(1, 4) }, 
        { BuildingTypes.Potato_Farm, new Tuple<int, int>(1, 4) } };
    public static Dictionary<BuildingTypes, Tile.TileTypes> Neighbor_Type = new Dictionary<BuildingTypes, Tile.TileTypes>() { 
        {BuildingTypes.Fishery,Tile.TileTypes.Water },
        {BuildingTypes.Lumberjack,Tile.TileTypes.Forest },
        {BuildingTypes.Sheep_Farm,Tile.TileTypes.Grass },
        {BuildingTypes.Potato_Farm,Tile.TileTypes.Grass }  };
    public static Dictionary<BuildingTypes, HashSet<Tile.TileTypes>> TileOptions = new Dictionary<BuildingTypes, HashSet<Tile.TileTypes>>() { 
        {BuildingTypes.Fishery,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Sand } }, 
        { BuildingTypes.Lumberjack, new HashSet<Tile.TileTypes>() { Tile.TileTypes.Forest } },
        {BuildingTypes.Sawmill,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } },
        {BuildingTypes.Sheep_Farm,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass} },
        {BuildingTypes.Framework_Knitters,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } },
        {BuildingTypes.Potato_Farm,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass} },
        {BuildingTypes.Schnapps_Distillery,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } }};
    public static Dictionary<BuildingTypes, HashSet<GameManager.ResourceTypes>> inputResources = new Dictionary<BuildingTypes, HashSet<GameManager.ResourceTypes>>() {
        {BuildingTypes.Sawmill,new HashSet<GameManager.ResourceTypes>(){GameManager.ResourceTypes.Wood } }, 
        {BuildingTypes.Framework_Knitters,new HashSet<GameManager.ResourceTypes>(){GameManager.ResourceTypes.Wool } },
        {BuildingTypes.Schnapps_Distillery ,new HashSet<GameManager.ResourceTypes>(){GameManager.ResourceTypes.Potato } } };
    public static Dictionary<BuildingTypes, GameManager.ResourceTypes> outputResource = new Dictionary<BuildingTypes, GameManager.ResourceTypes>() { 
        { BuildingTypes.Fishery,GameManager.ResourceTypes.Fish }, 
        { BuildingTypes.Lumberjack, GameManager.ResourceTypes.Wood }, 
        { BuildingTypes.Sawmill, GameManager.ResourceTypes.Planks }, 
        { BuildingTypes.Sheep_Farm, GameManager.ResourceTypes.Wool }, 
        { BuildingTypes.Framework_Knitters, GameManager.ResourceTypes.Clothes}, 
        { BuildingTypes.Potato_Farm,GameManager.ResourceTypes.Potato }, 
        {BuildingTypes.Schnapps_Distillery,GameManager.ResourceTypes.Schnapps }, };
    public static Dictionary<BuildingTypes, float> upkeep = new Dictionary<BuildingTypes, float>() { 
        {BuildingTypes.Fishery,40f }, 
        { BuildingTypes.Lumberjack, 10f }, 
        { BuildingTypes.Sawmill, 10f },
        {BuildingTypes.Sheep_Farm,20f },
        {BuildingTypes.Framework_Knitters,50f },
        {BuildingTypes.Potato_Farm,20f },
        {BuildingTypes.Schnapps_Distillery,40f }};
    public static Dictionary<BuildingTypes, float> cost_money = new Dictionary<BuildingTypes, float>() {
        {BuildingTypes.Fishery,100f },
        {BuildingTypes.Lumberjack,100f },
        {BuildingTypes.Sawmill,100f },
        {BuildingTypes.Sheep_Farm,100f },
        {BuildingTypes.Framework_Knitters,100f },
        {BuildingTypes.Potato_Farm,100f },
        {BuildingTypes.Schnapps_Distillery,100f } };
    public static Dictionary<BuildingTypes, float> cost_plank = new Dictionary<BuildingTypes, float>() { 
        {BuildingTypes.Fishery,2f }, 
        { BuildingTypes.Lumberjack, 0f }, 
        { BuildingTypes.Sawmill, 0f },
        {BuildingTypes.Sheep_Farm,2f }, 
        {BuildingTypes.Framework_Knitters,2f }, 
        {BuildingTypes.Potato_Farm,2f }, 
        {BuildingTypes.Schnapps_Distillery,2f }};
    public static HashSet<BuildingTypes> neighborsScaleEfficiency = new HashSet<BuildingTypes>() { BuildingTypes.Fishery, BuildingTypes.Lumberjack, BuildingTypes.Sheep_Farm, BuildingTypes.Potato_Farm };
    #endregion

    public void Construct(BuildingTypes bt, Tile t, GameManager gm)
    {
        _efficiency = 1f;
        if (Building.neighborsScaleEfficiency.Contains(bt))
        {
            Tile.TileTypes nt = Building.Neighbor_Type[bt];
            int m1 = Building.Neighbor_minmax[bt].Item1;
            int m2 = Building.Neighbor_minmax[bt].Item2;
            int neighborCount = 0;
            foreach (Tile n in t._neighborTiles) if (n._type == nt) neighborCount++;
            _efficiency = Math.Max(0f, Math.Min(1f, (float)(1 + neighborCount - m1) / (float)(1 + m2 - m1)));//calculates efficiency factor

        }
        waitTime = Building.resourceGeneration[bt] / _efficiency;
        GM = gm;
        _type = bt;
        t._building = this;
        GM._buildings.Add(this);
        //deducting resources used in construction
        GM._resourcesInWarehouse[GameManager.ResourceTypes.Planks] -= Building.cost_plank[bt];
        GM._money -= Building.cost_money[bt];
    }

    public static bool Constructable(BuildingTypes bt, Tile t, GameManager GM) 
    {
        bool placeable = true;
        UnityEngine.Debug.Assert(GM._buildingPrefabs.Dict.ContainsKey(bt), "Building Type has no model assigned.", GM);
        UnityEngine.Debug.Assert(t._building is null, "This spot allready has a building.", GM);
        //determines if building is placed on right tile.
        UnityEngine.Debug.Assert(Building.TileOptions.ContainsKey(bt), "Building has no tile type assigned.", GM);
        UnityEngine.Debug.Assert(Building.TileOptions[bt].Contains(t._type), "Building can't be placed on this tile type.", GM);
        UnityEngine.Debug.Assert(GM._resourcesInWarehouse[GameManager.ResourceTypes.Planks] >= Building.cost_plank[bt], "Not enough planks.", GM);
        UnityEngine.Debug.Assert(GM._money >= Building.cost_money[bt], "Not enough money.", GM);
        if (Building.neighborsScaleEfficiency.Contains(bt)) {
            Tile.TileTypes nt = Building.Neighbor_Type[bt];
            int m1 = Building.Neighbor_minmax[bt].Item1;
            int neighborCount = 0;
            foreach (Tile n in t._neighborTiles) if (n._type == nt) neighborCount++;
            UnityEngine.Debug.Assert(neighborCount >= m1, "Not enough neighbors of the correct type.", GM);
            placeable = placeable && (neighborCount >= m1);
        }
        placeable = placeable && (t._building is null) && (Building.TileOptions.ContainsKey(bt)) && (Building.TileOptions[bt].Contains(t._type)) && (GM._resourcesInWarehouse[GameManager.ResourceTypes.Planks] >= Building.cost_plank[bt]) && (GM._money >= Building.cost_money[bt]);
        return placeable;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > waitTime) 
        {
            timer = timer - waitTime;
            produce();
        }
    }

    void produce() 
    {
        bool resource_available = true;
        bool needs_resource = inputResources.ContainsKey(_type);
        if (needs_resource)
        {
            foreach (GameManager.ResourceTypes res in inputResources[_type]) Debug.Assert(GM._resourcesInWarehouse.ContainsKey(res), "Required resource type is not in Store.", this);
            foreach (GameManager.ResourceTypes res in inputResources[_type]) if (GM._resourcesInWarehouse[res] < 1f) resource_available = false;
            Debug.Assert(resource_available, "Input resources weren't available.", this);
        }
        if (resource_available) 
        {
            if (needs_resource)
            {
                foreach (GameManager.ResourceTypes res in inputResources[_type]) GM._resourcesInWarehouse[res]--;
            }
            Debug.Assert(outputResource.ContainsKey(_type),"couldn't find an output for "+ _type.ToString()+" type of building.", GM);
            Debug.Assert(GM._resourcesInWarehouse.ContainsKey(outputResource[_type]),"couldn't find a space in storage for this type of resource.",GM);
            GM._resourcesInWarehouse[outputResource[_type]] += outputCount[_type];

        }
    }

}

