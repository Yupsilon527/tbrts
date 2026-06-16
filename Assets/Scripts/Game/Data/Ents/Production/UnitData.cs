using UnityEngine;

public class UnitData : ProductionData
{
   // public StatsTableSO unit;

    public override AvailableState GetAvailableState(DataItemPlayer player)
    {
        var avs = base.GetAvailableState(player);
        if (avs == AvailableState.available && !player.CanAffordResources(GetCostForPlayer(player)))
        {
            return AvailableState.greyedout;
        }
        return avs;
    }
    public override void CompleteProduction(ProductionTable table)
    {
       // PlayerController.main.troopMan.SpawnBannerAtPoint(this, table.point, table.playerOwner);
    }
}
