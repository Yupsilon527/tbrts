using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Target (Channeled)", menuName = "Abilities/Ranged/Ranged Target (Channeled)")]

public class ChannelRangedDirectSO : ChannelRangedAreaSO
{
    public int MinCastRange;
    public int MaxCastRange;

    public override AbilityDefines.CheckHitMobs CheckTargetMobs()
    {
        return AbilityDefines.check_circle_around_target;
    }
    public override AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.target;
    }
    public override float GetMinRange()
    {
        return MinCastRange;
    }
    public override float GetMaxRange()
    {
        return MaxCastRange;
    }
}
