 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemWorld : MonoBehaviour
{
    public GameObject tilePrefab;

    public static DisplayItemWorld main;
    private void Awake()
    {
        main = this;
        if (tilepool == null)
            tilepool = GetComponent<ObjectPool>();
    }
    #region Room Display
    Vector2Int worldCenter = Vector2Int.zero;
    public DisplayItemTile[,] TilesetData;
    public void GenerateWorld(int fullSize)
    {
        int rSize = (fullSize * 2 + 1);
        TilesetData = new DisplayItemTile[rSize, rSize];
        //  GenerateSquare(center - Vector2Int.one * fullSize, center + Vector2Int.one * fullSize);
    }
    public void GenerateSquare(Vector2Int mins, Vector2Int maxs)
    {
        worldCenter = mins;
        for (Vector2Int wPos = mins; wPos.y <= maxs.y; wPos.y++)
        {
            for (wPos.x = mins.x; wPos.x <= maxs.x; wPos.x++)
            {
                DrawTile(wPos.x, wPos.y);
            }
        }
    }
    public void GenerateWhole()
    {
        int rSize = DataItemWorld.main.mdata.size;
        TilesetData = new DisplayItemTile[rSize, rSize];
        GenerateSquare(Vector2Int.zero, Vector2Int.one * (rSize - 1));
    }
    #endregion
    #region Tile Pools
    public ObjectPool tilepool;
    public void DrawTile(int iX, int iY)
    {
        DrawTile(new Vector2Int(iX, iY));
    }
    public void DrawTile(Vector2Int point)
    {
        DrawTile(DataItemWorld.main.GetTileAxial(point), point);
    }
    public void DrawTile(DataItemTile tData, Vector2Int worldPos)
    {
        if (tData != null)
        {
            GameObject tile = tilepool.PoolItem(tilePrefab);
            if (tile.TryGetComponent(out DisplayItemTile wtile))
            {
                wtile.InitData(tData, worldPos);
                RegisterTile(wtile,worldPos);
            }
        }
        else
        {
            Debug.LogError($"tData is null at "+worldPos);
        }
    }

    #endregion
    #region Clear
    public void ClearWorld()
    {
        if (TilesetData != null)
            foreach (DisplayItemTile tile in TilesetData)
            {
                ClearTile(tile);
            }
    }
    public void ClearTiles(DisplayItemTile[] tiles)
    {
        foreach (DisplayItemTile tile in tiles)
        {
            ClearTile(tile);
        }
    }
    void ClearTile(DisplayItemTile tile)
    {
        if (tile.data != null)
        {
            tile.data.display = null;
            name = tilePrefab.name;
        }
        tilepool.DeactivateObject(tile.gameObject);
    }
    #endregion
    #region World Center and Visbility
    Vector2Int VisibleWorldCenter;
    public void AdjustVisibleTiles()
    {
        //VisibleWorldCenter = EntityPlayer.main.movement.coords.AxialCoords;
        if (TilesetData == null)
        {
         //   GenerateWorldAroundPoint(VisibleWorldCenter, WorldDefines.PlayerLos);
        }
        foreach (DisplayItemTile tile in TilesetData)
        {
            tile.ChangeVisibility(tile.IsVisible());
        }
    }
    public Vector2Int GetVisibleWorldCenter()
    {
        return DataItemWorld.main.Globalize(worldCenter + new Vector2Int(Mathf.FloorToInt(TilesetData.GetLength(0) / 2), Mathf.FloorToInt(TilesetData.GetLength(1) / 2)));
    }
    public HexCoords GetCoordinateWorldCenter()
    {
        return new HexCoords(GetVisibleWorldCenter());
    }
    #endregion
    #region Move And Register Tiles
    /*public void MoveTile(Vector2Int start, Vector2Int end)
    {
        tilepool.DeactivateObject(TilesetData[end.x, end.y].gameObject);

        RoomTile movedTile = TilesetData[start.x, start.y];

        movedTile.gridPos = end;
        RegisterTile(movedTile);
    }*/
    public void RegisterTile(DisplayItemTile tile, Vector2Int worldPos)
    {
        Vector2Int realPos = new Vector2Int(worldPos.x - worldCenter.x, worldPos.y - worldCenter.y);
        print("Register Tile " + realPos);
        TilesetData[realPos.x, realPos.y] = tile;

        tile.name = tilePrefab.name + " " + tile.data.coords + "~" + worldPos;
        
        Vector2 bidipos = DataItemWorld.main.TranslatePositionFromCenter(worldPos);

        tile.transform.localPosition = new Vector3(bidipos.x, bidipos.y, 0);
    }
    #endregion

    #region Cycle
    public void CycleWorld(Vector2Int dir)
    {
        CycleWorld(dir.x, dir.y);
    }
    public void CycleWorld(int iX, int iY)
    {
        print("[DynamicWorld] Cycle World " + iX + ":" + iY);
        if (iX < 0)
            CycleWorldLeft(Mathf.Abs(iX));
       if (iX > 0)
            CycleWorldRight(iX);
        if (iY < 0)
            CycleWorldDown(Mathf.Abs(iY));
        if (iY > 0)
            CycleWorldUp(iY);

    }
    void CycleWorldLeft(int dim)
    {
        worldCenter.x -= dim;

        int width = TilesetData.GetLength(0);
        int height = TilesetData.GetLength(1);

        for (int iY = 0; iY < height; iY++)
            for (int iX = width - 1; iX >= 0; iX--)
            {
                if (iX < dim)
                {
                    DrawTile(worldCenter.x + iX, worldCenter.y + iY);
                }
                else
                {
                    if (iX >= width - dim)
                    {
                        ClearTile(TilesetData[iX, iY]);
                    }
                    TilesetData[iX, iY] = TilesetData[iX - dim, iY];
                }
            }
    }
    void CycleWorldRight(int dim)
    {
        worldCenter.x += dim;

        int width = TilesetData.GetLength(0);
        int height = TilesetData.GetLength(1);

        for (int iY = 0; iY < height; iY++)
            for (int iX = 0; iX < width; iX++)
            {
                if (iX >= width - dim)
                {
                    DrawTile(worldCenter.x + iX, worldCenter.y + iY);
                }
                else
                {
                    if (iX < dim)
                    {
                        ClearTile(TilesetData[iX, iY]);
                    }
                    TilesetData[iX, iY] = TilesetData[iX + dim, iY];
                }
            }
    }
    void CycleWorldDown(int dim)
    {
        worldCenter.y -= dim;

        int width = TilesetData.GetLength(0);
        int height = TilesetData.GetLength(1);

        for (int iX = 0; iX < width; iX++)
            for (int iY = height - 1; iY >= 0; iY--)
            {
                if (iY < dim)
                {
                    DrawTile(worldCenter.x + iX, worldCenter.y + iY);
                }
                else
                {
                    if (iY >= height - dim)
                    {
                        ClearTile(TilesetData[iX, iY]);
                    }
                    TilesetData[iX, iY] = TilesetData[iX , iY - dim];
                }
            }
    }
    void CycleWorldUp(int dim)
    {
        worldCenter.y += dim;

        int width = TilesetData.GetLength(0);
        int height = TilesetData.GetLength(1);

        for (int iX = 0; iX < width; iX++)
            for (int iY = 0; iY < height; iY++)
            {
                if (iY >= height - dim)
                {
                    DrawTile(worldCenter.x + iX, worldCenter.y + iY);
                }
                else
                {
                    if (iY < dim)
                    {
                        ClearTile(TilesetData[iX, iY]);
                    }
                    TilesetData[iX, iY] = TilesetData[iX , iY + dim];
                }
            }
    }
    #endregion
}
