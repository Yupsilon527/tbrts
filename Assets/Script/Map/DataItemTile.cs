using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using static WorldDefines;

public class DataItemTile
{
    public int searchIndex = 0;

    public int groundEntity;
    public int airEntity;

    public Elevations elevation;
    public int[] revealedByPlayer;

    public DataItemTile[] neighbors;
    public DisplayItemTile display;
    public void InitNeighbors(DataItemWorld data)
    {
        Vector2Int axial = coords.AxialCoords;
        neighbors = new DataItemTile[]
        {
            data.GetTileAxial(axial.x + 1,axial.y - 1),
            data.GetTileAxial(axial.x ,axial.y - 1),
            data.GetTileAxial(axial.x + 1,axial.y),
        data.GetTileAxial(axial.x - 1, axial.y ),
        data.GetTileAxial(axial.x - 1,axial.y + 1),
        data.GetTileAxial(axial.x, axial.y + 1),
        };
    }

    public HexCoords coords = new HexCoords(Vector2Int.zero);

    #region entity
    public Mob LocatedEntity;
    public bool EntityCheck(Mob e)
    {
        return LocatedEntity == null || LocatedEntity == e;
    }
    public bool IsPassible()
    {
        return true;
    }
    #endregion
}
