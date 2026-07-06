using UnityEditor;
using UnityEngine;

public class ArmyMovementComponent : ArmyComponent
{
    public int movementStarting = 0;
    public int movementLeft = 0;
    public bool movedThisTurn = false;
    public Vector2Int initialPosition;

    public ArmyMovementComponent(DataItemArmy parent) : base(parent)
    {
        movementLeft = 0;
    }

    public TerrainDefines.Movement GetMyMovement()
    {
        if (parent.formation.transport != null)
        {

            return parent.formation.transport.GetMovetype();
        }

        int total = 666;

        foreach (var Zim in parent.formation.GetUnits())
        {
            total = Mathf.Min(total, (int)Zim.GetMovetype());


        }

        return (TerrainDefines.Movement)total;
    }
    public int GetMyMovementDistance()
    {
        if (parent.formation.transport != null)
        {

            return parent.formation.transport.GetMyMovement();
        }

        int total = 666;

        foreach (var Zim in parent.formation.GetUnits())
        {
            total = Mathf.Min(total, Zim.GetMyMovement());


        }

        return total;
    }
    public bool CanIMove()
    {
        return (movementLeft > 0 && !parent.orders.IsIdle());
    }
    public bool PayAndMove(int movement)
    {
        if (CanPayMovement(movement))
        {
            PayMovement(movement);
            return true;
        }
        return false;
    }
    public bool CanPayMovement(int movement)
    {
        return movementLeft > movement;
    }

    public void PayMovement(int value)
    {

        if (value > 0 && value < movementLeft)
            movementLeft -= value;
        else
            movementLeft = 0;
    }
    public bool CanWalkOnTile(DataItemTile tile)
    {
        if (tile != null)
            return tile.IsPassible(GetMyMovement());
        return false;
    }

    public bool ShouldIMove()
    {
        return (movementLeft > 0 && !parent.orders.IsIdle() && !parent.orders.IsResting());
    }
    public void Teleport(Vector2Int Location)
    {
        parent.MoveToTile(Location, false);
        parent.UpdateLoS(true);
    }
    public void UpdateMaxMovement()
    {
    }
    public override void OnTurnBegin()
    {
        movementLeft = GetMyMovementDistance();
        UpdateStartingMovement();
    }
    public void UpdateStartingMovement()
    {
        movementStarting = movementLeft;
        initialPosition = parent.GetCoords();
    }
    public void ResolveMovement()
    {
        if (CanIMove())
        {
            movedThisTurn = true;
            parent.orders.RecalculateEntirePath();

            while (ShouldIMove())
            {
                if (parent.orders.OrderPass())
                {
                    parent.orders.AdvanceOrder();
                }
                else
                {
                    var firstOrder = parent.orders.GetCurrentOrder();
                    var next = firstOrder?.path?.Next() ?? null;
                    if (next != null && firstOrder.path.failure != Astar.Failure.impossible && firstOrder.path.failure != Astar.Failure.impassible_origin && firstOrder.path.failure != Astar.Failure.impassible_target)
                    {
                        bool walked = parent.MoveToTile(next.gridPos, false);
                        parent.UpdateLoS(!walked);
                        if (!walked)
                            return;
                    }
                    else return;
                }
            }
            parent.display?.OnPathChange();
            parent.UpdateLoS(true);
        }
    }
}
