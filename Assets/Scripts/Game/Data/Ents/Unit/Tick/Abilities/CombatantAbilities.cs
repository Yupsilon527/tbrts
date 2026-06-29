using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatantAbilities : UnitComponent, CombatantTicker
{
    public int lastTick = 0;

    public ResourceInt Ap;
    public ResourceInt Mp;
    public ResourceInt Sp;

    public HashSet<PropertyAbility> abilities = new();
    public HashSet<PropertyAbility> available = new();
    public CombatantAbilities(DataItemUnit parent) : base(parent)
    {
    }
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        if (act == AbilityDefines.Event.OnSpawn)
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
        else if (act ==  AbilityDefines.Event.OnTurnBegin)
        {
            if (parent.tile.buildingLayer is DataItemCastle city 
                && city.GetAlignment(parent) == PlayerDefines.Alignment.ally 
                && city.AmIUnderAlliedControl())
            Sp.SetPercentage(1);
        }
        base.TriggerFuncs(act);
    }
    public void ClearAbilities()
    {
        abilities.Clear();
    }
    public void FromCombatantData()
    {
        foreach (var ability in parent.data.weapons)
        {
            AddAbility(ability);
        }
    }
    public PropertyWeapon[] GetAttacks()
    {
        return abilities.Select(a => a is PropertyWeapon atk ? atk : null).ToArray();
    }
    public PropertySpell[] GetSpells()
    {
        return abilities.Select(a => a is PropertySpell spell ? spell : null).ToArray();
    }

    public virtual void AddAbility(PropertyAbility ability, bool active = false)
    {
        abilities.Add(ability);
        ability.FireEvent(AbilityDefines.Event.OnCreated);
        ability.SetCooldown(Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient * ability.actionDelay));
    }
    public virtual void RemoveAbility(PropertyAbility ability)
    {
        ability.FireEvent(AbilityDefines.Event.OnDestroyed);
        abilities.Remove(ability);
    }
    public  void AddAbility(ActionData ability, bool active = false)
    {
        if (ability is WeaponData w)
            AddAbility(new PropertyWeapon(parent, w));
        if (ability is SpellData s)
            AddAbility(new PropertySpell(parent, s));
    }
    public  void RemoveAbility(ActionData ability)
    {
        foreach (var ab in abilities)
        {
            if (ab.InternalName == ability.InternalName)
                RemoveAbility(ab);
        }
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
        foreach (var action in abilities)
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
            foreach (PropertyWeapon ability in abilities)
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
        var abilities = this.abilities.Where(a => a.CanBeCast(phase) );

        foreach (var action in GetAttacks())
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
        foreach (PropertyAbility ability in abilities)
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
        foreach (var action in abilities)
        {
            output += action.ToString() + "<br>";
        }
        return output;
    }
}
