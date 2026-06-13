using UnityEngine;

public class PlayerManager : Initializable
{
    public static PlayerManager main;
    public int activePlayer;
    public DataItemPlayer[] players;

    public DataItemPlayer GetActivePlayer()
    {
        return players[activePlayer];
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
    }
}
