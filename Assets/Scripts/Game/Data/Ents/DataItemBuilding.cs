using System.Collections.Generic;
using UnityEngine;

public class DataItemBuilding : DataItemObject
{
    public List<DataItemTile> occupiedTiles = new();

    public override Vector2Int GetCoords()
    {
        return occupiedTiles[0].gridPos;
    }

    public override DataItemTile[] GetOccupiedTiles()
    {
return        occupiedTiles.ToArray();
    }
}
