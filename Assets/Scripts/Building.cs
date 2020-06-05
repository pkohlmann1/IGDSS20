using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingTypes _type;
    public Tile _tile;
    public float _efficiency;
    public static float NeighborMaxScaleFactor = 2f;
    

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


}
