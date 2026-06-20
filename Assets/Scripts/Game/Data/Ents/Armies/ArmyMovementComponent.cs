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

    public bool ShouldIMove()
    {
        return (movementLeft > 0 && !parent.orders.IsIdle() && !parent.orders.IsResting());
    }

    public void Exhaust(int value)
    {
        movementLeft = 0;
    }
    public void Teleport(Vector2Int Location)
    {
        parent.MoveToTile(Location);
    }
    public void UpdateMaxMovement()
    {

    }
}
