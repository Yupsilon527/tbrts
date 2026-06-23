using UnityEngine;

public class SidewaysTile
{
    public Vector2Int gridPos;
    public DisplayItemTile display;

    public int Variation = -1;
    public int iEdgeData;
    public bool isRoad;
    public ElevationData terrain;

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
        tile.RevealedByPlayer = new int[] { 0, 0 };

        if (!skipdraw)
        {
            var display = GameObject.Instantiate(prefab);
            if (display.TryGetComponent(out DisplayItemTile dt))
            {
                dt.tile = tile;
                tile.display = dt;
                dt.name = "Tile " + pos.x + "_" + pos.y;
                dt.transform.position = tile.GetWorldPosition();
            }
        }
        return tile;
    }
    #endregion
    #region Neighbors
    public SidewaysTile[] neighbors;
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

    public bool IsRevealedByPlayer(DataItemPlayer player)
    {
        return true;
    }
    #endregion

}
