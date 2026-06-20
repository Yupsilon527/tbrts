using UnityEngine;

public class BuildingData : TechData
{
    public override void CompleteProduction(ProductionTable table)
    {
        table.producer.bonuses.BuildBuilding(this,1);
    }
}
