using UnityEngine;

public class ArmyInfoWindow : DragDropWindow
{
    public DataItemArmy assignedArmy;
    public void ForSingleArmy(DataItemArmy a)
    {
        assignedArmy = a;
        AssignPlayer(assignedArmy.GetPlayerOwner());
        dragdrop.InitSlots(a);
    }
}
