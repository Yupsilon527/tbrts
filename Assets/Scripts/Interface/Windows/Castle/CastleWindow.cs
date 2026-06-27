using UnityEngine;

public class CastleWindow : PlayerWindow
{
    public DataItemCastle assignedCastle;
    public virtual void FromCastle(DataItemCastle castle)
    {
        AssignPlayer(castle.GetPlayerOwner());
    }
}
