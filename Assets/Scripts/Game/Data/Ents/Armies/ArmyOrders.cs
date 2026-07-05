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
        return owner.GetCoords() == gridDest;
    }
}
public class RazeOrder : Order
{
    public RazeOrder(ID orderID, Vector2Int gridDest) : base(orderID, gridDest)
    {
    }

    public override bool Resolve(DataItemArmy attacker)
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
public class FollowOrder : Order
{
    public DataItemArmy TargetUnit;

    public FollowOrder(ID orderID, Vector2Int gridDest, DataItemArmy targetUnit) : base(orderID, gridDest)
    {
        TargetUnit = targetUnit;
    }

    public virtual bool TargetValid(DataItemArmy owner)
    {
        if (TargetUnit != null && TargetUnit.IsAlive())
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
        {
            gridDest = TargetUnit.GetCoords();
            base.RecalcPath(owner, origin);
        }
    }
    public override bool HasResolvedOrder(DataItemArmy owner)
    {
        return !TargetValid(owner) || base.HasResolvedOrder(owner);
    }
}
public class AttackOrder : FollowOrder
{
    public AttackOrder(ID orderID, Vector2Int gridDest, DataItemArmy targetUnit) : base(orderID, gridDest, targetUnit)
    {
    }

    public override bool Resolve(DataItemArmy owner)
    {
        if (TargetValid(owner))
        {
            if (owner.BattleAnother(TargetUnit))
                return false;
            /* SparseIntMap results = Combat.main.MockBattle(owner, TargetUnit, TargetUnit.tile);

             var allUnits = new List<DataItemUnit>();
             allUnits.AddRange(owner.formation.GetUnits());
             allUnits.AddRange(TargetUnit.formation.GetUnits());


             foreach (var result in results._entries)
             {
                 var unit = allUnits.FirstOrDefault(u => u.eID == result.key);
                 Combat.main.Inspect($"Unit {unit} remaining with {result.value} health!");
             }
            */
        }
        return true;
    }

}