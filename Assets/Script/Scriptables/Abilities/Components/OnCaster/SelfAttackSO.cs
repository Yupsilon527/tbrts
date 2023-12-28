using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Self Attack", menuName = "Abilities/Misc/Self Attack")]
public class SelfAttackSO : ActiveAbilitySO
{
    /* public override AbilityData Translate()
     {
         return new AbilityData(name,
                 (int)AbilityDefines.Behavior.self,
                EnergyCost, CastTime, 0, 0, 0, 0, GetAbilityConditions(), AiBehavior, GetAbilityElement(),
                 AbilityDefines.check_self_default,
             new Dictionary<AbilityDefines.Event, AbilityDefines.AbilityFunction> {{AbilityDefines.Event.precast, (CastTable castData) =>  //precast
         {
             //animation part
             castData.caster.AnimCast(Animation, castData.point, AbilityDefines.anim_delay, AbilityDefines.anim_dur, AbilityDefines.anim_traveltime, 1);

             //special effect
             Vector3 worldpos = castData.caster.GetWorldPosition();
                     foreach (SpecialEffectSO specialEffect in AbilityEffects)
                     {
                 if (specialEffect == null)
                 {
                     Debug.LogWarning("[EffectSO] Failed to create effect for ability " + name);
                     continue;
                 }
                         specialEffect.MakeEffect(castData,AbilityDefines.anim_midpoint,AbilityDefines.anim_midpoint);
                     }
         } },
                 {AbilityDefines.Event.oncast,     (CastTable castData) =>
                 {
                         Effect.ActivateOnCaster(castData,AbilityDefines.anim_midpoint);
                         Effect.ActivateOnTarget(castData,AbilityDefines.anim_midpoint);


                 }
                 }                    });
     }
    */

    public override AbilityDefines.CheckHitMobs CheckTargetMobs()
    {
        return AbilityDefines.check_self_default;
    }
    public override AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.self;
    }

}
