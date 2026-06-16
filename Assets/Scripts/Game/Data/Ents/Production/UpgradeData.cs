using System;
using UnityEngine;

public class UpgradeData : ProductionData
{

    [Header("Flags")]
    public bool infinite = false;
    public string[] affectedFlags;

    [Header("Price Change Alteration")]
    public ResourceAlteration[] resourceChanges;

    [Header("Bonus Damage Upgrade For Units")]
  //  public BonusDamageTable[] bonusDamage = new BonusDamageTable[0];
    [Header("Stat Alterations For Units")]
  //  public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
   // public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];

    [Header("Grant Resources/Income")]
    public ResourceCost[] grantedResources = new ResourceCost[0];
    public ResourceIncome[] grantedIncome = new ResourceIncome[0];
    public override AvailableState GetAvailableState(DataItemPlayer player)
    {
        if (player.HasReachedLimitForUpgrade(this))
        {
            return AvailableState.unavailable;
        }
        else if (prerequisites.Length == 1 && !PrerequisiteMet(player, prerequisites[0]))
        {
            return AvailableState.unavailable;
        }
        else return base.GetAvailableState(player);
    }
    public bool AppliesToThing(ProductionData data)
    {
        foreach (var check in affectedFlags)
        {
            if (check[0] == '_')
            {
                if (check.Substring(1).ToLower() == data.InternalName.ToLower())
                    return true;
            }
            else
            {
                foreach (var flag in data.flags)
                {
                    if (check == flag) return true;
                }
            }
        }
        return false;
    }
    public virtual void SetPlayerLevel(DataItemPlayer player, int oldLevel, int newLevel)
    {
        int delta = newLevel - oldLevel;
        foreach (var res in grantedResources)
            player.GiveResource(res, delta);

        foreach (var income in grantedIncome)
            player.IncreaseIncome(income, newLevel - oldLevel);

        player.ReviseRealIncome();
    }
    /*public override void SetUnitLevel(UpgradeComponent data, bool onSpawn, int oldLevel, int newLevel)
    {
        int delta = newLevel - oldLevel;

        foreach (var prop in properties)
            data.UpdateProperty(prop.Property, prop.value * delta);

        if (oldLevel == 0 && newLevel > 0)
            foreach (var stat in states)
                data.UpdateState(stat.State, (int)stat.priority);
        else if (newLevel == 0)
            foreach (var stat in states)
                data.UpdateState(stat.State, 0);

        data.parent.classification.GrantBonusDamageFromTable(bonusDamage, oldLevel, newLevel);
    }*/
    public override void CompleteProduction(ProductionTable table)
    {
        table.playerOwner.CompleteUpgrade(this, 1);
    }
    public override ResourceCost[] GetCostForPlayer(DataItemPlayer player, float mult = 1)
    {
        var baseCost = base.GetCostForPlayer(player, mult);
        int numResearch = player.GetUpgradeLevel(this);
        if (numResearch > 0)
        {
            ResourceCost.Multiply(baseCost, Mathf.Pow(1.1f, numResearch));
        }
        return baseCost;
    }
}
[Serializable]
public class ResourceAlteration
{
    public enum ChangeBehavior
    {
        nothing,
        raw,
        percentage
    }
    public EconomyDefines.EconomyResource changedResource = EconomyDefines.EconomyResource.Metal;
    public ChangeBehavior resourceChangeBehavior = ChangeBehavior.nothing;
    public float changeValue = 0;
}