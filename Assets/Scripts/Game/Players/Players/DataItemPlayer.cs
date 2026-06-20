using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemPlayer 
{
    public int ID, Team;
    public Color color;


    public string Name;
    public int AiLevel = -1;

    public DataFaction faction = new();

    public UnitGroup<DataItemArmy> units = new();
    public UnitGroup<DataItemCastle> buildings = new();

    public DataItemPlayer(int iD, Color color) : this(iD,iD,color)
    {
    }
    public DataItemPlayer(int iD, int team, Color color)
    {
        Team = team;
        ID = iD;
        this.color = color;
        upgrades = new(this);
        econ = new(this);
    }
    public PlayerUpgrades upgrades;
    public PlayerEconomy econ;
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
   
    
    public bool IsAiControlled()
    {
        return false;
    }
}
