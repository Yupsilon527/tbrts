
using UnityEditor.PackageManager;

public class PlayerUpgrades : PlayerComponent
{
    public UpgradeList upgrades;
    public PlayerUpgrades(DataItemPlayer player) : base(player)
    {
        upgrades = new();
        upgrades.onUpgradeLevelChange += (upgrade, oldLevel, newLevel) => {
            ApplyUpgradeToAllUnits(upgrade, oldLevel, newLevel);
        };
            }
    void ApplyUpgradeToAllUnits(TechData upgrade, int oldLevel, int newLevel)
    {
        foreach (var troop in player.units)
        {
            foreach (var unit in troop.formation.GetUnits())
                unit?.upgrades.upgrades.CompleteUpgrade(upgrade,  newLevel - oldLevel);
        }
    }
    public void ApplyResearchedUpgradeToNewlySpawnedUnit(DataItemUnit unit)
    {
        foreach (var upgrade in upgrades.researchedUpgrades)
        {
            unit.upgrades.upgrades.CompleteUpgrade(upgrade.upgrade,  upgrade.level);
        }
    }
}
