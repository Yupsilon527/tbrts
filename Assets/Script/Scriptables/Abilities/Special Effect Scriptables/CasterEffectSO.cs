using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Origin", menuName = "Abilities/Special Effects/On Origin")]
public class CasterEffectSO : SpecialEffectSO
{
    public override void MakeEffect(CastTable castData, float delay)
    {
         SpecialEffectPool.main.EffectFromPrefab(EffectPrefab, castData.origin, delay + GetDelay(), Mathf.Max(1, castData.ability.GetAreaRange(false)));
    }
}
