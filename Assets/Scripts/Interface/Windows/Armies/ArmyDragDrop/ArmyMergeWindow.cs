using UnityEngine;

public class ArmyMergeWindow : DragDropWindow
{
    public DataItemArmy unitA, unitB;
    public void MergeUnits(DataItemArmy a, DataItemArmy b)
    {
        unitA = a;
        unitB = b;

        dragdrop. Clear();
        dragdrop.InitSlots(a,b);
    }
}
