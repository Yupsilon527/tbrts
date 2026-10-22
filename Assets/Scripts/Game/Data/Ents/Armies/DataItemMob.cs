using UnityEngine;

public class DataItemMob : DataItemObject
{
    protected Vector2Int gridPos;
    protected DataItemTile tile;

    public override Vector2Int GetCoords()
    {
        return gridPos;
    }

    public override DataItemTile[] GetOccupiedTiles()
    {
        return new DataItemTile[] { tile };
    }
    public virtual DataItemTile GetMainTile()
    {
        return tile;
    }

}