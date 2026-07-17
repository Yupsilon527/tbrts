using Astar;
using UnityEngine;

public class Order
{
    public enum ID { Move = 0, Follow = 1, Rest = 2, Raze = 3 };
    public ID OrderID;
    public Vector2Int gridDest;
    public PathfinderPath path;

    public Order(ID orderID, Vector2Int gridDest)
    {
        OrderID = orderID;
        this.gridDest = gridDest;
    }

    public virtual void RecalcPath(DataItemBanner owner, Vector2Int origin)
    {
        path = owner.pathfinder.Solve(origin, gridDest);
    }
    public virtual bool Resolve(DataItemBanner owner)
    {
        return true;
    }
    public virtual bool HasResolvedOrder(DataItemBanner owner)
    {
        return owner.GetCoords() == gridDest;
    }
}
