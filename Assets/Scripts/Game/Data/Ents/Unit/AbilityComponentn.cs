using System;
using System.Collections.Generic;

public class AbilityComponentn : UnitComponent
{
    public Dictionary<UnitDefines.ArmyAbilities, int> abilities = new();

    public AbilityComponentn(DataItemUnit parent) : base(parent)
    {
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
        else
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
        return abilities[ability];
    }
}

[Serializable]
public class AbilityData
{
    public string abilityID;
    public int abilityLevel;
}