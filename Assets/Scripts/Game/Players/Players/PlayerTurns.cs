using UnityEngine;

public class PlayerTurns : PlayerComponent
{
    public int playerTurn = -1;

    public PlayerTurns(DataItemPlayer player) : base(player)
    {
    }
}
