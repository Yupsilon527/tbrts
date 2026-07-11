using System;
using System.Linq;
using UnityEngine;

public class TechData : ProductionData
{
    public enum UpgradeType
    {
        stacking,
        infinite,
        unique
    }
    public UpgradeType infinite = UpgradeType.stacking;
    [Header("Flags")]
    public string[] affectedFlags;

    [Header("Price Change Alteration")]
    public ResourceAlteration[] resourceChanges;

    [Header("Bonus Damage Upgrade For Units")]
    public BonusDamageTable[] bonusDamage = new BonusDamageTable[0];
    [Header("Stat Alterations For Units")]
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
    [Header("Abilities")]
    public SpellData[] tempSpells = new SpellData[0];
    public AbilityData[] abilitiesAdded = new AbilityData[0];

    [Header("Grant Resources/Income")]
    public ResourceCost[] grantedResources = new ResourceCost[0];
    public ResourceIncome[] grantedIncome = new ResourceIncome[0];
    public bool AppliesToThing(ProductionData data)
    {
        if (affectedFlags.Length == 0) return true;
        foreach (var check in affectedFlags)
        {
            if (check[0] == '_')
            {
                if (check.Substring(1).ToLower() == data.InternalName.ToLower())
                    return true;
            }
            else
            {
                foreach (var flag in data.flags)
                {
                    if (check == flag) return true;
                }
            }
        }
        return false;
    }
    public virtual void SetUnitLevel(DataItemUnit unit, bool onSpawn, int oldLevel, int newLevel)
    {
        int delta = newLevel - oldLevel;


        if (oldLevel == 0 && newLevel > 0)
        {
            foreach (var stat in states)
                unit.upgrades.UpdateState(stat.State, (int)stat.priority);
            foreach (var prop in properties)
                unit.upgrades.UpdateProperty(prop.Property, prop.value + prop.IncreasePerLevel * delta);
            foreach (var innate in abilitiesAdded)
                unit.innates.AddAbility(innate.abilityID, innate.abilityLevel * newLevel, UnitDefines.UpgradeCondition.upgrade);
        }
        else if (newLevel == 0)
        {
            foreach (var stat in states)
                unit.upgrades.UpdateState(stat.State, 0);
            foreach (var prop in properties)
                unit.upgrades.UpdateProperty(prop.Property, prop.IncreasePerLevel * delta - prop.value);
            foreach (var innate in abilitiesAdded)
            {
                int expectedLevel = innate.abilityLevel * oldLevel;
                var found = unit.innates.abilities.Where(a => a.abilityTemp == UnitDefines.UpgradeCondition.upgrade && a.abilityID.ToString().ToLower() == innate.abilityID && a.abilityLevel == expectedLevel).ToArray();
                if (found.Length > 0)
                {
                    unit.innates.abilities.Remove(found[0]);
                }
            }
        }
        else if (delta != 0)
        {
            foreach (var prop in properties)
                unit.upgrades.UpdateProperty(prop.Property, prop.IncreasePerLevel * delta);

            foreach (var innate in abilitiesAdded)
            {
                int expectedOldLevel = innate.abilityLevel * oldLevel;
                int expectedNewLevel = innate.abilityLevel * newLevel;

                var found = unit.innates.abilities
                    .Where(a => a.abilityTemp == UnitDefines.UpgradeCondition.upgrade
                                && a.abilityID.ToString().ToLower() == innate.abilityID
                                && a.abilityLevel == expectedOldLevel)
                    .ToArray();

                if (found.Length > 0)
                {
                    found[0].abilityLevel = expectedNewLevel;
                }
                else
                {
                    unit.innates.AddAbility(innate.abilityID, expectedNewLevel, UnitDefines.UpgradeCondition.upgrade);
                }
            }
        }

        foreach (var spell in tempSpells)
        {
            if (delta > 0)
                unit.actions.AddAbility(spell);
            else
                unit.actions.RemoveAbility(spell);
        }

        unit.bonuses.GrantBonusDamageFromTable(bonusDamage, oldLevel, newLevel);
    }
}

[Serializable]
public class ResourceAlteration
{
    public enum ChangeBehavior
    {
        nothing,
        raw,
        percentage
    }
    public EconomyDefines.EconomyResource changedResource = EconomyDefines.EconomyResource.Metal;
    public ChangeBehavior resourceChangeBehavior = ChangeBehavior.nothing;
    public float changeValue = 0;
}