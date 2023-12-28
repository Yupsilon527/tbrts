using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Point", menuName = "Abilities/Ranged/Ranged Point")]
public class RangedPointSO : RangedDirectSO
{
    public override AbilityDefines.CheckHitMobs CheckTargetMobs()
    {
        return AbilityDefines.check_circle_around_point;
    }
    public override AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.point;
    }
}
