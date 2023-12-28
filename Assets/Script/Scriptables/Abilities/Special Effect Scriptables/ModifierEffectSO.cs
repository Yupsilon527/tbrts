using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Modifier", menuName = "Abilities/Special Effects/On Modifier")]
public class ModifierEffectSO : AttachEffectSO
{
    public string ModifierName;

    public override void MakeEffect(CastTable castData, float delay)
    {
        foreach (Mob target in castData.GetHitEntities(false))
        {
            if (target.modifiers.TryFindModifierByName(ModifierName, true, out PropertyModifier linkMod))
            {
                GameObject effect = SpecialEffectPool.main.AttachEffectFromPrefabOnEntity(target, EffectPrefab, AttachPoint, delay + GetDelay());
                if (effect == null)
                    return;

                linkMod.AttachEffect(effect);
            }
        }
    }
}
