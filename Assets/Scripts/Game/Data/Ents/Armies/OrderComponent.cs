using Astar;
using System.Collections.Generic;
using UnityEngine;

public class OrderComponent : ArmyComponent
{
    public List<Order> orders = new();

    public OrderComponent(DataItemArmy parent) : base(parent)
    {
    }
    public bool IsIdle()
    {
        return NumOrders() == 0;
    }
    public bool IsResting()
    {
        return NumOrders() > 0 && orders[0].OrderID == Order.ID.Rest;
    }
    public void Clear()
    {

    }
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        if (orders.Count > 0 && orders[0].OrderID == Order.ID.Rest)
        {
            orders.RemoveAt(0);
        }
    }
    public void RecalculateEntirePath()
    {
        parent.pathfinder.movement = parent.movement.GetMyMovement();
        int index = -1;
        foreach (var order in orders)
        {
            order.RecalcPath(parent, index < 0 ? parent.gridPos : orders[index].gridDest);
            index++;
        }
        parent.display?.OnPathChange();
    }
    public Order GetCurrentOrder()
    {
        if (orders.Count == 0) return null;
        return orders[0];
    }
    public int NumOrders()
    {
        return orders.Count;
    }
    public void GiveOrder(Order o, int index = 999)
    {
        if (o.OrderID == Order.ID.Rest)
        {
            orders.Clear();
        }
        if (index >= orders.Count)
        {
            orders.Add(o);
        }
        else
        {
            orders.Insert(Mathf.Max(index, 0), o);
        }
        if (orders.Count == 1 || index < 1)
            ResolveCurrentOrder();
        else
            RecalculateEntirePath();
    }
    public void ReplaceOrder(Order o)
    {
        ClearOrders();
        GiveOrder(o, 0);
    }
    public void AdvanceOrder()
    {
        if (orders.Count > 0)
        {
            orders.RemoveAt(0);
            ResolveCurrentOrder();
        }
    }
    public void ClearOrders()
    {
        orders.Clear();
    }
    void ResolveCurrentOrder()
    {
        if (orders.Count > 0)
        {
            GetCurrentOrder().Resolve(parent);
        }
        parent.display?.OnPathChange();
    }

    public bool OrderPass()
    {
        var currentOrder = GetCurrentOrder();
        if (currentOrder != null && currentOrder.HasResolvedOrder(parent) && currentOrder.Resolve(parent))
        {
            return true;

        }
        return false;
    }

}

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

    public virtual void RecalcPath(DataItemArmy owner, Vector2Int origin)
    {
        path = owner.pathfinder.Solve(origin, gridDest);
    }
    public virtual bool Resolve(DataItemArmy owner)
    {
        return true;
    }
    public virtual bool HasResolvedOrder(DataItemArmy owner)
    {
        return owner.tile.gridPos == gridDest;
    }
    public virtual void Cancel()
    {

    }
}
public class RazeOrder : Order
{
    public RazeOrder(ID orderID, Vector2Int gridDest) : base(orderID, gridDest)
    {
    }

    public override bool Resolve(DataItemArmy attacker)
    {
        if (attacker.tile.buildingLayer is DataItemCastle city)
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
public class FollowOrder : Order
{
    public DataItemArmy TargetUnit;

    public FollowOrder(ID orderID, Vector2Int gridDest, DataItemArmy targetUnit) : base(orderID, gridDest)
    {
        TargetUnit = targetUnit;
    }

    public virtual bool TargetValid(DataItemArmy owner)
    {
        if (TargetUnit != null)
        {
            if (!TargetUnit.IsVisibleToAnother(owner))
                return false;
            return true;
        }
        return false;
    }
    public override void RecalcPath(DataItemArmy owner, Vector2Int origin)
    {
        if (TargetValid(owner))
            gridDest = TargetUnit.gridPos;
        base.RecalcPath(owner, origin);
    }
    public override bool HasResolvedOrder(DataItemArmy owner)
    {
        return TargetValid(owner) && base.HasResolvedOrder(owner);
    }
}