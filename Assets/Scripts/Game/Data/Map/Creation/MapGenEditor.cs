using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapGenEditor : MapGen
{
    public Vector2Int dims = Vector2Int.zero;
    public override int GetWidth()
    {
        return dims.x;
    }
    public override int GetHeight()
    {
        return dims.y;
    }

    #region Editor Stuff
#if UNITY_EDITOR
    public GameObject predefinedTilePrefab;

    public void Regenerate()
    {
        List<MapGenPredefinedTile> existingTiles = new();
        existingTiles.AddRange(GetComponentsInChildren<MapGenPredefinedTile>());

        Vector2Int mins = Vector2Int.one * -1;
        foreach (var tile in existingTiles)
        {
            if (tile.gridPos.x >= dims.x || tile.gridPos.y >= dims.y)
            {
                GameObject.DestroyImmediate(tile.gameObject);
            }
            else
            {
                mins.x = Mathf.Max(mins.x, tile.gridPos.x);
                mins.y = Mathf.Max(mins.y, tile.gridPos.y);
            }
        }
        if (mins.y < dims.y || mins.x < dims.x)
        {
            for (int y = 0; y < dims.y; y++)
            {
                for (int x = (y > mins.y ? 0 : (mins.x+1)); x < dims.x; x++)
                {
                    var newTile = (GameObject)PrefabUtility.InstantiatePrefab(predefinedTilePrefab);
                    newTile.transform.parent = transform;
                    newTile.transform.position = SidewaysMap.TranslateGridPosition(new Vector2Int(x, dims.y - y - 1),dims.y);
                    existingTiles.Add(newTile.GetComponentInChildren<MapGenPredefinedTile>());
                    Debug.Log($"Make tile at {x} {y}");

                }
            }
            }

            foreach (var tile in existingTiles)
            {
                if (tile != null)
                {
                    tile.elevation = mapData.MapData.GetTileAt(tile.gridPos.x, tile.gridPos.y, biomeData).terrain.CharID;
                    tile.variation = mapData.MapData.GetTileAt(tile.gridPos.x, tile.gridPos.y, biomeData).Variation;
                    EditorUtility.SetDirty(tile);
                }
            }
        }
#endif
        #endregion

        #region Generate
    public override void GenerateMap()
    {
        ScanTiles();
        SidewaysMap.main.DrawTheMapFromEditorData(OutputToMapData());
        Destroy(gameObject);
    }
    void ScanTiles()
    {
        Initalize(dims.x, dims.y);
        var tiles = GetComponentsInChildren<MapGenPredefinedTile>();
        List<ObjectData> spawnObjs = new List<ObjectData>();
        foreach (MapGenPredefinedTile dit in tiles)
        {
            Vector2Int tPos = new Vector2Int(dit.gridPos.y, dims.x - dit.gridPos.x);
            Tiles[tPos.y][tPos.x] = new SidewaysTile(biomeData.GetElevation(dit.elevation), dit.variation);
            if (dit.objectData != null)
                spawnObjs.Add(new ObjectData()
                {
                    data = dit.objectData,
                    GridLocation = dit.gridPos
                });
        }
        GenObj = spawnObjs.ToArray();
    }
    public void SaveMap()
    {
        var tiles = GetComponentsInChildren<MapGenPredefinedTile>();

        mapData.MapData.height = GetHeight();
        mapData.MapData.MapData = "";
        for (int y = 0; y < GetHeight(); y++)
        {
            for (int x = 0; x < GetWidth(); x++)
            {
                mapData.MapData.MapData += tiles[x * GetHeight() + y].elevation+""+ tiles[x * GetHeight() + y].variation;

            }
        }

        // Save the ScriptableObject as an asset at the specified path
        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssetIfDirty(mapData);

    }
    private void OnValidate()
    {
     if (mapData!=null && !IsDone) 
        {
            dims.x = mapData.MapData.GetWidth();
            dims.y = mapData.MapData.GetHeight();
            IsDone = true;
        }
    }
    #endregion
}
