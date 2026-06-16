using UnityEngine;

public class WorldSO : ScriptableBase
{
    public FactionSO[] factions;

    public bool NeutralTroopsRecuitment = false;
    public UnitSO[] neutralTroops;

    public bool NeutralBuilding = false;
    public BuildingSO[] neutralBuildings;
}
