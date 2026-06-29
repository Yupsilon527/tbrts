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

    }
   public void RaidCastle()
    {

    }
   public void RazeCastle()
    {

    }
   public void OccupyCastle()
    {

    }
}
