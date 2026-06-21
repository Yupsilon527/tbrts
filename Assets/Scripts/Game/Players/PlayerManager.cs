using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameComponent
{
    public int playerTurn = 0;
    public DataItemPlayer[] players;

    public DataItemPlayer GetCurrentPlayer()
    {
        return players[playerTurn];
    }

    public DataItemPlayer MakeNeutrals(CustomMap map)
    {
        DataItemPlayer neutrals = new DataItemPlayer(0, Color.gray)
        {
            Name = map.NeutralName
        };

        List<UnitData> asd = new();
        asd.AddRange(WorldManager.world.neutralUnits);
        foreach (string unitID in map.neutralUnits)
        {
            asd.Add(WorldManager.main.LoadUnit(unitID));
        }

        neutrals.faction = new DataFaction()
        {
            InternalName = map.NeutralName,

            producedUnits = asd.ToArray(),
            availableBuildings = WorldManager.world.neutralBuildings,
        };
        return  neutrals;
    }



    public void MakeThePlayersFromEditorData(CustomMap game)
    {

        var tempPlayers = new List<DataItemPlayer>();
        tempPlayers.Add(MakeNeutrals(game));

        foreach (var cplayer in game.players)
        {
            var player = new DataItemPlayer(cplayer);
            player.econ.GiveResources(game.startingPlayerResources);
            player.econ.GiveResources(cplayer.startingResources);
            tempPlayers.Add(player);
        }

        players = tempPlayers.ToArray();
    }

}
