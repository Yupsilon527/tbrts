using UnityEngine;
using VikingParty;

public class UnitComponent 
{
    public DataItemUnit parent;
    public UnitComponent(DataItemUnit parent)
    {
        this.parent = parent;
    }
    protected virtual bool SanityCheck()
    {
        return parent.damageable.IsAlive();
    }
    public virtual void Spawn()
    {

    }
    public virtual void TriggerFuncs(AbilityDefines.Event act)
    {
    }
}
