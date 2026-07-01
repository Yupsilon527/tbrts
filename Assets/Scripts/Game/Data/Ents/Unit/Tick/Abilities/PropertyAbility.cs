using System.Collections.Generic;

public abstract class PropertyAbility : PropertyAction
{
    public float procStrength = 1;
    public int uses = 0;
    public PropertyAbility(DataItemUnit caster)
    {
        parent = caster;
    }
    public virtual DataItemUnit[] GetValidTargets(DataItemUnit caster)
    {
        return new[] { caster };
    }
    public ApplyEffects[] GetAbilityEffects() { return null; }
    public virtual bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return HasResourcesToCast();
    }
    public virtual bool HasResourcesToCast()
    {
        return true;
    }
    public virtual void SpendResources()
    {

    }
    public virtual bool CastFromTable(CastTable table)
    {
        if ( HasResourcesToCast() )
        {
           SpendResources();
            ExtendCooldown(parent.stats.realStats.SpeedCoefficient);
            return true;
        }
        return false;
    }

    #region Events
    public void FireEvent(AbilityDefines.Event fct, DataItemUnit target)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new EventTable(Combat.main.currentTick, parent, target));
    }
    public void FireEvent(AbilityDefines.Event fct)
    {
        FireEvent(fct, new EventTable(Combat.main.currentTick, parent, parent));
    }
    public void FireEvent(AbilityDefines.Event fct, EventTable table)
    {
        if (!HasEvent(fct)) return;
        AbilityEvent(fct, table);
    }
    public bool HasEvent(AbilityDefines.Event evt)
    {
        if (AbilityFunctions != null)
            foreach (var fct in AbilityFunctions)
                if (fct.aEvent == evt)
                    return true;
        return false;
    }
    #endregion
    #region Ability Events
    public List<AbilityDefines.AbilityListener> AbilityFunctions;
    void AbilityEvent(AbilityDefines.Event fct, EventTable table)
    {
        if (AbilityFunctions != null)
        {
            foreach (var item in AbilityFunctions)
            {
                if (item.aEvent == fct)
                {
                    item.aFunction.Invoke(table);
                }
            }
        }
    }
    #endregion
    public abstract DataItemUnit[] GetMainTargets(CastTable table);
    public abstract DataItemUnit[] GetAreaTargets(CastTable table);
}
