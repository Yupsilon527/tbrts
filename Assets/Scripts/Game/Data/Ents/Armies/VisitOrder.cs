using UnityEngine;

public class VisitOrder : Order
{
    public VisitOrder(ID orderID, Vector2Int gridDest) : base(orderID, gridDest)
    {
    }

    public override bool Resolve(DataItemBanner attacker)
    {
        if (attacker.GetMainTile().buildingLayer is DataItemSite site)
        {
            if (attacker.VisitSite(site))
            {
                return true;
            }
        }
        return false;
    }
}
