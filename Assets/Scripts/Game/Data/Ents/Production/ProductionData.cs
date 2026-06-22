using UnityEngine;

public abstract class ProductionData 
{
    public override string ToString()
    {
        return InternalName + " UnitData";
    }
    public string InternalName;
    public enum AvailableState
    {
        available,
        greyedout,
        greyedoutNocost,
        unavailable,
        hidden,
    }
    [Header("General")]
    public Sprite icon;
    public string[] flags;
    [Header("Costs")]
    public ResourceCost[] costs = new ResourceCost[0];
    public string[] prerequisites = new string[0];
    public float GetBaseCost(EconomyDefines.EconomyResource resource)
    {
        foreach (var cost in costs)
        {
            if (cost.resource == resource)
                return cost.value;
        }
        return 0;
    }
    public virtual AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        foreach (string prerequisite in prerequisites)
        {
            if (!PrerequisiteMet(player, castle, prerequisite))
            {
                return AvailableState.greyedoutNocost;
            }
        }
        return AvailableState.available;
    }

    public bool PrerequisiteMet(DataItemPlayer player, DataItemCastle castle, string prerequisite)
    {
        //if (prerequisite.Substring(0, 2) == "b_")
        {
          //  return player.HasBuilding(prerequisite);
        }
        return player.upgrades.upgrades.UpgradeResearched(prerequisite);
    }
    public virtual float GetCostForPlayer(DataItemPlayer player, EconomyDefines.EconomyResource resource, float mult = 1)
    {
        foreach (var cost in GetCostForPlayer(player, mult))
        {
            if (cost.resource == resource)
                return cost.value;
        }
        return 0;
    }
    public bool IsFreeForPlayer(DataItemPlayer player)
    {
        var costs = GetCostForPlayer(player);
        return (costs[(int)EconomyDefines.EconomyResource.Gold].value == 0
            && costs[(int)EconomyDefines.EconomyResource.Metal].value == 0
            && costs[(int)EconomyDefines.EconomyResource.Mana].value == 0
            && costs[(int)EconomyDefines.EconomyResource.Labor].value == 0);
    }
    public virtual ResourceCost[] GetCostForPlayer(DataItemPlayer player, float mult = 1)
    {
        float[] additions = new float[(int)EconomyDefines.EconomyResource.Total];
        float[] multipliers = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        foreach (var upgrade in player.upgrades.upgrades.researchedUpgrades)
        {
            if (upgrade.level > 0 && upgrade.upgrade.AppliesToThing(this))
            {
                foreach (var change in upgrade.upgrade.resourceChanges)
                {
                    int iR = (int)change.changedResource;
                    switch (change.resourceChangeBehavior)
                    {
                        case ResourceAlteration.ChangeBehavior.raw:
                            additions[iR] += change.changeValue * upgrade.level;
                            break;
                        case ResourceAlteration.ChangeBehavior.percentage:
                            for (int i = 0; i < upgrade.level; i++)
                                multipliers[iR] *= Mathf.Pow(change.changeValue, upgrade.level);
                            break;
                    }
                }
            }
        }
        ResourceCost[] final = new ResourceCost[(int)EconomyDefines.EconomyResource.Total];
        for (int iR = 0; iR < final.Length; iR++)
        {
            EconomyDefines.EconomyResource res = (EconomyDefines.EconomyResource)iR;
            final[iR] = new ResourceCost(res, (GetBaseCost(res) + additions[iR]) * multipliers[iR] * mult);
        }
        return final;
    }
    public virtual bool Produce(ProductionTable table)
    {
        if (table.percent < 0) return true;
        var price = GetCostForPlayer(table.playerOwner);
        if (table.playerOwner.econ.CanAffordResources(price))
        {
            table.playerOwner.econ.SpendResources(price);
            return true;

        }
        return false;
    }
    public virtual void CompleteProduction(ProductionTable table)
    {

    }
    public virtual bool IsValidPosition(Vector2Int origin)
    {
        return true;
    }
    public virtual float GetRadius()
    {
        return 1f;
    }
    public virtual bool IsUnique()
    {
        return true;
    }
    public virtual void AdoptOther(ProductionData other, float resMult)
    {
        if (icon == null)
            icon = other.icon;
        if (flags.Length == 0)
        {
            flags = other.flags;
        }
        if (prerequisites.Length == 0 && other.prerequisites.Length != 0)
        {
            prerequisites = other.prerequisites;
        }
        if (costs.Length == 0)
        {
            costs = new ResourceCost[other.costs.Length];
            for (int c = 0; c < costs.Length; c++)
            {
                costs[c] = new ResourceCost(other.costs[c].resource, other.costs[c].value * resMult);
            }
        }
    }
}

public class ProductionTable
{
    public ProductionTable(DataItemPlayer playerOwner, Vector3 point, Vector2Int node, DataItemCastle producer = null, float percent = 0)
    {
        this.playerOwner = playerOwner;
        this.producer = producer;
        this.node = node;
        this.point = point;
        this.percent = percent;
    }
    public DataItemPlayer playerOwner; public DataItemCastle producer; public Vector2Int node; public Vector3 point; public float percent = 1; public float costPercent = 1;

}