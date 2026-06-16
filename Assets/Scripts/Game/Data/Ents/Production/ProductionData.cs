using System;
using UnityEngine;

[Serializable]
public class ProductionData 
{
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
    public virtual bool IsValidPosition(Vector2Int origin)
    {
        return true;
    }
    public virtual void CompleteProduction(ProductionTable table)
    {

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