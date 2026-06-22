using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayItemTile : MonoBehaviour
{
    public SidewaysTile tile;
    public SpriteRenderer highlight;
    public void Draw()
    {
        Vector2Int gridPos = tile.gridPos;
        MakeTileGameObject(gridPos.x, gridPos.y, Vector2.zero);
        if (gridPos.x == SidewaysMap.main.mapData.GetWidth() - 1 && gridPos.y >= SidewaysMap.main.mapData.GetHeight() - 1)
        {
            MakeTileGameObject(gridPos.x + 1, gridPos.y + 1, Vector2.right * TerrainDefines.UnitsPerTile + Vector2.down * TerrainDefines.UnitsPerTile);
        }
        if (gridPos.x == SidewaysMap.main.mapData.GetWidth() - 1)
        {
            MakeTileGameObject(gridPos.x + 1, gridPos.y, Vector2.right * TerrainDefines.UnitsPerTile);

        }
        if (gridPos.y == SidewaysMap.main.mapData.GetHeight() - 1)
        {
            MakeTileGameObject(gridPos.x, gridPos.y + 1, Vector2.down * TerrainDefines.UnitsPerTile);
        }
    }
    public void MakeTileGameObject(int iX, int iY, Vector2 delta)
    {
        ElevationData ElevationBorders = SidewaysMap.main.mapData.biome.Elevations.FirstOrDefault(item => item.elevation == TerrainDefines.Elevation.Void);
        ElevationData ElevationPlain = SidewaysMap.main.mapData.biome.Elevations.FirstOrDefault(item => item.elevation == TerrainDefines.Elevation.Plain);

        //Draw the Foundations

        List<ElevationData> NeighborData = new List<ElevationData> { ElevationBorders, ElevationBorders, ElevationBorders, ElevationBorders };
        int[] VariationData = new int[] { 0, 0, 0, 0, 0 };
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


                        var elevation = edge.GetSprite(nData, iN < VariationData.Length ? VariationData[iN] : 0);

                        layer.name = "Segment " + iX + "_" + iY + " " + elevation.name;

                        layer.GetComponent<SpriteRenderer>().sprite = elevation;
                        layer.GetComponent<SpriteRenderer>().sortingOrder = (int)edge.elevation;
                    }

                    eUsed.Add(edge);
                }
            }
        }
    }
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
            highlight.color = GetColor(newState);
        }
    }

    public void Highlight(tileState newState)
    {
        if (gameObject != null)
        {
            HighlightColor = GetColor(newState);
            if (HighlightColor.a > 0)
            {
                highlight.color = GetColor(newState);
            }
            else
            {
                highlight.color = OverlayColor;
            }
        }
    }
    #endregion

}
