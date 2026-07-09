using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataFaction : BaseData
{
    public Sprite emblemTexture, bannerTexture, castleTexture;
    [Header("Startup")]
    public HashSet<ResourceCost> startingResources = new();
    public HashSet<ResourceIncome> startingIncome = new();

    [Header("Specific")]
    public UpgradeData[] innateUpgrades;
    public BuildingData[] availableBuildings;
    public DataFaction()
    {

    }
    public DataFaction( RaceSO race)
    {
        InternalName = race.InternalName;

        bannerTexture = race.display;

        availableBuildings = race.buildings.Select(p => p.building).ToArray();
    }
    public DataFaction(FactionSO faction, RaceSO race)
    {
        InternalName = faction.InternalName;

        bannerTexture = race.display;
        emblemTexture = faction.display;
        castleTexture = faction.castle;

        foreach (var r in faction.startingResources)
            startingResources.Add(r);
        foreach (var r in race.startingResources)
            startingResources.Add(r);
        foreach (var i in faction.startingIncome)
            startingIncome.Add(i);
        foreach (var i in race.startingIncome)
            startingIncome.Add(i);

        innateUpgrades = faction.innateUpgrades.Select(p => p.upgrade).Union(race.innateUpgrades.Select(p => p.upgrade)).ToArray();
        availableBuildings = faction.buildings.Select(p => p.building).Union(race.buildings.Select(p => p.building)).ToArray();
    }

    public List<UnitData> GetRecruitableArmies(bool neutral)
    {
        List<UnitData> ProductionArmies = new List<UnitData>();
        if (neutral || WorldManager.world.NeutralArmiesAreDefault)
        {
            foreach (var building in WorldManager.world.neutralFaction.availableBuildings)
            {
                foreach (var unitID in building.production)
                {
                    ProductionArmies.Add(WorldManager.main.LoadUnit(unitID));
                }
            }
        }
        foreach (var building in availableBuildings)
        {
            foreach (var Panty in building.production)
            {
                foreach (var unitID in building.production)
                {
                    ProductionArmies.Add(WorldManager.main.LoadUnit(unitID));
                }
            }
        }
        return ProductionArmies;
    }

    public List<BuildingData> GetAvailableUpgrades(bool neutral)
    {
        List<BuildingData> ProductionArmies = new List<BuildingData>();
        if (neutral || WorldManager.world.NeutralBuildingsAreDefault)
        {
            foreach (BuildingData Panty in WorldManager.world.neutralFaction.availableBuildings)
            {
                ProductionArmies.Add(Panty);
            }
        }
        foreach (BuildingData Panty in availableBuildings)
        {

            if (!ProductionArmies.Contains(Panty))
            {
                ProductionArmies.Add(Panty);
            }
        }

        return ProductionArmies;
    }

    public List<UnitData> FindArmiesWithAbility(string Ability, bool neutrals)
    {
        List<UnitData> AvailableArmies = new List<UnitData>();
        foreach (UnitData Panty in GetRecruitableArmies(neutrals))
        {
            if (Panty.HasAbility(Ability))
            {
                AvailableArmies.Add(Panty);
            }
        }
        return AvailableArmies;
    }
}
