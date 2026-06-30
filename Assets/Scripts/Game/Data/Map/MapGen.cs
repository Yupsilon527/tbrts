using System.Collections.Generic;
using UnityEngine;

public abstract class MapGen : Initializable
{
    public MapBiomeSO biomeData;
    public MapChunkSO mapData;
    public bool IsDone = false;
    public virtual void GenerateMap()
    {

    }

    #region Initalize and Extend
    int width, height;
    public List<SidewaysTile> Tiles;

    public virtual void Initalize(int w, int h)
    {
        Inspect("[MapGeneration] Initialize map size " + w + "," + h);
        width = w;
        height = h;
        Tiles = new List<SidewaysTile>();
        for (int iX = 0; iX < w; iX++)
        {
            for (int iY = 0; iY < h; iY++)
            {
                Tiles.Add(new SidewaysTile(biomeData.Ground, 0));
            }
        }
    }
    #endregion
    #region Size
    public virtual int GetWidth()
    {
        return width;
    }
    public virtual int GetHeight()
    {
        return height;
    }
    #endregion
    public SidewaysTile GetTile(int x, int y)
    {
        int ti = y * width + x;
        Inspect($"Get tile {x} {y}: {ti}");
        return Tiles[ti];
    }
    public void SetTile(int x, int y, SidewaysTile tile)
    {
        int ti = y * width + x;
        Inspect($"Set tile {x} {y}: {ti}");
        Tiles[ti] = tile;
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
                var t = GetTile(iX, iY);
                generatedMap.tile_data[iY, iX] = new SidewaysTile(t.terrain, t.Variation);
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