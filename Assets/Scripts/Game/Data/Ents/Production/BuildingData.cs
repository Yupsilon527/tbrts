using System.Linq;
using UnityEngine;

public class BuildingData : TechData
{
    public UnitData[] production;
    public ResourceIncome[] income;
    public UpgradeData[] passiveBonuses;
    public float GetResourceIncome(EconomyDefines.IncomeResource resource)
    {
        return income.Sum(i => i.resource == resource ? i.value : 0);
    }
    public override void CompleteProduction(ProductionTable table)
    {
        table.producer.bonuses.BuildBuilding(this,1);
    }
}
