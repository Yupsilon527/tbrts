using Astar;
using System.Collections.Generic;
using UnityEngine;

public class OrderComponent : ArmyComponent
{
    public List<Order> orders = new();

    public OrderComponent(DataItemArmy parent) : base(parent)
    {
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
    public void GiveOrder(Order o, int index)
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
    }

    private void Update()
    {
        if (OrderPass())
        {
            AdvanceOrder();
        }

    }
    bool OrderPass()
    {
        var currentOrder = GetCurrentOrder();
        if (currentOrder != null && currentOrder.HasResolvedOrder(parent))
        {
            currentOrder.Conclude(parent);
            return true;

        }
        return false;
    }

    internal bool IsIdle()
    {
        return GetCurrentOrder() == null;
    }
}

public class Order
{
    public enum ID { Move = 0, Follow = 1, Rest = 2, Raze = 3 };
    public ID OrderID;
    public Vector2Int gridDest;
    public DataItemArmy TargetUnit;
    PathfinderPath path;
    public void Resolve(DataItemArmy owner)
    {
        if (TargetUnit != null)
        {
            if (!TargetUnit.IsVisibleToAnother(owner))
                Fail();
            if (TargetUnit.gridPos != gridDest)
            {
                gridDest = TargetUnit.gridPos;
                path = null;
            }
        }
        if (path == null)
        {
            path = owner.pathfinder.Solve(owner.gridPos, gridDest);
        }
    }
    public bool HasResolvedOrder(DataItemArmy owner)
    {
        if (TargetUnit != null)
            return owner.tile.IsAdjecent(gridDest);
        else return owner.tile.gridPos == gridDest;
    }
    public void Conclude(DataItemArmy owner)
    {

    }
    void Fail()
    {

    }
}