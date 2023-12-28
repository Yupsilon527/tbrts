using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cast Attach On Targets", menuName = "Abilities/Special Effects/Cast Attach On Targets")]
public class AttachEffectOnTargetAESO : AbilityEffect
{
    public GameObject EffectPrefab;
    public string AttachPoint = "origin";
    public float Scale = 1;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        if (EffectPrefab == null)
            return;
        foreach (Mob target in targets)
        {
            SpecialEffectPool.main.EffectFromPrefabOnEntity(target, EffectPrefab, AttachPoint, GetDelay(), scale : Scale );
        }

    }

}
