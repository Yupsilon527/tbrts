using UnityEngine;

public abstract class ArmyComponent 
{
    public DataItemArmy parent;
    public virtual void OnTurnBegin() { }
}
