
using UnityEngine;

public class TechtreeSo : ScriptableBase
{
    public Sprite display;
    public ResourceCost[] startingResources = new ResourceCost[0];
    public ResourceIncome[] startingIncome = new ResourceIncome[0];

    public UpgradeSO[] innateUpgrades;

    public BuildingSO[] buildings;
}
