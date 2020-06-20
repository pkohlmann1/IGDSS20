
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    public int _workerCount;
    public float _happiness;
    #endregion

    #region enumeration
    public enum BuildingTypes { Empty, Fishery, Lumberjack, Sawmill, Sheep_Farm, Framework_Knitters, Potato_Farm, Schnapps_Distillery, Farm_House };
    #endregion

    #region Referentials
    public BuildingTypes _type;
    public Tile _tile;

    #region Manager References
    public JobManager _jobManager; //Reference to the JobManager
    public GameManager GM;
    #endregion

    #endregion

    #region dictionaries
    public static Dictionary<BuildingTypes, HashSet<Tile.TileTypes>> TileOptions = new Dictionary<BuildingTypes, HashSet<Tile.TileTypes>>() {
        {BuildingTypes.Fishery,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Sand } },
        { BuildingTypes.Lumberjack, new HashSet<Tile.TileTypes>() { Tile.TileTypes.Forest } },
        {BuildingTypes.Sawmill,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } },
        {BuildingTypes.Sheep_Farm,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass} },
        {BuildingTypes.Framework_Knitters,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } },
        {BuildingTypes.Potato_Farm,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass} },
        {BuildingTypes.Schnapps_Distillery,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } },
        {BuildingTypes.Farm_House,new HashSet<Tile.TileTypes>() {Tile.TileTypes.Grass, Tile.TileTypes.Forest, Tile.TileTypes.Stone } }};
    public static Dictionary<BuildingTypes, Type> buildingClassType = new Dictionary<BuildingTypes, Type>() {
        {BuildingTypes.Empty,typeof(Building)},
        { BuildingTypes.Farm_House, typeof(HousingBuilding) },
        { BuildingTypes.Fishery, typeof(ProductionBuilding) },
        { BuildingTypes.Framework_Knitters, typeof(ProductionBuilding) },
        { BuildingTypes.Lumberjack, typeof(ProductionBuilding) },
        { BuildingTypes.Potato_Farm, typeof(ProductionBuilding) },
        { BuildingTypes.Sawmill, typeof(ProductionBuilding) },
        { BuildingTypes.Schnapps_Distillery, typeof(ProductionBuilding) },
        { BuildingTypes.Sheep_Farm, typeof(ProductionBuilding) }};
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
    public static Dictionary<BuildingTypes, float> cost_money = new Dictionary<BuildingTypes, float>() {
        {BuildingTypes.Fishery,100f },
        {BuildingTypes.Lumberjack,100f },
        {BuildingTypes.Sawmill,100f },
        {BuildingTypes.Sheep_Farm,100f },
        {BuildingTypes.Framework_Knitters,100f },
        {BuildingTypes.Potato_Farm,100f },
        {BuildingTypes.Schnapps_Distillery,100f },
        {BuildingTypes.Farm_House,100f } };
    public static Dictionary<BuildingTypes, float> cost_plank = new Dictionary<BuildingTypes, float>() {
        {BuildingTypes.Fishery,2f },
        { BuildingTypes.Lumberjack, 0f },
        { BuildingTypes.Sawmill, 0f },
        {BuildingTypes.Sheep_Farm,2f },
        {BuildingTypes.Framework_Knitters,2f },
        {BuildingTypes.Potato_Farm,2f },
        {BuildingTypes.Schnapps_Distillery,2f },
        {BuildingTypes.Farm_House,0f }};
    public static Dictionary<BuildingTypes, float> upkeep = new Dictionary<BuildingTypes, float>() {
        {BuildingTypes.Fishery,40f },
        { BuildingTypes.Lumberjack, 10f },
        { BuildingTypes.Sawmill, 10f },
        {BuildingTypes.Sheep_Farm,20f },
        {BuildingTypes.Framework_Knitters,50f },
        {BuildingTypes.Potato_Farm,20f },
        {BuildingTypes.Schnapps_Distillery,40f },
        {BuildingTypes.Farm_House,0f }
    };
    public static Dictionary<BuildingTypes, int> workerCapacity = new Dictionary<BuildingTypes, int>() { 
        { BuildingTypes.Farm_House, 10 }, 
        { BuildingTypes.Fishery, 25}, 
        { BuildingTypes.Lumberjack, 5 }, 
        { BuildingTypes.Sawmill, 10 }, 
        { BuildingTypes.Sheep_Farm, 10 }, 
        { BuildingTypes.Framework_Knitters, 50 }, 
        { BuildingTypes.Potato_Farm, 20 }, 
        { BuildingTypes.Schnapps_Distillery, 20 } };
    public static HashSet<BuildingTypes> neighborsScaleEfficiency = new HashSet<BuildingTypes>() { BuildingTypes.Fishery, BuildingTypes.Lumberjack, BuildingTypes.Sheep_Farm, BuildingTypes.Potato_Farm };
    #endregion


    #region Methods   
    public abstract void Construct(BuildingTypes bt, Tile t, GameManager gm);

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
        if (Building.neighborsScaleEfficiency.Contains(bt))
        {
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

    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    public abstract int WorkerCount();
    public float WorkerHappiness() 
    {
        float happiness = 0;
        foreach (Worker w in _workers) if (w.gameObject.activeInHierarchy) happiness += w._happiness;
        return happiness/_workerCount;
    }

    protected void Update()
    {
        _workerCount = WorkerCount();
        _happiness = WorkerHappiness();
    }
    #endregion


}