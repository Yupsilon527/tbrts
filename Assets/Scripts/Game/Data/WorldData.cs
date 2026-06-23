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
        availableFactions = scriptable.factions.Select(faction => new DataFaction(faction)).ToArray();
        neutralFaction = new DataFaction(scriptable.neutrals);

        NeutralArmiesAreDefault = scriptable.NeutralArmiesAreDefault;
        NeutralBuildingsAreDefault = scriptable.NeutralBuildingsAreDefault;

        availableMaps = WorldManager.main.maps.Where(m => m.assignedWorld == scriptable.InternalName).ToArray();
    }
}
