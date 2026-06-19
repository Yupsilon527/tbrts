using UnityEngine;

public class WorldData : BaseData
{
    public DataFaction[] factions;

    public bool globalNeutralUnits;
    public UnitData[] neutralUnits;

    public bool globalNeutralBuildings;
    public UnitData[] neutralBuildings;

    public MapData[] availableMaps;
}
