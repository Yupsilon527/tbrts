using System;
using System.Linq;
using UnityEngine;
[Serializable]
public class BuildingData : TechData
{
    [Header("Production")]
    public string[] production;
    [Header("Income")]
    public ResourceIncome[] income;
    [Header("Aura/Bonuses")]    //TODO
    public UpgradeData[] passiveBonuses;
    public override AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        if (castle.bonuses.HasBuilding(this))
            return AvailableState.hidden;
            return base.GetAvailableState(player, castle);
    }
    public float GetResourceIncome(EconomyDefines.IncomeResource resource)
    {
        return income.Sum(i => i.resource == resource ? i.value : 0);
    }
    public override bool CompleteProduction(ProductionTable table)
    {
        table.castle.bonuses.BuildBuilding(this,1,true);
        return true;
    }
}
