using UnityEngine;

public class ArmyMovementComponent : ArmyComponent
{
    public int movementStarting = 0;
    public int movementLeft = 0;
    public bool movedThisTurn = false;
    public Vector2Int initialPosition;

    public ArmyMovementComponent(DataItemArmy parent) : base(parent)
    {
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
    public bool CanIMove()
    {
        return (movementLeft > 0 && !parent.orders.IsIdle());
    }
    public bool CanPayMovement(int movement)
    {
        return movementLeft > movement;
    }

    public void PayMovement(int value)
    {
        if (value > 0)
            movementLeft -= value;
        else
            movementLeft = 0;
    }
    public bool CanWalkOnTile(SidewaysTile tile)
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
    }
    public void UpdateMaxMovement()
    {
    }
    public override void OnTurnBegin()
    {
        movementLeft = 0;
        UpdateStartingMovement();
    }
    public void UpdateStartingMovement()
    {
        movementStarting = movementLeft;
        initialPosition = parent.gridPos;
    }
    public void ResolveMovement()
    {
        if (CanIMove())
        {
             movedThisTurn = true;
            parent.orders.RecalculateEntirePath();

            while (ShouldIMove())
            {
                var firstOrder = parent.orders.GetCurrentOrder();
                if (firstOrder.HasResolvedOrder(parent))
                {
                    if (firstOrder.Resolve(parent))
                    {
                        parent.orders.AdvanceOrder();
                        return;
                    }
                    else
                    {
                        parent.orders.AdvanceOrder();
                    }
                }
                else
                {
                    var next = firstOrder.path.Next();
                    parent.MoveToTile(next.gridPos, false);
                }
            }
        }
    }
}
