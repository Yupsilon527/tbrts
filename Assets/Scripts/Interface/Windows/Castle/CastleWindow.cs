using TMPro;
using UnityEngine;

public class CastleWindow : PlayerWindow
{
    public DataItemCastle assignedCastle;

    public TextMeshProUGUI castleName, castleDesc;
    public virtual void FromCastle(DataItemCastle castle)
    {
        AssignPlayer(castle.GetPlayerOwner());
    }
}
