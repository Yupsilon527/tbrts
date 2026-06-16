using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SidewaysTile
{
    public Vector2Int gridPos;
    public GameObject gameObject;

    public int Variation = -1;
    public int iEdgeData;
    public bool isRoad;
    public ElevationData elevation;

    //layers
    public DataItemArmy armyLayer;
    public DataItemBuilding buildingLayer, regionCastle;
    public DataItemPowerup powerupLayer;

    public override string ToString()
    {
        return "sTile " + gridPos + " " + elevation.ToString();
    }

    #region Tile Creation

    public SidewaysTile(ElevationData eC = null, int tV = 0)
    {
        elevation = eC != null ? eC : new ElevationData();
        Variation = tV;
    }
    public static SidewaysTile CreateTile(GameObject prefab, MapData mapData, Vector2Int pos, bool skipdraw)
    {
        SidewaysTile tile = new SidewaysTile();
        tile.gridPos = pos;
        tile.elevation = mapData.GetTileAt(pos).elevation;
        tile.RevealedByPlayer = new int[] { 0, 0 };

        if (!skipdraw)
        {
            tile.gameObject = GameObject.Instantiate(prefab);
            tile.gameObject.name = "Tile " + pos.x + "_" + pos.y;
            tile.gameObject.transform.position = tile.GetWorldPosition();

        }
        return tile;
    }
    public void MakeTileGameObject(int iX, int iY, Vector2 delta)
    {
        ElevationData ElevationBorders = SidewaysMap.main.mapData.biome.Elevations.FirstOrDefault(item => item.elevation == TerrainDefines.Elevation.Void);
        ElevationData ElevationPlain = SidewaysMap.main.mapData.biome.Elevations.FirstOrDefault(item => item.elevation == TerrainDefines.Elevation.Plain);

        //Draw the Foundations

        List<ElevationData> NeighborData = new List<ElevationData> { ElevationBorders, ElevationBorders, ElevationBorders, ElevationBorders };
        int[] VariationData = new int[] { 0, 0, 0, 0 };
        int[][] iData = new int[][] { new int[] { -1, -1 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 0, 0 } };

        for (int iNeighbor = 0; iNeighbor < NeighborData.Count; iNeighbor++)
        {
            int tX = iX + iData[iNeighbor][0];
            int tY = iY + iData[iNeighbor][1];

            var tile = SidewaysMap.main.mapData.GetTileAt(new Vector2Int(tX, tY));
            if (tile != null && tile.elevation != null)
            {
                VariationData[iNeighbor] = tile.Variation;
                NeighborData[iNeighbor] = tile.elevation;
            }
            else
            {
                NeighborData[iNeighbor] = ElevationBorders;
            }

        }
        if (NeighborData.Any(e => e.elevation == TerrainDefines.Elevation.Forest) ||
            NeighborData.Any(e => e.elevation == TerrainDefines.Elevation.Hill) ||
            NeighborData.Any(e => e.elevation == TerrainDefines.Elevation.Mountain))
        {
            NeighborData.Add(ElevationPlain);
        }


        var baseElevation = NeighborData[0];
        foreach (ElevationData Gir in NeighborData)
        {
            if (Gir != null && (baseElevation == null || Gir.elevation < baseElevation.elevation))
                baseElevation = Gir;
        }

        if (baseElevation != null && baseElevation.TilesetVariations != null && baseElevation.TilesetVariations.Length > 0)
        {
            GameObject Background = GameObject.Instantiate(SidewaysMap.main.spritePrefab);
            Background.GetComponent<SpriteRenderer>().sprite = baseElevation.GetSprite(0, VariationData[0]);
            Background.transform.SetParent(gameObject.transform);
            Background.transform.localPosition = (Vector3)delta;
            Background.name = "Segment " + iX + "_" + iY + " " + baseElevation;
        }

        List<ElevationData> eUsed = new List<ElevationData>();
        eUsed.Add(baseElevation);
        for (int iN = 0; iN < NeighborData.Count; iN++)
        {
            ElevationData edge = NeighborData[iN];
            if (!eUsed.Contains(edge))
            {

                bool[] pass = new bool[] {  NeighborData [0].elevation < edge.elevation,
                        NeighborData [1].elevation < edge.elevation,
                        NeighborData [2].elevation< edge.elevation,
                        NeighborData [3].elevation < edge.elevation
                    };

                if (edge.elevation >= TerrainDefines.Elevation.Forest && edge.elevation <= TerrainDefines.Elevation.Mountain)
                {

                    pass = new bool[] { NeighborData [0].elevation != edge.elevation,
                             NeighborData [1].elevation != edge.elevation,
                             NeighborData [2].elevation != edge.elevation,
                             NeighborData [3].elevation != edge.elevation
                         };
                }
                else if (edge.elevation == TerrainDefines.Elevation.Plain)
                {
                    pass = new bool[] {
                             NeighborData [0].elevation < TerrainDefines.Elevation.Plain || NeighborData [0].elevation > TerrainDefines.Elevation.Mountain,
                             NeighborData [1].elevation < TerrainDefines.Elevation.Plain || NeighborData [1].elevation > TerrainDefines.Elevation.Mountain,
                             NeighborData [2].elevation < TerrainDefines.Elevation.Plain || NeighborData [2].elevation > TerrainDefines.Elevation.Mountain,
                             NeighborData [3].elevation < TerrainDefines.Elevation.Plain || NeighborData [3].elevation > TerrainDefines.Elevation.Mountain
                         };
                }

                int nData = TerrainDefines.GetSprite(pass);

                if (nData >= 0)
                {
                    if (edge != null && edge.TilesetVariations.Length > 0)
                    {
                        GameObject layer = Object.Instantiate(SidewaysMap.main.spritePrefab);
                        layer.transform.SetParent(gameObject.transform);
                        layer.transform.localPosition = (Vector3)delta;

                        layer.transform.localScale = Vector3.one;


                        var elevation = edge.GetSprite(nData, VariationData[iN]);

                        layer.name = "Segment " + iX + "_" + iY + " " + elevation.name;

                        layer.GetComponent<SpriteRenderer>().sprite = elevation;
                        layer.GetComponent<SpriteRenderer>().sortingOrder = (int)edge.elevation;
                    }

                    eUsed.Add(edge);
                }
            }
        }
    }

    public void Draw()
    {
        MakeTileGameObject(gridPos.x, gridPos.y, Vector2.zero);
        if (gridPos.x == SidewaysMap.main.mapData.GetWidth() - 1 && gridPos.y >= SidewaysMap.main.mapData.GetHeight() - 1)
        {
            MakeTileGameObject(gridPos.x + 1, gridPos.y + 1, Vector2.right + Vector2.down);
        }
        if (gridPos.x == SidewaysMap.main.mapData.GetWidth() - 1)
        {
            MakeTileGameObject(gridPos.x + 1, gridPos.y, Vector2.right);

        }
        if (gridPos.y == SidewaysMap.main.mapData.GetHeight() - 1)
        {
            MakeTileGameObject(gridPos.x, gridPos.y + 1, Vector2.down);
        }
    }
    #endregion
    #region Tile Colors
    public Color OverlayColor;
    public Color HighlightColor;
    public enum tileState
    {
        clear = 0,
        highlight,
        highlight_ability,
        setup,
        select_empty,
        select_ally,
        select_enemy,
        select_unit,
        valid_tile,
        invalid_tile,
        mindread_danger,
        mindread_ally,
        mindread_walkally,
    };

    public Color GetColor(tileState colorID)
    {
        switch (colorID)
        {
            case tileState.highlight:
            case tileState.mindread_walkally:
                return new Color(1, 1, 1, .2f);
            case tileState.select_empty:
                return new Color(.2f, .2f, .2f, .2f);
            case tileState.select_ally:
            case tileState.mindread_ally:
            case tileState.setup:
                return new Color(0, 1, 0, .2f);
            case tileState.select_enemy:
            case tileState.mindread_danger:
            case tileState.select_unit:
                return new Color(1, .75f, 0, .2f);
            case tileState.valid_tile:
                return new Color(1, 1, 1, .5f);
            case tileState.highlight_ability:
                return new Color(1, 1, 1, .2f);
            case tileState.invalid_tile:
                return new Color(.2f, .2f, .2f, .2f);
            default:
                return Color.clear;
        }
    }

    public void ChangeColor(tileState newState)
    {
        if (gameObject != null)
        {
            OverlayColor = GetColor(newState);
            gameObject.GetComponent<SpriteRenderer>().color = GetColor(newState);
        }
    }

    public void Highlight(tileState newState)
    {
        if (gameObject != null)
        {
            HighlightColor = GetColor(newState);
            if (HighlightColor.a > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().color = GetColor(newState);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = OverlayColor;
            }
        }
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
    #region Entities
    public DataItemArmy locatedArmy;

    public void AddEntity(DataItemArmy mob)
    {
        locatedArmy = mob;
    }
    public void RemoveEntity(DataItemArmy mob)
    {
        if (mob == locatedArmy)
            locatedArmy = null;
    }
    #endregion
}
