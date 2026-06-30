using System.Collections.Generic;
using UnityEngine;

public class SidewaysMap : MonoBehaviour
{
    public static SidewaysMap main;
    public GameObject tilePrefab, spritePrefab;
    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    public MapData mapData;

    public SidewaysTile[,] tiles;

    public int width { get => tiles.GetLength(0); }
    public int height { get => tiles.GetLength(1); }

    public enum GetTileType 
    {
        real,
        imaginary,
        clamped
    }
    public SidewaysTile[] GetTiles(Vector2Int[] av, GetTileType imaginary = GetTileType.real)
    {
        List<SidewaysTile> valid = new List<SidewaysTile>();
        foreach (Vector2Int tile in av)
        {
            valid.Add(GetTile(tile, imaginary));
        }
        return valid.ToArray();
    }
    public bool IsOnMap(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
    public SidewaysTile GetTile(Vector2Int v, GetTileType imaginary = GetTileType.real) { return GetTile(v.x, v.y, imaginary); }
    public SidewaysTile GetTile(int iX, int iY, GetTileType imaginary = GetTileType.real)
    {
        if (IsOnMap(iX,iY))
        {
            return tiles[iX, iY];
        }
        if (imaginary == GetTileType.imaginary)
        {
            return SidewaysTile.CreateTile(tilePrefab,mapData, new Vector2Int(iX, iY), true);
        }
        else if (imaginary == GetTileType.clamped)
        {
            return tiles[Mathf.Clamp(iX, 0, width - 1), Mathf.Clamp(iY, 0, height - 1)];
        }
        else return null;
    }
    public SidewaysTile[] GetTilesInArea(TerrainDefines.AreaType areaType, Vector2Int center, int radius, GetTileType imaginary = GetTileType.real)
    {
     switch (areaType)
        {
            case TerrainDefines.AreaType.square:
                return GetTilesInRect(center - radius * Vector2Int.one , center + radius * Vector2Int.one, imaginary);
            default:
                return GetTilesInCircle(center, radius, imaginary);
        }
    }
    public SidewaysTile[] GetTilesInRect(Rect rect, GetTileType imaginary = GetTileType.real)
    {
        Vector2Int start = TranslateWorldPosition(rect.min);
        Vector2Int end = TranslateWorldPosition(rect.max);
        return GetTilesInRect(start, end, imaginary);
    }
    public SidewaysTile[] GetTilesInRect(RectInt rect, GetTileType imaginary = GetTileType.real)
    {
        return GetTilesInRect(rect.min,rect.max, imaginary);
    }
    public SidewaysTile[] GetTilesInRect(Vector2Int start, Vector2Int end, GetTileType imaginary = GetTileType.real)
    {
        List<SidewaysTile> found = new List<SidewaysTile>();
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                found.Add(GetTile(x, y, imaginary));
            }
        }
        return found.ToArray();
    }
    public SidewaysTile[] GetTilesInCircle(Vector2Int center, int radius,GetTileType imaginary = GetTileType.real)
    {
        List<SidewaysTile> found = new List<SidewaysTile>();
        int radiusSquared = radius * radius;

        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                // Calculate the distance from the center
                int dx = x - center.x;
                int dy = y - center.y;
                if (dx * dx + dy * dy <= radiusSquared)
                {
                    found.Add(GetTile(x, y, imaginary));
                }
            }
        }
        return found.ToArray();
    }
    public SidewaysTile GetClosestToPoint(SidewaysTile position, TerrainDefines.Movement movement, float maxDistance = Mathf.Infinity, bool empty = false)
    {
        if (position.IsPassible(movement) && (position.armyLayer == null ||!empty))
            return position;
        return GetClosestToPoint(position.gridPos, movement, maxDistance) ;
    }
    public SidewaysTile GetClosestToPoint(Vector2Int point, TerrainDefines.Movement movement, float maxDistance = Mathf.Infinity, bool empty = false)
    {
        List<SidewaysTile> openlist = new List<SidewaysTile>
        {
           GetTile(point,imaginary: GetTileType.clamped)
        };
        List<SidewaysTile> closedlist = new ();

        while (openlist.Count > 0)
        {
            if (openlist[0] != null)
            {
                if (openlist[0].IsPassible(movement) && (!empty|| openlist[0].armyLayer == null))
                    return openlist[0];
                
                    foreach (var neighbor in openlist[0].neighbors)
                    {
                        if (neighbor == null) continue;
                        if (!closedlist.Contains (neighbor))
                        {
                            closedlist.Add(neighbor);

                            float dist = (openlist[0].gridPos - point).sqrMagnitude;
                            if (dist <= maxDistance) openlist.Add(neighbor);
                        

                    }
                }
            }
            openlist.RemoveAt(0);
        }

        return GetClosest(point, closedlist.ToArray());
    }
    public SidewaysTile GetClosest(Vector2Int point, SidewaysTile[] list )
    {
        if (list.Length > 0)
        {
            var closest = list[0];
            int distance = int.MaxValue;
            foreach (var tile in list)
            {
                var sqrDist = (tile.gridPos - point).sqrMagnitude;
                if (sqrDist < distance)
                {
                    closest = tile;
                    distance = sqrDist;
                }
            }
            return closest;
        }
        return null;
    }


    public static Vector2Int TranslateWorldPosition(Vector2 worldPosition)
    {
        return Vector2Int.FloorToInt( new Vector2(worldPosition.x, worldPosition.y) / TerrainDefines.UnitsPerTile);
    }

    public static Vector2 TranslateGridPosition(Vector2Int gridPosition,int height)
    {
        return new Vector2(gridPosition.x, height - gridPosition.y - 1 ) * TerrainDefines.UnitsPerTile;
    }

    public  Vector3 TranslateEntityPosition(Vector2Int gridPosition)
    {
        Vector2 output = TranslateGridPosition(gridPosition) + new Vector2(1,-1) * .5f * TerrainDefines.UnitsPerTile; ;
        return new Vector3(output.x, output.y, output.y);
    }

    public Vector2 TranslateGridPosition(Vector2Int gridPosition)
    {
        return TranslateGridPosition(gridPosition,height);
    }

    public void DrawTheMapFromEditorData(MapData Map)
    {
        UnityEngine.Debug.Log("Awake Map Done");
        mapData = Map;

        tiles = new SidewaysTile[Map.GetWidth(), Map.GetHeight()];

        for (int iY = 0; iY < height; iY++)
        {

            for (int iX = 0; iX < width; iX++)
            {
                SidewaysTile tile = SidewaysTile.CreateTile(tilePrefab, mapData, new Vector2Int(iX, iY), false);
                tiles[iX, iY] = tile;
                tile.display?.gameObject?.transform.SetParent(transform);
            }
        }
        UnityEngine.Debug.Log("Tileset Done");
        Draw();
    }
    public void Draw()
    {
        foreach (SidewaysTile tile in tiles)
        {
            tile.InitNeighbors();
            tile.display?.Draw();
        }
    }
}