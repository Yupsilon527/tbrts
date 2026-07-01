using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatantAbilities : UnitComponent, CombatantTicker
{
    public int nextTick = 0;
    public ResourceInt Ap = new ResourceInt(1, "AP", false, false);
    public ResourceInt Mp = new ResourceInt(1, "MP", false, false);
    public ResourceInt Sp = new ResourceInt(1, "SP", false, false);

    public HashSet<PropertyAbility> actions = new();
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
        else if (act == AbilityDefines.Event.CombatBegin)
        {
            nextTick = 0;
            Ap.SetPercentage(1);
            Mp.SetPercentage(1);
        }
        else if (act == AbilityDefines.Event.CombatPhase)
        {
            foreach (var atk in GetAttacks())
                if (atk.original.attackPhase == Combat.main.currentPhase)
                {
                    atk.SetCooldown(Combat.main.currentTick + Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient * atk.startupDelay));
                    atk.uses = 0;
                }
        }
        else if (act == AbilityDefines.Event.OnTurnBegin)
        {
            if (parent.troop.tile.buildingLayer is DataItemCastle city
                && city.GetAlignment(parent) == PlayerDefines.Alignment.ally
                && city.AmIUnderAlliedControl())
                Sp.SetPercentage(1);
        }
        base.TriggerFuncs(act);
    }
    public void ClearAbilities()
    {
        actions.Clear();
    }
    public void FromCombatantData()
    {
        foreach (var ability in parent.data.attacks)
        {
            AddAbility(ability);
        }
        nextTick = GetNextTick();
    }
    public PropertyWeapon[] GetAttacks()
    {
        return actions.Select(a => a is PropertyWeapon atk ? atk : null).ToArray();
    }
    public PropertySpell[] GetSpells()
    {
        return actions.Select(a => a is PropertySpell spell ? spell : null).ToArray();
    }

    public virtual void AddAbility(PropertyAbility ability, bool active = false)
    {
        actions.Add(ability);
        ability.FireEvent(AbilityDefines.Event.OnCreated);
    }
    public virtual void RemoveAbility(PropertyAbility ability)
    {
        ability.FireEvent(AbilityDefines.Event.OnDestroyed);
        actions.Remove(ability);
    }
    public void AddAbility(ActionData ability, bool active = false)
    {
        if (ability is WeaponData w)
            AddAbility(new PropertyWeapon(parent, w));
        if (ability is SpellData s)
            AddAbility(new PropertySpell(parent, s));
    }
    public void RemoveAbility(ActionData ability)
    {
        foreach (var ab in actions)
        {
            if (ab.InternalName == ability.InternalName)
                RemoveAbility(ab);
        }
    }
    public PropertyAbility[] GetAvailableAbilities(CombatDefines.AttackPhase phase, bool castable)
    {
        available.Clear();
        if (!castable || SanityCheck())
        {
            foreach (PropertyWeapon ability in actions)
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
    public void Tick(int currentTick)
    {
        Trigger(Combat.main.currentPhase, currentTick);
    }
    public int GetNextTick()
    {
        int ticks = int.MaxValue;
        foreach (var action in actions)
        {
            ticks = Mathf.Min(ticks, action.nextTime);
        }
        return ticks;
    }
    public bool Trigger(CombatDefines.AttackPhase phase, int currentTick)
    {
        var abilities = GetAttacks();

        foreach (var action in abilities.Where(a => a.CanBeCast(phase) && a.GetValidTargets(parent).Length > 0))
        {
            if (action.nextTime <= currentTick)
            {
                while (action.nextTime <= currentTick && action.HasResourcesToCast() && action.GetValidTargets(parent).Length > 0)
                {
                    Combat.main.Inspect($"Combatant {parent} performs action {action.InternalName} at tick {currentTick}");

                    var target = action.GetBestTargetForAbility(parent);
                    var castData = new AttackTable(phase, currentTick, parent, target.gridPos, action);
                    action.CastFromTable(castData);
                }
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Events
    public void EventReaction(AbilityDefines.Event evtData, DataItemUnit other)
    {
        foreach (PropertyAbility ability in actions)
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
        foreach (var action in actions)
        {
            output += action.ToString() + "<br>";
        }
        return output;
    }
}
