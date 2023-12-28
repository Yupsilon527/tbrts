using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attach On Targets", menuName = "Abilities/Special Effects/Attach On Targets")]
public class TargetAttachEffectSO : SpecialEffectSO
{
    public string AttachPoint = "origin";

    public override void MakeEffect(CastTable castData, float delay)
    {
        if (EffectPrefab == null)
            return ;
        foreach (Mob hit in castData.GetHitEntities(false))
        {
            SpecialEffectPool.main.EffectFromPrefabOnEntity(hit, EffectPrefab,  AttachPoint, delay + GetDelay());
        }
    }
}
