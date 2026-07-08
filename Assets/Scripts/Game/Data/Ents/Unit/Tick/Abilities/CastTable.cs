using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventTable
{
    public int tick = 0;
    public DataItemUnit caster;
    public DataItemUnit target;

    public EventTable(CastTable table, DataItemUnit target): this (table.tick, table.attacker,target)
    {
    }

    public EventTable(int tick, DataItemUnit caster, DataItemUnit target)
    {
        this.tick = tick;
        this.caster = caster;
        this.target = target;
    }
    public virtual DataItemUnit GetTarget()
    {
        if (caster.GetAlignment(target) == PlayerDefines.Alignment.enemy && target.damageable.guardian != null)
            return target.damageable.guardian;
        return target;
    }
}
public class ReactionTable : EventTable
{
    public PropertyAttribute modifier;
    public ReactionTable(CastTable table, DataItemUnit target, PropertyAttribute modifier) : base(table, target)
    {
        this.modifier = modifier;
    }

    public ReactionTable(int tick, DataItemUnit caster, DataItemUnit target, PropertyAttribute modifier) : base(tick, caster, target)
    {
        this.modifier = modifier;
    }
    public override DataItemUnit GetTarget()
    {
        return target;
    }
}
public class CastTable
{
    public bool attackingSide = false;
    public float proc = 0;
    public int tick = 0;
    public DataItemUnit attacker;
    public Vector2Int targetPoint;
    public PropertyAbility ability;

    public DataItemUnit[] maintarget;
    public DataItemUnit[] sidetarget;
    public void Redirect(DataItemUnit target)
    {
        maintarget = new DataItemUnit[] { target };
        sidetarget = new DataItemUnit[0] { };
    }

    public CastTable(DataItemUnit caster, Vector2Int targetPoint, PropertyAbility ability)
    {
        this.attacker = caster;
        this.targetPoint = targetPoint;
        this.ability = ability;
        ComputeTargets();
    }
    public CastTable(DataItemUnit caster, DataItemUnit target)
    {
        this.attacker = caster;
        this.targetPoint = target.troopPosition;

        maintarget = new DataItemUnit[] { target };
        sidetarget = new DataItemUnit[0];
        Precast();
    }


    public void ComputeTargets()
    {
        maintarget = ability.GetMainTargets(this);
        sidetarget = ability.GetAreaTargets(this);
        Precast();
    }
    public virtual void ComputeDamageTable(DataItemUnit target)
    {
        target.damageable.lastDamage = new DamageTable(attacker, target, AttackDefines.HitType.normal);
    }
    public virtual void Precast()
    {
        ComputeDamageTable(attacker);
        foreach (var mt in maintarget)
        {
            ComputeDamageTable(mt);
        }
        foreach (var st in sidetarget)
        {
            ComputeDamageTable(st);
        }
    }
}
public class AttackTable : CastTable
{
    public AttackTable(CombatDefines.AttackPhase phase, int tick, DataItemUnit caster, Vector2Int targetPoint, PropertyAbility ability) : base(caster, targetPoint, ability)
    {
        this.phase = phase;
        this.tick = tick;
        attackingSide = Combat.main.IsAttackingSide(caster);
        ComputeTargets();
    }
    public CombatDefines.AttackPhase phase;
    public AttackDefines.HitType DetermineHitType(DataItemUnit target)
    {
        AttackDefines.HitType hitType = AttackDefines.HitType.normal;

        if (ability is PropertyWeapon attack)
        {
            float ranval = Random.value;
            if (!attacker.GetState(ModifierDefines.State.cannot_miss) && !attack.original.HasFlag(CombatDefines.AttackFlag.cannotMiss))
            {
                float evasion = target.dodgeCounter / 2f * target.stats.realStats.DodgeChance * target.stats.realStats.GetLuckCoefficient();

                if (ranval < Mathf.Min(1 - AttackDefines.minAccuracy, evasion))
                {
                    target.dodgeCounter = 1;
                    attacker.FireEventOnTarget(AbilityDefines.Event.OnDodgeEnemy, target);
                    return AttackDefines.HitType.miss;
                }
                else
                {
                    target.dodgeCounter++;
                }
            }

            if (attack.original.HasFlag(CombatDefines.AttackFlag.indirectAttack))
            {
                target.FireEventOnTarget(AbilityDefines.Event.IndirectHitByEnemy, attacker);
            }
            else
            {
                if (attacker.GetState(ModifierDefines.State.true_block))
                {
                    hitType = AttackDefines.HitType.blocked;
                }
                else
                {
                    float accuracy = attacker.stats.realStats.Offense / Mathf.Max(target.stats.realStats.Defense);
                    accuracy = accuracy * .6f + Mathf.Min(.4f, attacker.hitCounter / 2 * .5f); //TODO DEFINE

                    float critChance = 10 * accuracy + attacker.stats.realStats.CritChance * (1 + accuracy) / 2f * attacker.critCounter * .5f;
                    float hitChance = 50 * accuracy;
                    float blockChance = 25 / accuracy + attacker.stats.realStats.BlockChance * (1 + accuracy) / 2f;
                    float parryChance = 15 / accuracy + attacker.stats.realStats.BlockChance * (1 + accuracy) / 2f;

                    ranval = Random.value * (critChance + hitChance + blockChance + parryChance);
                    if (ranval < blockChance)
                    {
                        hitType = ranval < parryChance ? AttackDefines.HitType.blockCrit : AttackDefines.HitType.blocked;
                        attacker.hitCounter++;
                    }
                    else
                    {
                        hitType = (ranval > parryChance + blockChance + hitChance) ? AttackDefines.HitType.criticalHit : AttackDefines.HitType.normal;
                        attacker.hitCounter = 1;
                        if (hitType == AttackDefines.HitType.criticalHit)
                            attacker.critCounter = 1;
                        else
                            attacker.critCounter++;

                    }
                }
                switch (hitType)
                {
                    case AttackDefines.HitType.blockCrit:
                    case AttackDefines.HitType.blocked:
                        target.FireEventOnTarget(AbilityDefines.Event.OnBlockEnemy, attacker);
                        attacker.FireEventOnTarget(AbilityDefines.Event.OnHitEnemy, target);
                        break;
                    case AttackDefines.HitType.criticalHit:
                        attacker.FireEventOnTarget(AbilityDefines.Event.OnCritEnemy, target);
                        break;
                    default:
                        attacker.FireEventOnTarget(AbilityDefines.Event.OnHitEnemy, target);
                        break;
                }

                target.FireEventOnTarget(AbilityDefines.Event.DirectHitByEnemy, attacker);
            }

            attacker.FireEventOnTarget(AbilityDefines.Event.AttackHit, target);
            target.FireEventOnTarget(AbilityDefines.Event.OnHitByEnemy, attacker);
        }
        return hitType;
    }
    public override void ComputeDamageTable(DataItemUnit target)
    {
        target.damageable.lastDamage = new DamageTable(attacker, target, DetermineHitType(target));
    }
}

