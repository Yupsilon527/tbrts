using System;
using System.Collections.Generic;

public class AbilityComponentn : UnitComponent
{
    public Dictionary<UnitDefines.ArmyAbilities, int> abilities = new();

    public AbilityComponentn(DataItemUnit parent) : base(parent)
    {
        foreach ( var innate in parent.data.abilities)
        {
            AddAbility(innate.abilityID, innate.abilityLevel);
        }
    }
    public void AddAbility(string name, int level)
    {
        if (Enum.TryParse(name, true, out UnitDefines.ArmyAbilities ability))
            AddAbility(ability, level); 
    }
    public void AddAbility(UnitDefines.ArmyAbilities ability, int level)
    {
        if (abilities.ContainsKey(ability))
        {
            abilities[ability] += level;
        }
        else if (level >0)
        {
            abilities.Add(ability, level);
        }
    }
    public bool HasAbility(UnitDefines.ArmyAbilities ability)
    {
        return abilities.ContainsKey(ability);
    }
    public bool HasAbility(string name)
    {
        if (Enum.TryParse(name, true, out UnitDefines.ArmyAbilities ability))
            return HasAbility(ability);
        return false;
    }
    public int GetAbilityLevel(string name)
    {
        if (Enum.TryParse(name, true, out UnitDefines.ArmyAbilities ability))
        return GetAbilityLevel(ability);
        return 0;
    }
    public int GetAbilityLevel(UnitDefines.ArmyAbilities ability)
    {
        if (abilities.ContainsKey(ability))
            return abilities[ability];
        return 0;
    }
    public int GetAbilityCombined(UnitDefines.ArmyAbilities ability)
    {
        return GetAbilityLevel(ability) + parent.troop?.GetAuraBonuses(ability) ?? 0;
    }
}

[Serializable]
public class AbilityData
{
    public string abilityID;
    public int abilityLevel;
}