using UnityEngine;

public class DataItemPlayer 
{
    public int ID, Team;
    public Color color;

    public Resource[] resources;

    public string Name;
    public int AiLevel = -1;
    public bool Defeated = false;
    public int TurnDefeat = -1;

    // public UnitGroup<Mob> units = new();
    // public UnitGroup<Building> buildings = new();

    public DataItemPlayer(int iD, Color color)
    {
        Team = iD;
        ID = iD;
        this.color = color;
    }
    public DataItemPlayer(int iD, int team, Color color)
    {
        Team = team;
        ID = iD;
        this.color = color;
    }
    public override string ToString()
    {
        return $"Player {ID} (team {Team})";
    }
    public bool isNeutral()
    {
        return ID == 0;
    }
    public bool isPlayer()
    {
        return ID == 1;
    }
    public PlayerDefines.Alignment GetAlignment(DataItemPlayer Other)
    {
        if (Other == null || Other.isNeutral())
        {
            if (ID == 0)
            {
                return PlayerDefines.Alignment.playerowned;
            }
            return PlayerDefines.Alignment.enemy;
        }
        else if (Other.Team != Team)
        {
            return PlayerDefines.Alignment.enemy;
        }
        else if (Other.ID == ID)
        {
            return PlayerDefines.Alignment.playerowned;
        }
        else
        {
            return PlayerDefines.Alignment.ally;
        }
    }
}
