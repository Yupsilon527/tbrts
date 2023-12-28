using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBaseComponent : MobComponent, IEntityEvent
{
    protected override void Awake()
    {
        base.Awake();
        InitAbilities();
    }
    public abstract void InitAbilities();

    public virtual void AddAbility(PropertyAbility ability, bool active = false)
    {
        ability.FireEvent(AbilityDefines.Event.OnCreated);

            ability.SetActive(active);
    }
    public virtual void RemoveAbility(PropertyAbility ability)
    {
        ability.FireEvent(AbilityDefines.Event.OnDestroy);
    }

    public abstract PropertyAbility[] GetAvailableAbilities(bool castable);
    #region Evebst
    public void EventReaction(AbilityDefines.Event evtData, Mob[] affectedCritters)
    {
        foreach (PropertyAbility ability in GetAvailableAbilities(false))
        {
            if (ability == null)
                continue;
            ability.FireEvent(evtData, affectedCritters);
        }
    }
    #endregion
}
