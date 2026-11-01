using System;
using System.Linq;
using UnityEngine;
[Serializable]
public class BuildingData : TechData
{
    public enum GrantBonus
    {
        upgrade,
        garrison,
        aura
    }
    public enum IncomeBonus
    {
        raw,
        citySize,
    }
    public enum BuildingType
    {
        generic,
        unique,
    }
    [Header("Granted Bonus")]
    public GrantBonus bonusType;
    public IncomeBonus incomeType;
    public BuildingType uniqueType;
    [Header("Production")]
    public string[] production;
    public override AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        if (castle.bonuses.HasBuilding(this))
            return AvailableState.hidden;
            return base.GetAvailableState(player, castle);
    }
    public float GetResourceIncome(DataItemCastle castle, EconomyDefines.IncomeResource resource)
    {
        float total = grantedIncome.Sum(i => i.resource == resource ? i.value : 0);
        switch (incomeType)
        {
            default:
                return total;
            case IncomeBonus.citySize:
                return total * castle.GetSize();
        }
    }
    public override bool CompleteProduction(ProductionTable table)
    {
        table.castle.bonuses.BuildBuilding(this,1,true);
        return true;
    }
}
