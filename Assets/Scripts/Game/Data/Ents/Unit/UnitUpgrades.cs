using System.Collections.Generic;
using System.Linq;

public class UnitUpgrades : UnitProperties
{
    public UpgradeList upgrades;

    public UnitUpgrades(DataItemUnit parent) : base(parent)
    {
        upgrades = new();
        upgrades.onUpgradeLevelChange += (upgrade, oldLevel, newLevel) => {
            upgrade.SetUnitLevel(this, false, oldLevel, newLevel);
        };
    }

}

