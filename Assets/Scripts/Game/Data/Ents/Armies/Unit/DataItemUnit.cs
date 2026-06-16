using UnityEngine;

public class DataItemUnit : DataItemObject
{
    public int nextAction = 0;
    public int hitCounter = 1;
    public int dodgeCounter = 1;

    public UnitStatsTable scriptable;
    public UnitStats stats;
    public UnitDamageable damageable;
    public CombatantAbilities abilities;
    public CombatantModifiers modifiers;

    public virtual bool IsPlayerOwned()
    {
        return false;
    }
    public DataItemUnit(UnitStatsTable table, int level)
    {
        scriptable = table;
        stats = new(this, table, level);
        damageable = new(this);
        abilities = new(this);
        modifiers = new(this);
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
    public virtual bool GetState(ModifierDefines.modStates State)
    {
        return modifiers.GetState(State);
    }
    public float GetProperty(ModifierDefines.Properties Property)
    {
        if (ModifierDefines.IsPropertyMultiplicative(Property))
            return modifiers.GetPropertyMultiplicative(Property);
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyAdditive(ModifierDefines.Properties Property)
    {
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Properties Property)
    {
        return modifiers.GetPropertyMultiplicative(Property);
    }
    #endregion
    public DataItemUnit GetAttackTarget()
    {
        var targets = IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
        return targets[0];
    }
    public string OutputTable()
    {
        string output = stats.realStats.OutputTable();
        // Properties (non-zero values only)
        bool hasProperties = false;
        string propertiesOutput = "";
        for (int i = 0; i < (int)ModifierDefines.Properties.total; i++)
        {
            ModifierDefines.Properties prop = (ModifierDefines.Properties)i;
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
        for (int i = 0; i < (int)ModifierDefines.modStates.total; i++)
        {
            if (GetState((ModifierDefines.modStates)i))
            {
                hasStates = true;
                ModifierDefines.modStates state = (ModifierDefines.modStates)i;
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
}
