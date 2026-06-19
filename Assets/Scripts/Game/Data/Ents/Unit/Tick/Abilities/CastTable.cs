using UnityEngine;
using Random = UnityEngine.Random;

public class EventTable
{
    public int tick = 0;
    public DataItemUnit caster;
    public DataItemUnit target;

    public EventTable(CastTable table, DataItemUnit target)
    {
        this.target = target;
        this.caster = table.attacker;
        this.tick = table.tick;
    }

    public EventTable(int tick, DataItemUnit caster, DataItemUnit target)
    {
        this.tick = tick;
        this.caster = caster;
        this.target = target;
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

    public CastTable(DataItemUnit caster, Vector2Int targetPoint, PropertyAbility ability)
    {
        this.attacker = caster;
        this.targetPoint = targetPoint;
        this.ability = ability;
        ComputeTargets();
    }


    public void ComputeTargets()
    {
        maintarget = ability.GetMainTargets(this);
        sidetarget = ability.GetAreaTargets(this);
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
            if (!attacker.GetState(ModifierDefines.State.cannot_miss))
            {
                float evasion = target.dodgeCounter / 2f * target.stats.realStats.DodgeChance * target.stats.realStats.GetLuckCoefficient();

                if (ranval > Mathf.Min(AttackDefines.minAccuracy, evasion))
                {
                    target.dodgeCounter = 1;
                    return AttackDefines.HitType.miss;
                }
                else
                {
                    target.dodgeCounter++;
                }
            }
            float accuracy = attacker.stats.realStats.Offense / Mathf.Max(target.stats.realStats.Defense);
            accuracy = accuracy * .6f + Mathf.Min(.4f, attacker.hitCounter / 2 * .5f); //TODO DEFINE

            float critChance = 15 * accuracy + attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.critical_chance) * (1 + accuracy) / 2f;   //TODO DEFINE
            float hitChance = 50 * accuracy;
            float missChance = 30 / accuracy;
            float parryChance = 20 / accuracy + attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.parry_chance) * (1 + accuracy) / 2f;

            ranval = Random.value * (critChance + hitChance + missChance + parryChance);
            if (ranval < missChance)
            {
                hitType = ranval < parryChance ? AttackDefines.HitType.halfBlock : AttackDefines.HitType.blocked;
                attacker.hitCounter++;
            }
            else
            {
                hitType = (ranval > parryChance + missChance + hitChance) ? AttackDefines.HitType.criticalHit : AttackDefines.HitType.normal;
                attacker.hitCounter = 1;

                attacker.FireEventOnTarget(AbilityDefines.Event.AttackHit, target);
                target.FireEventOnTarget(AbilityDefines.Event.OnHitByEnemy, attacker);
            }
        }
        return hitType;
    }
    public override void ComputeDamageTable(DataItemUnit target)
    {
        target.damageable.lastDamage = new DamageTable(attacker, target, DetermineHitType(target));
    }
}

