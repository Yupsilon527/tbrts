using UnityEngine;

public class DataItemPlayer
{
    public int ID, Team;
    public Color color;


    public string Name;
    public int AiLevel = -1;

    public DataFaction faction = new();

    public DataItemCastle capital;
    public UnitGroup<DataItemBanner> units = new();
    public UnitGroup<DataItemCastle> buildings = new();

    public PlayerUpgrades upgrades;
    public PlayerEconomy econ;

    public DataItemPlayer(CustomPlayer custom, Color color) : this(custom.id, custom.Team, color)
    {
        Name = custom.Name;
        if (custom.FactionName == "" || custom.FactionName == "Random")
            if (ID > 0)
                AssignFaction(WorldManager.world.availableFactions[Mathf.FloorToInt(Random.value * WorldManager.world.availableFactions.Length)]);
            else
                AssignFaction(WorldManager.world.neutralFaction);
        else
            AssignFaction(WorldManager.main.LoadFaction(custom.FactionName));
    }
    public DataItemPlayer(int iD, Color color) : this(iD, iD, color)
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
    void AssignFaction(DataFaction f)
    {
        faction = f;
        if (f.startingResources != null)
            foreach (var r in f.startingResources)
            { econ.GiveResource(r.resource, r.value, true); }
        if (f.innateUpgrades != null)
            foreach (var up in f.innateUpgrades)
            { upgrades.upgrades.CatchUpUpgrade(up, 1); }
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


    public bool IsAiControlled()
    {
        return false;
    }
    public void OnTurnBegin()
    {
        foreach (var city in buildings)
        {
            for (int i = 0; i < city.income.baseIncome.Length; i++)
            {
                econ.GiveResource((EconomyDefines.EconomyResource)i, city.income.baseIncome[i]);
            }
        }
        econ.HandleIncome();
    }
}
