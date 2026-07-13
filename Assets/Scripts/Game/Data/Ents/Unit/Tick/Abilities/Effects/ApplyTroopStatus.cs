using UnityEngine;

public class ApplyTroopStatus : ApplyEffects
{
    public ModifierDefines.TroopState effect;
    public int turnDuration;
    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        table.target.troop.status.ApplyPendingStatus(this);
    }
}
