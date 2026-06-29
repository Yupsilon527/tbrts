using System;
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
    public string[] abilitiesAdded = new string[0];

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

        foreach (var prop in properties)
            unit.upgrades.UpdateProperty(prop.Property, prop.value * delta);

        if (oldLevel == 0 && newLevel > 0)
            foreach (var stat in states)
                unit.upgrades.UpdateState(stat.State, (int)stat.priority);
        else if (newLevel == 0)
            foreach (var stat in states)
                unit.upgrades.UpdateState(stat.State, 0);

        unit.bonuses.GrantBonusDamageFromTable(bonusDamage, oldLevel, newLevel);

        if (delta > 0)
        {
            foreach (var innate in abilitiesAdded)
                unit.innates.AddAbility(innate, delta);
        }
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