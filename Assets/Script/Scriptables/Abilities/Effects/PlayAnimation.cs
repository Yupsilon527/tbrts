using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Play Animation", menuName = "Abilities/Effects/Play Animation")]
public class PlayAnimation : AbilityEffect
{
    public string AnimationName = "";
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        foreach (var target in targets)
        target.animations.PlayAnimation(AnimationName);
    }

}
