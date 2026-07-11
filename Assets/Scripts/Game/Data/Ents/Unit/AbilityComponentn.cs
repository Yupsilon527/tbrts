using System;
using System.Collections.Generic;
using System.Linq;

public class UnitInnates : UnitComponent
{
    public class AbilityStorage
    {
        public UnitDefines.ArmyAbilities abilityID;
        public UnitDefines.UpgradeCondition abilityTemp;
        public int abilityLevel;

        public AbilityStorage(UnitDefines.ArmyAbilities abilityID, UnitDefines.UpgradeCondition abilityTemp, int abilityLevel)
        {
            this.abilityID = abilityID;
            this.abilityTemp = abilityTemp;
            this.abilityLevel = abilityLevel;
        }
    }
    public HashSet<AbilityStorage> abilities = new();

    public UnitInnates(DataItemUnit parent) : base(parent)
    {
        foreach ( var innate in parent.data.abilities)
        {
            AddAbility(innate.abilityID, innate.abilityLevel,UnitDefines.UpgradeCondition.permanent);
        }
    }
    public void AddAbility(string name, int level, UnitDefines.UpgradeCondition temp)
    {
        if (Enum.TryParse(name, true, out UnitDefines.ArmyAbilities ability))
            AddAbility(ability, level,temp); 
    }
    public void AddAbility(UnitDefines.ArmyAbilities ability, int level, UnitDefines.UpgradeCondition temp)
    {
        foreach (var abil in abilities)
        {
            if (abil.abilityID == ability && abil.abilityTemp == temp)
            {
                abil.abilityLevel += level;
                return;
            }
        }
        abilities.Add(new(ability, temp, level));
    }
    public bool HasAbility(UnitDefines.ArmyAbilities ability)
    {
        return abilities.Any(a => a.abilityID == ability);
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
        return abilities.Sum(a => a.abilityID == ability ? a.abilityLevel : 0);
    }
    public int GetAbilityCombined(UnitDefines.ArmyAbilities ability)
    {
        return GetAbilityLevel(ability) + parent.troop?.GetAuraBonuses(ability) ?? 0;
    }
    public void ClearTempAbilities()
    {
        foreach (var a  in abilities.ToArray())
        {
            if (a.abilityTemp == UnitDefines.UpgradeCondition.temp)
                abilities.Remove(a);
        }
    }
}

[Serializable]
public class AbilityData
{
    public string abilityID;
    public int abilityLevel;
}