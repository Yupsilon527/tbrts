using System;
using UnityEngine;

public class EconomyDefines 
{
    public enum EconomyResource
    {
        Steel = 0,
        Gold = 1,
        Mana = 2,
        Total = 3,
    }
}

[Serializable]
public class ResourceCost
{
    public EconomyDefines.EconomyResource resource;
    public float value;
    public ResourceCost() { }

    public ResourceCost(EconomyDefines.EconomyResource resource, float value)
    {
        this.resource = resource;
        this.value = value;
    }
    public override string ToString()
    {
        return $"Cost {value} {resource}";
    }
    public void Multiply(float mult)
    {
        value *= mult;
    }
    public static void Multiply(ResourceCost[] costs, float mult)
    {
        foreach (var cost in costs)
            cost.Multiply(mult);
    }
    public static void Multiply(ResourceCost[] costs, EconomyDefines.EconomyResource resource, float mult)
    {
        foreach (var cost in costs)
            if (cost.resource == resource)
                cost.Multiply(mult);
    }
}
[Serializable]
public class ResourceIncome
{
    public EconomyDefines.IncomeResource resource;
    public float value;
    public ResourceIncome() { }

    public ResourceIncome(EconomyDefines.IncomeResource resource, float value)
    {
        this.resource = resource;
        this.value = value;
    }
    public override string ToString()
    {
        return $"Income {value} {resource}";
    }
}
