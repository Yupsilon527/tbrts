using UnityEngine;

public class ArmyMergeWindow : PlayerWindow
{
    public DataItemArmy unitA, unitB;
    public AbilityDragDropInterface dragdrop;
    public void MergeUnits(DataItemArmy a, DataItemArmy b)
    {
        unitA = a;
        unitB = b;

        dragdrop.InitSlots(a,b);
    }
    protected override void OnClosed()
    {
        base.OnClosed();
        dragdrop.ApplyChanges();
        dragdrop.Clear();
    }
}
