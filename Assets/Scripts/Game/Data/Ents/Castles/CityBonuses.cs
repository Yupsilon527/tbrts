using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CityBonuses : CityComponent
{
    public UpgradeList upgrades;
    public CityBonuses(DataItemCastle city) : base(city)
    {
        upgrades = new UpgradeList();
        upgrades.onUpgradeLevelChange += (upgrade, oldLevel, newLevel) => {
            BuildBuilding(upgrade, newLevel - oldLevel) ;
        };
    }
    public void BuildBuilding(TechData upgrade, int levels)
    {
        upgrades.CompleteUpgrade(upgrade, levels);
        foreach (var army in city.GetGarrison())
        {
            ApplyUpgradeToAllUnits(army, upgrade, levels);
        }
    }
    void ApplyUpgradeToAllUnits(DataItemArmy army, TechData upgrade, int levels)
    {
        foreach (var unit in army.formation.GetUnits())
        {
            unit.upgrades.upgrades.CompleteUpgrade(upgrade, levels);
        }
    }
    public void ApplyResearchedUpgradeToNewlySpawnedUnit(DataItemUnit unit)
    {
        foreach (var upgrade in upgrades.researchedUpgrades)
        {
            unit.upgrades.upgrades.CompleteUpgrade(upgrade.upgrade, upgrade.level);
        }
    }

}
