using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataFaction : BaseData
{
    public Sprite sigilTexture, bannerTexture;
    [Header("Startup")]
    public HashSet<ResourceCost> startingResources = new();
    public HashSet<ResourceIncome> startingIncome = new();

    public UpgradeData[] innateUpgrades;

    [Header("Production")]
    public UnitData[] producedUnits;
    public BuildingData[] availableBuildings;
    public DataFaction()
    {

    }
    public DataFaction(FactionSO faction)
    {
        sigilTexture = faction.sigilTexture;
        bannerTexture = faction.bannerTexture;

        foreach (var r in faction.startingResources)
            startingResources.Add(r);
        foreach (var i in faction.startingIncome)
            startingIncome.Add(i);

        producedUnits = faction.producedUnits.Select(p => p.unit).ToArray();
        availableBuildings = faction.buildings.Select(p => p.building).ToArray();
    }

    public List<UnitData> GetRecruitableArmies()
    {
        List<UnitData> ProductionArmies = new List<UnitData>();
        if (WorldManager.world.globalNeutralUnits)
        {
            foreach (UnitData Panty in WorldManager.world.neutralUnits)
            {
                ProductionArmies.Add(Panty);
            }
        }
        foreach (UnitData Panty in producedUnits)
        {

            if (!ProductionArmies.Contains(Panty))
            {
                ProductionArmies.Add(Panty);
            }

        }
        return ProductionArmies;
    }

    public List<BuildingData> GetAvailableUpgrades()
    {
        List<BuildingData> ProductionArmies = new List<BuildingData>();
        if (WorldManager.world.globalNeutralBuildings)
        {
            foreach (BuildingData Panty in WorldManager.world.neutralBuildings)
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

    public List<UnitData> FindArmiesWithAbility(string Ability)
    {
        List<UnitData> AvailableArmies = new List<UnitData>();
        foreach (UnitData Panty in GetRecruitableArmies())
        {
            if (Panty.HasAbility(Ability) )
            {
                AvailableArmies.Add(Panty);
            }
        }
        return AvailableArmies;
    }
}
