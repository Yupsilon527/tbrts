using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cast Attach On Targets With Modifier", menuName = "Abilities/Special Effects/Cast Attach On Targets With Modifier")]
public class AttachModifierEffectAESO : AttachEffectOnTargetAESO
{
    public string ModifierName;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        if (EffectPrefab == null)
            return;
        foreach (Mob target in targets)
        {
            if (target.modifiers.TryFindModifierByName(ModifierName, true, out PropertyModifier linkMod))
            {
                GameObject effect = SpecialEffectPool.main.AttachEffectFromPrefabOnEntity(target, EffectPrefab, AttachPoint,  GetDelay());
                if (effect == null)
                    return;

                linkMod.AttachEffect(effect);
            }
        }

    }
}
