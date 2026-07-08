using TMPro;
using UnityEngine;

public class CastleRazeWindow : CastleWindow
{
    public DataItemArmy attacker;

    public bool ValidRaze()
    {
        return attacker!= null && assignedCastle!=null && attacker.CanInvadeCastle(assignedCastle);
    }
   public void SackCastle()
    {
        assignedCastle.RazeCastle(attacker, BuildingDefines.CastleRazeMode.sack);
        Close();
    }
   public void RaidCastle()
    {
        assignedCastle.RazeCastle(attacker, BuildingDefines.CastleRazeMode.raid);
        Close();
    }
   public void RazeCastle()
    {
        assignedCastle.RazeCastle(attacker, BuildingDefines.CastleRazeMode.raze);
        Close();
    }
   public void OccupyCastle()
    {
        assignedCastle.RazeCastle(attacker, BuildingDefines.CastleRazeMode.occupy);
        Close();
    }
}
