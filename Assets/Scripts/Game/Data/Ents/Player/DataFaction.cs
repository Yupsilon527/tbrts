using System.Collections.Generic;
using UnityEngine;

public class DataFaction : BaseData
{
    [Header("Startup")]
    public HashSet<ResourceCost> startingResources = new();
    public HashSet<ResourceIncome> startingIncome = new();

}
