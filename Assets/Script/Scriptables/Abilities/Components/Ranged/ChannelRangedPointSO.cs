using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ranged Point (Channeled)", menuName = "Abilities/Ranged/Ranged Point (Channeled)")]
public class ChannelRangedPointSO : ChannelRangedDirectSO
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
