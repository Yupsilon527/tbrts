using System;
using UnityEngine;

public class SidewaysTile
{
    public Vector2Int gridPos;
    public DisplayItemTile display;

    public int Variation = -1;
    public int iEdgeData;
    public bool isRoad;
    public ElevationData terrain;

    public SidewaysTile[] neighbors = Array.Empty<SidewaysTile>();
    public bool[]passible;
    public int[]movecost;
    public void Init()
    {
        RevealedByPlayer = new int[GameManager.main.playerManager.players.Length];
        InitPassible(); 
    }
    void InitPassible()
    {
        passible = new bool[(int)TerrainDefines.Movement.Total];
        movecost = new int[(int)TerrainDefines.Movement.Total];

        for (int i = 0; i < passible.Length; i++)
        {
            passible[i] = TerrainDefines.CanIWalkOver((TerrainDefines.Movement)i, terrain.elevation);
            movecost[i] = TerrainDefines.GetMoveCost((TerrainDefines.Movement)i, terrain.elevation);
        }
    }
    public int GetMoveCost(TerrainDefines.Movement m)
    {
        return movecost[(int)m];
    }
    public bool IsPassible(TerrainDefines.Movement m)
    {
        return passible[(int)m];
    }
    public TerrainDefines.Elevation GetWalkElevation()
    {
        return terrain.elevation;
    }
    //layers
    public DataItemArmy armyLayer;
    public DataItemBuilding buildingLayer, regionCastle;
    public DataItemPowerup powerupLayer;

    public override string ToString()
    {
        return "sTile " + gridPos + " " + terrain.ToString();
    }
    public bool IsAdjecent(SidewaysTile other)
    {
        return IsAdjecent(other.gridPos);
    }
    public bool IsAdjecent(Vector2Int other)
    {
        return Mathf.Abs(gridPos.x - other.x) <= 1 && Mathf.Abs(gridPos.y - other.y) <= 1;
    }

    #region Tile Creation

    public SidewaysTile(ElevationData eC = null, int tV = 0)
    {
        terrain = eC != null ? eC : new ElevationData();
        Variation = tV;
    }
    public static SidewaysTile CreateTile(GameObject prefab, MapData mapData, Vector2Int pos, bool skipdraw)
    {
        SidewaysTile tile = new SidewaysTile();
        tile.gridPos = pos;
        tile.terrain = mapData.GetTileAt(pos).terrain;
        tile.Init();

        if (!skipdraw)
        {
            var display = GameObject.Instantiate(prefab);
            if (display.TryGetComponent(out DisplayItemTile dt))
            {
                dt.tile = tile;
                tile.display = dt;
                dt.name = $"Tile {pos}: {pos.y*mapData.GetWidth()+pos.x}";
                dt.transform.position = tile.GetWorldPosition();
            }
        }
        return tile;
    }
    #endregion
    #region Neighbors
    public void InitNeighbors()
    {
        neighbors = new SidewaysTile[] {
            GetNeighbor( Vector2Int.right),
            GetNeighbor( Vector2Int.left),
            GetNeighbor( Vector2Int.down),
            GetNeighbor( Vector2Int.up),

            GetNeighbor(  Vector2Int.right+Vector2Int.down),
            GetNeighbor(  Vector2Int.left+Vector2Int.down),
            GetNeighbor(  Vector2Int.right+Vector2Int.up),
            GetNeighbor(  Vector2Int.left+Vector2Int.up),
        };
    }
    public enum GridDirection
    {
        right,
        left,
        down,
        up,
    }
    public SidewaysTile GetNeighbor(GridDirection delta)
    {
        if (neighbors == null || neighbors.Length == 0)
            InitNeighbors();
        return neighbors[(int)delta];
    }
    SidewaysTile GetNeighbor(Vector2Int delta)
    {
        return SidewaysMap.main.GetTile(gridPos.x + delta.x, gridPos.y - delta.y, SidewaysMap.GetTileType.imaginary);
    }
    public Vector2 GetWorldPosition()
    {
        return SidewaysMap.main.TranslateGridPosition(gridPos);
    }
    public Vector3 GetCenterPosition()
    {
        return SidewaysMap.main.TranslateEntityPosition(gridPos);
    }
    public bool IsNeighboring(SidewaysTile other)
    {
        Vector2 deltapos = other.gridPos - gridPos;
        return Mathf.Abs(deltapos.x) <= 1 && Mathf.Abs(deltapos.y) <= 1;
    }
    #endregion
    #region Visibility
    public float sqrDistance;
    public int[] RevealedByPlayer;

    public bool IsRevealedByPlayer(DataItemPlayer player, UnitDefines.TileVisibility visibility)
    {
        return RevealedByPlayer[player.ID] >= (int)visibility;
    }
    #endregion

}
