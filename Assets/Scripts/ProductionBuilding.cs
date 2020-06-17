using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    public BuildingTypes _type;
    public Tile _tile;
    public float _efficiency;
    public GameManager GM;


    #region Time
    private float timer = 0.0f;
    public float waitTime;
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
        #endregion

    override public void Construct(BuildingTypes bt, Tile t, GameManager gm)
    {
        _efficiency = 1f;
        if (ProductionBuilding.neighborsScaleEfficiency.Contains(bt))
        {
            Tile.TileTypes nt = ProductionBuilding.Neighbor_Type[bt];
            int m1 = ProductionBuilding.Neighbor_minmax[bt].Item1;
            int m2 = ProductionBuilding.Neighbor_minmax[bt].Item2;
            int neighborCount = 0;
            foreach (Tile n in t._neighborTiles) if (n._type == nt) neighborCount++;
            _efficiency = Math.Max(0f, Math.Min(1f, (float)(1 + neighborCount - m1) / (float)(1 + m2 - m1)));//calculates efficiency factor

        }
        waitTime = ProductionBuilding.resourceGeneration[bt] / _efficiency;
        GM = gm;
        _type = bt;
        t._building = this;
        GM._buildings.Add(this);
        //deducting resources used in construction
        GM._resourcesInWarehouse[GameManager.ResourceTypes.Planks] -= Building.cost_plank[bt];
        GM._money -= Building.cost_money[bt];
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
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
            Debug.Assert(outputResource.ContainsKey(_type), "couldn't find an output for " + _type.ToString() + " type of building.", GM);
            Debug.Assert(GM._resourcesInWarehouse.ContainsKey(outputResource[_type]), "couldn't find a space in storage for this type of resource.", GM);
            GM._resourcesInWarehouse[outputResource[_type]] += outputCount[_type];

        }
    }
}

