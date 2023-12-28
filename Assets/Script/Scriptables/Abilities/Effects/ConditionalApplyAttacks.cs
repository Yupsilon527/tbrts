using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conditional Apply Attack", menuName = "Abilities/Effects/Conditional Apply Attack")]
public class ConditionalApplyAttacks : ApplyConditionalEffect
{
    public AttackData Attack;

    public override void ActivateOnTarget(CastTable table, Mob target, float animdelay)
    {
        table.caster.damageable.ApplyAttack(table.ability, target, Attack, table.ability.original.flags, animdelay); //TODO ability AoE define
    }
}
