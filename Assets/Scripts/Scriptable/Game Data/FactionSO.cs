using UnityEngine;

[CreateAssetMenu(fileName = "Race", menuName = "Data/World/Race")]
public class FactionSO : ScriptableBase
{
    public CharacterSO character;
    public ResourceCost[] startingResources = new ResourceCost[0];
    public ResourceIncome[] startingIncome = new ResourceIncome[0];

    public UpgradeSO[] innateUpgrades;

    public UnitSO[] producedUnits;
    public BuildingSO[] buildings;

}
