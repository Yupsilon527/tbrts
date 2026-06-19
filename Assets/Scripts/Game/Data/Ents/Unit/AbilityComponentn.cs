using System;
using System.Collections.Generic;

public class AbilityComponentn : UnitComponent
{
    public Dictionary<string, int> abilities = new();

    public AbilityComponentn(DataItemUnit parent) : base(parent)
    {
    }
    public void AddAbility()
    {

    }
    public bool HasAbility(string ability)
    {
        return abilities.ContainsKey(ability);
    }
    public int GetAbilityLevel(string ability)
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