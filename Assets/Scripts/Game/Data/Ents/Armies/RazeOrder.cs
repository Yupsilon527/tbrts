using UnityEngine;

public class RazeOrder : Order
{
    public RazeOrder(ID orderID, Vector2Int gridDest) : base(orderID, gridDest)
    {
    }

    public override bool Resolve(DataItemBanner attacker)
    {
        if (attacker.GetMainTile().buildingLayer is DataItemCastle city)
        {
            //player ai
            if (attacker.CanInvadeCastle(city))
            {
                InterfaceManager.main.OpenCastleRazeWindow(city, attacker);
                return true;
            }
        }
        return false;
    }
}
