using System;
using System.Linq;

[Serializable]
public class UnitData : ProductionData
{
    public UnitStatsTable unit;
    public AttackDefines.MobFlag[] unitFlags;
    public WeaponData[] weapons;
    public SpellData[] spells;
    public AbilityData[] abilities;
    public override AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        var avs = base.GetAvailableState(player, castle);
        if (avs == AvailableState.available && !player.econ.CanAffordResources(GetCostForPlayer(player)))
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
    public int GetCommandValue()
    {
        return 1;
    }
    public bool HasAbility(UnitDefines.ArmyAbilities ability)
    {
        return HasAbility(ability.ToString());
    }
    public bool HasAbility(string abilityID)
    {
        return abilities.Any(a => a.abilityID == abilityID);
    }
}
