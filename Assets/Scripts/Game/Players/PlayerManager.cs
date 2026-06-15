using UnityEngine;

public class PlayerManager : GameComponent
{
    public int playerTurn = 0;
    public DataItemPlayer[] players;

    public DataItemPlayer GetCurrentPlayer()
    {
        return players[playerTurn];
    }

}
