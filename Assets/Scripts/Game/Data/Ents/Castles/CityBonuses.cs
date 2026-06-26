
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
        city.production.Revision();
        city.income.Revision();
    }
    public bool HasBuilding(BuildingData b)
    {
        return HasBuilding(b.InternalName.ToLower());
    }
    public bool HasBuilding(string name)
    {
        return upgrades.UpgradeResearched(name);
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
    public override void OnCastleRaze()
    {
        base.OnCastleRaze();
        upgrades.Clear();
    }

    public bool CanBuildBuilding(BuildingData b)
    {
        return b.GetAvailableState(city) == ProductionData.AvailableState.available && !HasBuilding(b);
    }

}
