using System.Collections.Generic;
using System.Linq;

public class WorldData : BaseData
{
    public DataFaction neutralFaction;
    public DataFaction[] availableFactions;

    public bool NeutralArmiesAreDefault;
    public bool NeutralBuildingsAreDefault;

    public CustomMap[] availableMaps;
    public WorldData(WorldSO scriptable)
    {
        InternalName = scriptable.InternalName;

        HashSet<DataFaction> factions = new();
        foreach (var race in scriptable.races)
        { foreach (var faction in race.subFactions)
            {
                factions.Add(new DataFaction(faction, race));
            } 
        }
        availableFactions = factions.ToArray();
        neutralFaction = new DataFaction(scriptable.neutrals);

        NeutralArmiesAreDefault = scriptable.NeutralArmiesAreDefault;
        NeutralBuildingsAreDefault = scriptable.NeutralBuildingsAreDefault;

        availableMaps = WorldManager.main.maps.Where(m => m.assignedWorld == scriptable.InternalName).ToArray();
    }
}
