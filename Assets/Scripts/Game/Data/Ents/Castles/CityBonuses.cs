using System.Linq;

public class CityBonuses : CityComponent
{
    public UpgradeList upgrades;
    public CityBonuses(DataItemCastle city) : base(city)
    {
        upgrades = new UpgradeList();
        upgrades.onUpgradeLevelChange += (upgrade, oldLevel, newLevel) => {
            BuildBuilding(upgrade, newLevel - oldLevel,true) ;
        };
    }
    public void BuildBuilding(TechData upgrade, int levels, bool revise)
    {
        upgrades.CompleteUpgrade(upgrade, levels);

        int level = upgrades.GetUpgradeLevel(upgrade);
        foreach (var army in city.GetGarrison())
        {
            ApplyUpgradeToAllUnits(army, upgrade, level);
        }
        GrantFreeBuildings();
        if (revise)
        {
            city.production.Revision();
            city.income.Revision();
        }
    }
    public void GrantFreeBuildings()
    {
        var playerOwner = city.GetPlayerOwner();
        foreach (var b in playerOwner.faction.GetAvailableUpgrades(false))
        {
            if (!HasBuilding(b) && b.GetAvailableState(playerOwner, city) == ProductionData.AvailableState.available && b.IsCompletelyFree(playerOwner))
            {
                BuildBuilding(b, 1, false);
            }
        }
    }
    public bool HasBuilding(BuildingData b)
    {
        return HasBuilding(b.InternalName);
    }
    public bool HasBuilding(string name)
    {
        return upgrades.UpgradeResearched(name);
    }
    void ApplyUpgradeToAllUnits(DataItemArmy army, TechData upgrade, int level)
    {
        if (army == null) return;
        foreach (var unit in army.formation.GetUnits())
        {
            unit.upgrades.upgrades.CatchUpUpgrade(upgrade, level);
        }
    }
    public void ApplyResearchedUpgradeToNewlySpawnedUnit(DataItemUnit unit)
    {
        foreach (var upgrade in upgrades.researchedUpgrades)
        {
            if (upgrade.upgrade.AppliesToThing(unit.data))
            unit.upgrades.upgrades.CompleteUpgrade(upgrade.upgrade, upgrade.level);
        }
    }
    public override void OnCastleRaze()
    {
        base.OnCastleRaze();
        upgrades.Clear();
    }
    public ResearchedUpgrade[] GetBonusesByType(BuildingData.GrantBonus bonusType)
    {
        return upgrades.researchedUpgrades.Select(u => (u.upgrade is BuildingData building && building.bonusType == bonusType) ? u : null).ToArray();
    }
}
