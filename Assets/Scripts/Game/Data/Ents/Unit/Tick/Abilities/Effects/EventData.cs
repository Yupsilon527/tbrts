using UnityEngine;

public class EventData : ApplyEffects
{
    public AbilityDefines.Event abilityEvent;

    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        table.target.FireEventOnSelf(abilityEvent);
    }
}

