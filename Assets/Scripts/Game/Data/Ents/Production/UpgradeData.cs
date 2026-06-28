using System;
using UnityEngine;

[Serializable]
public class UpgradeData : TechData
{
    public override AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        if (player.upgrades.upgrades.HasReachedLimitForUpgrade(this))
        {
            return AvailableState.unavailable;
        }
        else if (prerequisites.Length == 1 && !PrerequisiteMet(player, castle, prerequisites[0]))
        {
            return AvailableState.unavailable;
        }
        else return base.GetAvailableState(player, castle);
    }
    public virtual void SetPlayerLevel(DataItemPlayer player, int oldLevel, int newLevel)
    {
        int delta = newLevel - oldLevel;
        foreach (var res in grantedResources)
            player.econ.GiveResource(res, delta);

        foreach (var income in grantedIncome)
            player.econ.IncreaseIncome(income, newLevel - oldLevel);

        player.econ.ReviseRealIncome();
    }
    public override void CompleteProduction(ProductionTable table)
    {
        table.playerOwner.upgrades.upgrades.CompleteUpgrade(this, 1);
    }
    public override ResourceCost[] GetCostForPlayer(DataItemPlayer player, float mult = 1)
    {
        var baseCost = base.GetCostForPlayer(player, mult);
        int numResearch = player.upgrades.upgrades.GetUpgradeLevel(this);
        if (numResearch > 0)
        {
            ResourceCost.Multiply(baseCost, Mathf.Pow(1.1f, numResearch));
        }
        return baseCost;
    }
}