using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Data/World")]
public class WorldSO : ScriptableBase
{
    public FactionSO neutrals;
    public FactionSO[] factions;

    public bool NeutralArmiesAreDefault = false;
    public bool NeutralBuildingsAreDefault = false;
}
