using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Melee", menuName = "Abilities/Special Effects/On Melee")]
public class MeleeEffectSO : SpecialEffectSO
{
    public float DistanceFromCenter = 1f;
    public override void MakeEffect(CastTable castData, float delay)
    {
        GameObject effect = SpecialEffectPool.main.EffectFromPrefab(EffectPrefab, castData.origin, delay + GetDelay(), Mathf.Max(1, castData.ability.GetAreaRange(false)));
        if (effect == null)
            return ;

        Vector3 delta = castData.point - castData.origin;

        effect.transform.localPosition = effect.transform.localPosition  + delta.normalized * DistanceFromCenter;
        effect.transform.right = delta;
    }
}
