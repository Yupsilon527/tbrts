using System.Linq;
using UnityEngine;

public class DataItemArmy : DataItemObject
{
    public int Movement = 0;
    public bool movedThisTurn = false;


    public DataItemUnit[] Formation = new DataItemUnit[6];
    public DataItemUnit transport;

    public int CountLivingTroops()
    {
        return Formation.Sum(u => u != null && u.damageable.IsAlive() ? 1 : 0);
    }
    public DataItemUnit GetTroopInPosition(int x, int y)
    {
        return Formation[x + y * 3];
    }
    public int GetPowerValue(bool threat)
    {
        return 0;
    }
}
