using System;
using System.Linq;
using UnityEngine;
[Serializable]
public class BuildingData : TechData
{
    [Header("Production")]
    public UnitData[] production;
    [Header("Income")]
    public ResourceIncome[] income;
    [Header("Aura/Bonuses")]    //TODO
    public UpgradeData[] passiveBonuses;
    public float GetResourceIncome(EconomyDefines.IncomeResource resource)
    {
        return income.Sum(i => i.resource == resource ? i.value : 0);
    }
    public override void CompleteProduction(ProductionTable table)
    {
        table.producer.bonuses.BuildBuilding(this,1,true);
    }
}
