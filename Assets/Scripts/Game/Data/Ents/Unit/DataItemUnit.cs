using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemUnit : DataItemObject
{
    public int nextAction = 0;
    public int hitCounter = 1;
    public int critCounter = 1;
    public int dodgeCounter = 1;

    public UnitData scriptable;
    public DataItemArmy troop;
    public UnitStats stats;
    public UnitDamageable damageable;
    public CombatantAbilities abilities;
    public UnitBonuses bonuses;
    public CombatantModifiers modifiers;
    public AbilityComponentn innates;

    public virtual bool IsPlayerOwned()
    {
        return false;
    }
    public DataItemUnit(UnitData table)
    {
        scriptable = table;
        stats = new(this, table.unit);
        damageable = new(this);
        abilities = new(this);
        modifiers = new(this);
        bonuses = new(this);
        innates = new(this);
    }
    #region events
    public void FireEventOnSelf(AbilityDefines.Event evtData, bool refresh = false)
    {
        HandleEvent(evtData, new DataItemUnit[0], refresh);
    }
    public void FireEventOnTarget(AbilityDefines.Event evtData, DataItemUnit target, bool refresh = false)
    {
        HandleEvent(evtData, new DataItemUnit[] { target }, refresh);
    }
    public virtual void HandleEvent(AbilityDefines.Event evt, DataItemUnit[] targets, bool refresh = false)
    {
        abilities.TriggerFuncs(evt);
        modifiers.EventReaction(evt, targets);
        if (evt == AbilityDefines.Event.CombatBegin)
        {
            UpdateNextAction(0);
        }
    }
    #endregion
    public void Act()
    {
        Act(nextAction);
    }
    public void Act(int steps)
    {
        abilities.Tick(steps);
        modifiers.Tick(steps);
        UpdateNextAction(steps);
    }
    public bool CanAct(CombatDefines.AttackPhase phase)
    {
        return abilities.attacks.Any(a => a.original.attackPhase == phase && a.HasResourcesToCast());
    }
    protected void UpdateNextAction(int steps)
    {
        nextAction = Mathf.Min(abilities.GetNextTick(steps), modifiers.GetNextTick(steps));
    }
    #region States
    public virtual void Refresh(bool force = false)
    {
        FireEventOnSelf(AbilityDefines.Event.OnRefresh);
        modifiers.Refresh(force);
    }
    public virtual bool GetState(ModifierDefines.State State)
    {
        return modifiers.GetState(State);
    }
    public float GetProperty(ModifierDefines.Property Property)
    {
        if (ModifierDefines.IsPropertyMultiplicative(Property))
            return modifiers.GetPropertyMultiplicative(Property);
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyAdditive(ModifierDefines.Property Property)
    {
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Property Property)
    {
        return modifiers.GetPropertyMultiplicative(Property);
    }
    #endregion
    public string OutputTable()
    {
        string output = stats.realStats.OutputTable();
        // Properties (non-zero values only)
        bool hasProperties = false;
        string propertiesOutput = "";
        for (int i = 0; i < (int)ModifierDefines.Property.total; i++)
        {
            ModifierDefines.Property prop = (ModifierDefines.Property)i;
            bool multi = ModifierDefines.IsPropertyMultiplicative(prop);
            float value = GetProperty(prop);
            if ((multi && value != 1) || (!multi && value != 0))
            {
                hasProperties = true;
                float displayValue = multi ? Mathf.Round(value * 100) : Mathf.Round(value * 100) / 100;
                propertiesOutput += $"{prop}: {(displayValue > 0 ? "+" : "")}{displayValue}{(multi ? "%" : "")}<br>";
            }
        }
        if (hasProperties)
        {
            output += "<br><b>Properties</b><br>";
            output += propertiesOutput;
        }

        // States (active states only)
        bool hasStates = false;
        string statesOutput = "";
        for (int i = 0; i < (int)ModifierDefines.State.total; i++)
        {
            if (GetState((ModifierDefines.State)i))
            {
                hasStates = true;
                ModifierDefines.State state = (ModifierDefines.State)i;
                statesOutput += $"{state}<br>";
            }
        }
        if (hasStates)
        {
            output += "<br><b>States</b><br>";
            output += statesOutput;
        }
        output += abilities.OutputTable();
        return output;
    }
    public bool IsInCombat()
    {
        return troop.IsInCombat();
    }
    public int GetCommandValue()
    {
        return 1 + innates.GetAbilityLevel("command");
    }
    public int GetPowerValue(bool accountPenalty)
    {
        return 0;
    }

    public TerrainDefines.Movement GetMovetype()
    {

        if (innates.GetAbilityLevel("ghost") > 0)
        {
            return TerrainDefines.Movement.Ghost;
        }
        else   if (innates.GetAbilityLevel("ghost") > 0)
        {
            return TerrainDefines.Movement.Ghost;
        }
        else if (innates.GetAbilityLevel("fly") > 0)
        {
            return TerrainDefines.Movement.Fly;
        }
        else if (innates.GetAbilityLevel("teleport") > 0)
        {
            return TerrainDefines.Movement.Teleport;
        }
        else if (innates.GetAbilityLevel("wheels") > 0)
        {
            return TerrainDefines.Movement.Wheels;
        }
        else if (innates.GetAbilityLevel("giant") > 0)
        {
            return TerrainDefines.Movement.GroundGiant;
        }
        else if (innates.GetAbilityLevel("foot") > 0)
        {
            return TerrainDefines.Movement.GroundFoot;
        }
        else if (innates.GetAbilityLevel("amphibian") > 0)
        {
            return TerrainDefines.Movement.Amphibian;
        }
        else if (innates.GetAbilityLevel("swim") > 0)
        {
            return TerrainDefines.Movement.Swimmer;
        }
        else if (innates.GetAbilityLevel("seaworthy") > 0)
        {
            return TerrainDefines.Movement.Boat;
        }

        return TerrainDefines.Movement.Ground;
    }

    public float GetPurchaseCost(DataItemPlayer Owner)
    {
        float Cost = stats.realStats.GetPowerValue(innates.abilities);

        Cost -= Stats[DataItemArmy.Stat_Resource] * Game.iArmyCostReduction;

        if (Owner != null)
        {
            if (!Owner.Faction.Armies.Contains(this))
            {
                Cost += 1;
            }
        }

        return Mathf.Max(1, Mathf.CeilToInt(Cost * Game.iArmyBuildMultiplier));
    }
    public float GetUpkeep()
    {
        return Mathf.Max(1, Mathf.CeilToInt(GetPower(true) * Game.iArmySalaryMultiplier));
    }

    public bool isTransport()
    {
        return innates.GetAbilityLevel("transport") > 0;
    }
}