using UnityEngine;

public class WorldData : BaseData
{
    public DataFaction[] factions;

    public bool globalNeutralUnits;
    public UnitData[] neutralUnits;

    public bool globalNeutralBuildings;
    public BuildingData[] neutralBuildings;

    public MapData[] availableMaps;
    public WorldData(WorldSO scriptable)
    {

    }
}
