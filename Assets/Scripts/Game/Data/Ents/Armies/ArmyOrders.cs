using UnityEngine;

public class ArmyOrders : ArmyComponent
{
    public ArmyOrders(DataItemArmy parent) : base(parent)
    {
    }

    public bool IsIdle()
    {
        return true;
    }
    public bool IsResting()
    {
        return true;
    }
    public void Clear()
    {

    }
}
