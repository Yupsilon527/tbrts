using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelAbilitySO : ActiveAbilitySO
{
    public AbilityEffect[] SucceedEffects = new AbilityEffect[0];
   
    public float ChannelInterval = 1;
    public override float GetChannelInterval()
    {
        return ChannelInterval;
    }
}
