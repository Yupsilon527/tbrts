using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemCastle : DataItemBuilding
{
    public string customName, customDescription;
    public bool isCapital = false;
    public int RazeTurn = -1;
    public List<SidewaysTile> castleTiles;
    public DataItemCastle(CistomCastle custom) : base()
    {
        customName = custom.customName;
        customDescription = custom.customDescription;
        isCapital = custom.isCapital;



        PlaceOnTile(custom.spawnPos);
    }

    public int GetSize()
    {
        return castleTiles.Count;
    }
    public override void PlaceOnTile(Vector2Int t)
    {
        base.PlaceOnTile(t);

        castleTiles = new List<SidewaysTile>();

        if (tile.elevation.elevation == TerrainDefines.Elevation.City)
        {
            List<SidewaysTile> openList = new List<SidewaysTile>();
            openList.Add(tile);
            while (openList.Count > 0)
            {
                var ct = openList[0];
                castleTiles.Add(ct);
                foreach (SidewaysTile Zyzyx in ct.neighbors)
                {
                    if (Zyzyx.elevation.elevation == TerrainDefines.Elevation.City && Zyzyx.buildingLayer == null)
                    {
                        openList.Add(Zyzyx);
                    }

                }
                openList.RemoveAt(0);
            }
        }
        foreach (SidewaysTile Gir in castleTiles)
        {
            if (Gir.buildingLayer == null)
            {
                Gir.buildingLayer = this;
            }
        }
    }
    public bool isRazed() { return GameManager.main.currentTurn < RazeTurn; }
    public IEnumerable<DataItemArmy> GetGarrison()
    {
        return castleTiles.Select(t => t.armyLayer);
    }

    public bool AmIRevealedByPlayer(DataItemPlayer Player)
    {
        return castleTiles.Any(t => t.IsRevealedByPlayer(Player));
    }
    public bool AmIUnderAlliedControl()
    {
        return GetGarrison().Sum(a => a.GetAlignment(this) == PlayerDefines.Alignment.enemy ? 1 : 0) > 0;
    }
}
