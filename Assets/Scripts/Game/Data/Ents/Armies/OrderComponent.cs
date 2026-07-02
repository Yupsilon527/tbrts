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
        parent.display?.OnPathChange();
    }
    public void ReplaceOrder(Order o)
    {
        ClearOrders();
        GiveOrder(o, 0);
        parent.movement.ResolveMovement();
    }
    public void AdvanceOrder()
    {
        if (orders.Count > 0)
        {
            orders.RemoveAt(0);
            ResolveCurrentOrder();
            parent.display?.OnPathChange();
        }
    }
    public void ClearOrders()
    {
        orders.Clear();
        parent.display?.OnPathChange();
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
