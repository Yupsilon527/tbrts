
public class CustomPlayer
{

    public string Name;
    public string FactionName = "";
    public int id, Team, AiLevel;

    public ResourceCost[] startingResources;

    public bool Used = true;
    public bool Enabled = true;
    public bool Ready = true;
    public bool Locked = false;
    public bool LockedRace = false;
    public bool LockedTeam = false;
}
