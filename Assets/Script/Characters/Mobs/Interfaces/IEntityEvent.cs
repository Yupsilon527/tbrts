using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityEvent
{
    public void EventReaction(AbilityDefines.Event evtData, Mob target)
    {
        EventReaction(evtData, new Mob[] { target });
    }
    public void EventReaction(AbilityDefines.Event evt, Mob[] affectedCritters);
}