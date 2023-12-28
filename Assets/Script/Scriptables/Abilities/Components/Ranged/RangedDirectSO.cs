using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Target", menuName = "Abilities/Ranged/Ranged Target")]
public class RangedDirectSO : RangedAreaSO
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
