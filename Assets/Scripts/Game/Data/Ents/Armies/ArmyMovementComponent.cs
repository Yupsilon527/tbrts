using UnityEngine;

public class ArmyMovementComponent : ArmyComponent
{
    public int movementStarting = 0;
    public int movementLeft = 0;
    public bool movedThisTurn = false;
    public Vector2Int initialPosition;

    public int GetMyMovement()
    {
        if (parent.formation.transport != null)
        {

            return parent.formation.transport.GetMyMovement();
        }

        float total = 666;

        foreach (entityUnit Zim in getUnits(false))
        {

            total = Mathf.Min(total, Zim.GetMyMovement());


        }

        return (int)total;
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

}
