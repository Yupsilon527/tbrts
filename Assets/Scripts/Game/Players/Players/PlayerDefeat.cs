using UnityEngine;

public class PlayerDefeat : PlayerComponent

{
    public bool Defeated = false;
    public int TurnDefeat = -1;

    public PlayerDefeat(DataItemPlayer player) : base(player)
    {
    }
}
