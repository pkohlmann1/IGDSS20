﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Attributes
    public TileTypes _type; //The type of the tile
    public Building _building; //The building on this tile
    public List<Tile> _neighborTiles; //List of all surrounding tiles. Generated by GameManager
    public int _coordinateHeight; //The coordinate on the y-axis on the tile grid (not world coordinates)
    public int _coordinateWidth; //The coordinate on the x-axis on the tile grid (not world coordinates)
    #endregion

    #region Navigation
    public static Dictionary<TileTypes, float> traversalTime = new Dictionary<TileTypes, float>() {
        { TileTypes.Water, 30f },
        { TileTypes.Sand, 2f },
        { TileTypes.Grass, 1f },
        {TileTypes.Forest, 2f}, 
        {TileTypes.Stone, 1f},
        {TileTypes.Mountain, 3f} };
    public Dictionary<Tile, Tile> routeTo;
    public Dictionary<Tile, float> travelTime;

    public Dictionary<Tile, Tile> n_routeTo = new Dictionary<Tile, Tile>();
    public Dictionary<Tile, float> n_travelTime = new Dictionary<Tile, float>();

    public void mergeNewRouting() 
    {
        foreach (KeyValuePair<Tile, Tile> entry in n_routeTo) routeTo[entry.Key]=entry.Value;
        foreach (KeyValuePair<Tile, float> entry in n_travelTime) travelTime[entry.Key] = entry.Value;
        n_routeTo = new Dictionary<Tile, Tile>();
        n_travelTime = new Dictionary<Tile, float>();
    }
    #endregion

    #region Enumerations
    public enum TileTypes { Empty, Water, Sand, Grass, Forest, Stone, Mountain }; //Enumeration of all available tile types. Can be addressed from other scripts by calling Tile.Tiletypes
    #endregion
    //This class acts as a data container and has no functionality
}
