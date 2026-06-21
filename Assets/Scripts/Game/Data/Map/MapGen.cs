using System.Collections.Generic;
using UnityEngine;

public abstract class MapGen : MonoBehaviour
{
    public MapBiomeSO biomeData;
    public MapChunkSO mapData;
    public bool IsDone = false;
    public virtual void GenerateMap()
    {

    }

    #region Initalize and Extend
    public List<List<SidewaysTile>> Tiles;

    public virtual void Initalize(int w, int h)
    {
        Debug.Log("[MapGeneration] Initialize map size " + w + "," + h);
        Tiles = new List<List<SidewaysTile>>();
        for (int iX = 0; iX < w; iX++)
        {
            List<SidewaysTile> row = new List<SidewaysTile>();
            for (int iY = 0; iY < h; iY++)
            {
                row.Add(new SidewaysTile( biomeData.Ground , 0));
            }
            Tiles.Add(row);
        }
    }
    #endregion
    #region Size
    public virtual int GetWidth()
    {
        return Tiles.Count;
    }
    public virtual int GetHeight()
    {
        return Tiles[0].Count;
    }
    #endregion
    public void SetTile(int x, int y, SidewaysTile tile)
    {
        Tiles[y][x] = tile;
    }
    #region Output
    public virtual MapData OutputToMapData()
    {
        MapData generatedMap = new MapData();

        //generate elevation data;
        generatedMap.biome = biomeData;
        generatedMap.elevation_data = biomeData.Elevations;
        generatedMap.InitElevationData();

        //generate tiles
        generatedMap.tile_data = new SidewaysTile[GetHeight(), GetWidth()];
        for (int iY = 0; iY < generatedMap.tile_data.GetLength(0); iY++)
        {
            for (int iX = 0; iX < generatedMap.tile_data.GetLength(1); iX++)
            {
                generatedMap.tile_data[iY, iX] = new SidewaysTile(Tiles[iY][iX].elevation, Tiles[iY][iX].Variation);
            }
        }
        generatedMap.object_data = GenObj;
        return generatedMap;
    }
    #endregion
    #region Prepare Objects
    public ObjectData[] GenObj;
    public ObjectData GenerateObjectFromData(ObjectDataSO objectData, Vector2Int location)
    {
        if (objectData != null)
        {
            Debug.Log("[MapGenObj] Generating object " + objectData.name);
            ObjectData GenObj = new ObjectData();
            GenObj.data = objectData;
            GenObj.GridLocation = location;
            return GenObj;
        }
        return null;
    }
    #endregion
}