using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attach On Caster", menuName = "Abilities/Special Effects/Attach On Caster")]
public class AttachEffectSO : TargetAttachEffectSO
{
    public override void MakeEffect(CastTable castData, float delay)
    {
        if (EffectPrefab == null)
            return ;

        SpecialEffectPool.main.EffectFromPrefabOnEntity(castData.caster, EffectPrefab,  AttachPoint, delay + GetDelay());
    }
}
