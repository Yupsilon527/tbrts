using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Apply Attack", menuName = "Abilities/Effects/Apply Attack")]
public class ApplyAttacks : AbilityEffect
{
    public AttackData Attack;

    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        table.caster.damageable.ApplyAttack(table.ability, targets, Attack, table.ability.original.flags, animdelay); //TODO ability AoE define
    }
}
