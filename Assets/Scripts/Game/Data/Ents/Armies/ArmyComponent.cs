using UnityEngine;

public abstract class ArmyComponent 
{
    public DataItemArmy parent;

    protected ArmyComponent(DataItemArmy parent)
    {
        this.parent = parent;
    }

    public virtual void OnTurnBegin() { }
}
