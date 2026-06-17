using UnityEngine;

public class EventData : ApplyEffects
{
    public AbilityDefines.Event abilityEvent;
    public override void ActivateOnUnit(CastTable table, DataItemUnit target, float strength = 1)
    {
        target.FireEventOnSelf(abilityEvent);
    }
}

