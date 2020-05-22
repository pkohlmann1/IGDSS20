using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingTypes _type;
    public Tile _tile;
    public float upkeep;
    public float cost_money;
    public float cost_plank;

    public float resourceGeneration;
    public float outputCount;
    public float efficiency;
    public HashSet<Tile.TileTypes> TileOptions;
    public bool neighborsScaleEfficiency;
    public int minNeighbors;
    public int maxNeighbors;
    public HashSet<GameManager.ResourceTypes> inputResources;
    public GameManager.ResourceTypes outputResource;

    #region enumeration
    public enum BuildingTypes {Fishery, Lumberjack, Sawmill, Sheep_Farm, Framework_Knitters, Potato_Farm, Schnapps_Distillery};
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
