using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Play Caster Animation", menuName = "Abilities/Special Effects/Caster Animation")]
public class PlayCasterAnimationSO : SpecialEffectSO
{
    public string Animation = "attack";
    public float AnimationTime = 1;
    public override void MakeEffect(CastTable castData, float delay)
    {
        if (castData.caster.animations != null)
        {
            castData.caster.animations.PlayAnimation(Animation, AnimationTime);
        }
    }
}
