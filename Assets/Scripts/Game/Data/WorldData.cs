using System.Linq;
using UnityEngine;

public class WorldData : BaseData
{
    public DataFaction[] factions;

    public bool NeutralTroopsRecuitment;
    public UnitData[] neutralUnits;

    public bool NeutralBuilding;
    public BuildingData[] neutralBuildings;

    public CustomMap[] availableMaps;
    public WorldData(WorldSO scriptable)
    {
        InternalName = scriptable.InternalName;
        factions = scriptable.factions.Select(faction => new DataFaction(faction)).ToArray();

        NeutralTroopsRecuitment = scriptable.NeutralTroopsRecuitment;
        neutralUnits = scriptable.neutralUnits.Select(u => u.unit).ToArray();

        NeutralBuilding = scriptable.NeutralBuilding;
        neutralBuildings = scriptable.neutralBuildings.Select(u => u.building).ToArray();

        availableMaps = WorldManager.main.maps.Where(m => m.assignedWorld == scriptable.InternalName).ToArray();
    }
}
