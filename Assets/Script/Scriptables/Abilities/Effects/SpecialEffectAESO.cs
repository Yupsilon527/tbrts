using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cast Attach On Point", menuName = "Abilities/Special Effects/Cast Attach On Point")]
public class SpecialEffectAESO : AbilityEffect
{
    public float scale = 1;

    public bool ScaleWithCaster = false;
    public bool ScaleWithAoe = false;
    public GameObject EffectPrefab;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        if (EffectPrefab == null)
            return;
        float realScale = scale;
        if (ScaleWithAoe) realScale *= table.ability.GetAreaRange(false);
        if (table.ability.GetCastBehavior() == AbilityDefines.Behavior.point && Targeting == AbilityDefines.TargetType.ability_target)
        {
            SpecialEffectPool.main.EffectFromPrefab(EffectPrefab, table.point, GetDelay(), realScale);
        }
        else
        {
            foreach (Mob target in targets)
            {
                SpecialEffectPool.main.EffectFromPrefab(EffectPrefab, target.transform.position, GetDelay(), realScale);
            }
        }
    }
}
