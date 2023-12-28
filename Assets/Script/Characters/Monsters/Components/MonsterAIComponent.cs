using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAIComponent : CombatantComponent
{
    public float newTargetCheckTime = 2;
    float lastAttackTime = 0;
    protected override void ApproachTarget()
    {
        if (!CanCastAbilityOnTarget())
        {
            FindNewTarget();
            lastAttackTime = Time.time + 1;
            base.ApproachTarget();
        }
    }
    void FindNewTarget()
    {
        if (Time.time - lastAttackTime  > newTargetCheckTime)
        {
            SearchNextTarget(GetActiveAbility().GetMaxRange(false));
        }
    }
    protected override void OnTargetOutOfRange()
    {
        FindNewTarget();
    }
    protected override void AttackTarget()
    {
        if (!CanCastAbilityOnTarget())
        {
            base.AttackTarget();
            lastAttackTime = Time.time;
        }

    }
    bool CanCastAbilityOnTarget()
    {
        foreach (PropertyAbility ability in parent.abilities.GetAvailableSpells(true))
        {
            if ( ability.original.GetAbilityBehavior() == AbilityDefines.Behavior.self && ability.GetAreaRange(false) > 0 && (parent.transform.position - attackTarget.transform.position).sqrMagnitude > ability.GetAreaRange(false) * ability.GetAreaRange(false)) { 
                continue;
            }
            CastAbilityOnTarget(ability, attackTarget);
            return true;
        }
        return false;
    }
}
