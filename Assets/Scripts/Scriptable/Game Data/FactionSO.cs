using UnityEngine;

public class FactionSO : ScriptableBase
{
    public Sprite sigilTexture, bannerTexture;
    public ResourceCost[] startingResources = new ResourceCost[0];
    public ResourceIncome[] startingIncome = new ResourceIncome[0];

    public UpgradeSO[] innateUpgrades;

    public UnitSO[] producedUnits;
    public BuildingSO[] buildings;

}
