using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Config/Map")]
public class MapData
    : ScriptableObject
{
    public string world;
    public string desc;
    public string version;

    public int size = 10;
    public int[,] ElevationMatrix;
    public int[,] TerrainMatrix;
    public string[] StoredTerrain;

    //public BaseData[] storedEntities;
    public string neutralFaction;
    public PlayerData[] players;

    private void OnValidate()
    {
        ElevationMatrix = new int[size, size];
        TerrainMatrix = new int[size, size];
    }
}
