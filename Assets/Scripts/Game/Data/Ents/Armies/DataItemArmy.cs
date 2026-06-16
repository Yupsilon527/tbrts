using UnityEngine;

public class DataItemArmy : DataItemObject
{
    public int Movement = 0;
    public bool movedThisTurn = false;


    public DataItemUnit[,] Formation = new DataItemUnit[3, 2];
    public DataItemUnit transport;

    public int GetPowerValue(bool threat)
    {
        return 0;
    }
}
