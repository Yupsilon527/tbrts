using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatantAbilities : UnitComponent, CombatantTicker
{
    public int lastTick = 0;

    public ResourceInt Ap;
    public ResourceInt Mp;

    public HashSet<PropertyWeapon> attacks = new();
    public HashSet<PropertyWeapon> available = new();
    public CombatantAbilities(DataItemUnit parent) : base(parent)
    {
    }
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        if (act == AbilityDefines.Event.OnRefresh
            || act == AbilityDefines.Event.OnSpawn)
        {
            ClearAbilities();
            FromCombatantData();
        }
        else if (act ==  AbilityDefines.Event.CombatBegin)
        {
            lastTick = 0;
            Ap.SetPercentage(1);
            Mp.SetPercentage(1);
        }
        base.TriggerFuncs(act);
    }
    public void ClearAbilities()
    {
        attacks.Clear();
    }
    public void FromCombatantData()
    {
        ClearAbilities();
        foreach (var ability in parent.data.weapons)
        {
            if (ability != null) AddAbility(ability);
        }
    }


    public virtual void AddAbility(PropertyWeapon ability, bool active = false)
    {
        attacks.Add(ability);
        ability.FireEvent(AbilityDefines.Event.OnCreated);
        ability.SetCooldown(Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient * ability.actionDelay));
    }
    public virtual void RemoveAbility(PropertyWeapon ability)
    {
        ability.FireEvent(AbilityDefines.Event.OnDestroyed);
        attacks.Remove(ability);
    }
    public bool Tick(int steps)
    {
        int tickDelta = steps - lastTick;
        lastTick = steps;
        return Trigger(Combat.main.currentPhase, tickDelta);
    }
    public int GetNextTick(int steps)
    {
        int ticks = int.MaxValue;
        foreach (var action in attacks)
        {
            ticks = Mathf.Min(ticks, steps + action.expiration);
        }
        return ticks;
    }
    public PropertyAbility[] GetAvailableAbilities(CombatDefines.AttackPhase phase, bool castable)
    {
        available.Clear();
        if (!castable || SanityCheck())
        {
            foreach (PropertyWeapon ability in attacks)
            {
                if (ability == null)
                    continue;
                if (castable && ability.CanBeCast(phase))
                    available.Add(ability);
            }
        }
        return available.ToArray();
    }

    #region Casting
    public bool Trigger( CombatDefines.AttackPhase phase, int ticks)
    {
        var abilities = attacks.Where(a => a.CanBeCast(phase) );

        foreach (var action in abilities)
        {
            if (action.ForwardTime(ticks))
            {
                while (action.expiration <= 0)
                {
                    int currentTick = Combat.main.currentTick;
                    Combat.main.Inspect($"Combatant {parent.data.InternalName} performs action {action.InternalName} at turn {currentTick}");

                    var target = action.GetBestUnitForAbility();
                    var castData = new AttackTable(phase,currentTick, parent, target.gridPos, action);
                    action.CastFromTable(castData);
                }
                return true;
            }
        }
        return ticks == 0;
    }
    #endregion

    #region Events
    public void EventReaction(AbilityDefines.Event evtData, DataItemUnit other)
    {
        foreach (PropertyAbility ability in attacks)
        {
            if (ability == null)
                continue;
            ability.FireEvent(evtData, other);
        }
    }
    #endregion

    public virtual string OutputTable()
    {
        string output = "<br><b>Actions</b><br>";
        foreach (var action in attacks)
        {
            output += action.ToString() + "<br>";
        }
        return output;
    }
}
