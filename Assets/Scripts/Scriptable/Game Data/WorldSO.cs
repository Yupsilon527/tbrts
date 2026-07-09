using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Data/World")]
public class WorldSO : ScriptableBase
{
    public RaceSO neutrals;
    public RaceSO[] factions;
    public RaceSO[] races;

    public bool NeutralArmiesAreDefault = false;
    public bool NeutralBuildingsAreDefault = false;
}
