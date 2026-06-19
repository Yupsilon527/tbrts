using System;
using UnityEngine;

[Serializable]
public class UnitData : ProductionData
{
    public UnitStatsTable unit;
    public AttackDefines.MobFlag[] unitFlags;
    public PropertyWeapon[] weapons;
    public PropertySpell[] spells;
    public AbilityData[] abilities;
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
    public override ResourceCost[] GetCostForPlayer(DataItemPlayer player, float mult = 1)
    {
        return base.GetCostForPlayer(player, mult);
    }

}
