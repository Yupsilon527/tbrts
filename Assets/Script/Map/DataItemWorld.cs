using System.Collections.Generic;
using UnityEngine;

public class DataItemWorld : MonoBehaviour
{
    public static DataItemWorld main;
    private void Awake()
    {
        main = this;
    }
    public DataItemTile[,] Tiles;
    public void FinalizeWorld()
    {
        foreach (DataItemTile tile in Tiles)
        {
            if (tile != null)
            {
                tile.InitNeighbors(this);
            }
        }
    }
    #region WorldSize
    int WorldSize = 1;
    public void InitializeWorld(int size)
    {

            WorldSize = size;
            int fullSize = GetFullSize();
            Tiles = new DataItemTile[fullSize, fullSize];
    }
    public void PushTile(int iX, int iY, DataItemTile tile)
    {
        Tiles[iX, iY] = tile;
    }
    public MapData mdata;
    public void GenerateWorld(MapData data)
    {
        mdata = data;
        Tiles = new DataItemTile[data.size, data.size];

        for (Vector2Int wPos = Vector2Int.zero; wPos.y < data.size; wPos.y++)
        {
            for (wPos.x = 0; wPos.x < data.size; wPos.x++)
            {
                Tiles[wPos.x, wPos.y] = new DataItemTile()
                {
                    coords = new HexCoords(wPos),
                    elevation = (WorldDefines.Elevations) data.ElevationMatrix[wPos.x, wPos.y],
                };
            }
        }
    }

    public int GetFullSize()
    {
        return WorldSize * 2 + 1;
    }
    public int GetHalfSize()
    {
        return WorldSize;
    }
    public Vector2Int GetWorldCenter()
    {
        return Vector2Int.one * GetHalfSize();
    }
    public HexCoords GetWorldCoordinates()
    {
        return new HexCoords(GetWorldCenter()) ;
    }
    #endregion
    #region Translate
    public Vector2 TranslatePositionFromCenter(HexCoords pos)
    {
        return TranslatePositionFromCenter(pos.AxialCoords, Vector2Int.zero);
    }
    public Vector2 TranslatePositionFromCenter(Vector2Int pos)
    {
        return TranslatePositionFromCenter(pos, Vector2Int.zero);
    }
    public Vector2 TranslatePositionFromCenter(Vector2Int pos, Vector2Int worldDelta)
    {
        return TranslatePosition(pos + worldDelta * GetFullSize(), Vector2Int.one * GetHalfSize());
    }
    public static Vector2 TranslatePosition(Vector2Int pos, Vector2Int center)
    {
        return TranslatePosition(pos - center);

    }
    public static Vector2 TranslatePosition(Vector2Int pos)
    {
        var x = (Mathf.Sqrt(3) * pos.x + Mathf.Sqrt(3) / 2 * pos.y);
        var y = (1.5f * pos.y);
        return new Vector2(x, y) * .5f;

    }
    public Vector2Int TranslateCoordinate(Vector2 point)
    {
        return Vector2Int.RoundToInt(point);
    }
    public DataItemTile GetClosestToPoint( Vector2Int point)
    {
        return GetClosestToPoint( new HexCoords(point));
    }
    public DataItemTile GetClosestToPoint(HexCoords point)
    {
        float sqrDist = Mathf.Infinity;
        DataItemTile dest = null;
        foreach (DataItemTile n in Tiles)
        {
            if (n.IsPassible())
            {
                float dist = n.coords.DistanceFrom(point);
                if (dist < sqrDist)
                {
                    dest = n;
                    sqrDist = dist;
                }
            }
        }
        return dest;
    }
    public DataItemTile GetClosestToPoint(Mob e, Vector2Int point)
    {
        return GetClosestToPoint(e, new HexCoords(point));
    }
    public DataItemTile GetClosestToPoint(Mob e, HexCoords point)
    {
        float sqrDist = Mathf.Infinity;
        DataItemTile dest = null;
        foreach (DataItemTile n in Tiles)
        {
            if (WorldDefines.CanFitEntity(n,e))
            {
                float dist = n.coords.DistanceFrom( point);
                if (dist < sqrDist)
                {
                    dest = n;
                    sqrDist = dist;
                }
            }
        }
        return dest;
    }
    #endregion
    #region Find Tiles
    public DataItemTile GetTile(HexCoords hex)
    {
        return GetTileAxial(hex.GlobalAxialCoords);
    }
    public DataItemTile GetTileAxial(int iX, int iY)
    {
        return GetTileAxial(new Vector2Int(iX, iY));
    }
    public Vector2Int Globalize(Vector2Int pos)
    {
        return Globalize(pos.x, pos.y);
    }
    public Vector2Int Globalize(int x, int y)
    {
        int w = Tiles.GetLength(0);
        x = x % w;
        while (x < 0) x += w;

        int h = Tiles.GetLength(1);
        y = y % h;
        while (y < 0) y += h;
        return new Vector2Int(x, y);
    }
    public HexCoords Globalize(HexCoords pos)
    {
        return new HexCoords(Globalize(pos.GlobalAxialCoords));
    }
    public DataItemTile GetTileAxial(Vector2Int pos)
    {
        //if (pos.x < 0 || pos.y < 0 || pos.x >= Tiles.GetLength(0) || pos.y >= Tiles.GetLength(1))
        //    return null;
        pos = Globalize(pos);
        return Tiles[pos.x, pos.y];
    }
    public DataItemTile GetTileCube(Vector3Int cubepos)
    {
        return GetTileAxial(Globalize(HexCoords.CubeToAxial(cubepos)));
    }
    public DataItemTile[] GetTilesInRect(Vector2Int mins, Vector2Int Maxs)
    {
        List<DataItemTile> foundTiles = new List<DataItemTile>();

        for (int iX = mins.x; iX <= Maxs.x; iX++)
            for (int iY = mins.y; iY <= Maxs.y; iY++)
            {
                DataItemTile foundTile = GetTileAxial(iX, iY);
                if (foundTile != null)
                {
                    foundTiles.Add(foundTile);
                }
            }

        return foundTiles.ToArray();
    }
    public DataItemTile[] GetTilesInCirc(HexCoords center, int radius)
    {
        return GetTilesInCirc(center.AxialCoords, radius);
    }
    public DataItemTile[] GetTilesInCirc(Vector2Int center, int radius)
    {
        List<DataItemTile> foundTiles = new List<DataItemTile>();
        for (int iX = center.x - radius; iX <= center.x + radius; iX++)
            for (int iY = center.y - radius; iY <= center.y + radius; iY++)
            {
                DataItemTile foundTile = GetTileAxial(iX, iY);
                if (foundTile != null && HexCoords.AxialDistance(center, new Vector2Int(iX, iY)) <= radius)
                {
                    foundTiles.Add(foundTile);
                }
            }

        return foundTiles.ToArray();
    }
    public DataItemTile[] GetTilesInLine(Vector3Int center, Vector3Int direction, int radius)
    {
        List<DataItemTile> foundTiles = new List<DataItemTile>();
        for (int step = 0; step <= radius; step++)
        {
            DataItemTile selTile = GetTileCube(center + step * direction);
            foundTiles.Add(selTile);
        }

        return foundTiles.ToArray();
    }
    #endregion

}
