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
        foreach (var player in map.players)
        {
            if (player.id == 0)
            {
                neutrals = new DataItemPlayer(player, Color.gray);
            }
        }
        neutrals.Team = -1;
        return  neutrals;
    }



    public void MakeThePlayersFromEditorData(CustomMap game)
    {

        var tempPlayers = new List<DataItemPlayer>();
        tempPlayers.Add(MakeNeutrals(game));

        foreach (var cplayer in game.players)
        {
            var player = new DataItemPlayer(cplayer, PlayerDefines.playerColors[cplayer.id]);
            player.econ.GiveResources(game.startingPlayerResources);
            player.econ.GiveResources(cplayer.startingResources);
            tempPlayers.Add(player);
        }

        players = tempPlayers.ToArray();
    }

}
