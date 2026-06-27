using UnityEngine;

public class CityIncome : CityComponent
{
    public float[] baseIncome = new float[(int)EconomyDefines.IncomeResource.Total];
    public CityIncome(DataItemCastle city) : base(city)
    {
    }
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        Revision();
    }
    public override void Revision()
    {
        base.Revision();
        for (int i = 0; i < baseIncome.Length; i++)
        {
            baseIncome[i] = 0;
        }
            foreach (var unit in city.bonuses.upgrades.researchedUpgrades)
        {
            if (unit.upgrade is BuildingData building)
            {
                for (int i = 0; i < baseIncome.Length; i++)
                {
                    baseIncome[i] += building.GetResourceIncome((EconomyDefines.IncomeResource)i) ;
                }
            }
        }
    }
    public override void OnCastleRaze()
    {
        base.OnCastleRaze();
        Revision();
    }
}
