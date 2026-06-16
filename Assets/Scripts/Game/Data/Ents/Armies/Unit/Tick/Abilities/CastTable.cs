using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CastTable
{
    public CombatDefines.AttackPhase phase;
    public bool directHit = false;
    public AttackDefines.HitType hit =  AttackDefines.HitType.normal;
    public int tick = 0;
    public float proc = 0;
    public DataItemUnit caster;
    public Vector2Int target;

    public CastTable(DataItemUnit attacker, DataItemUnit target, CombatDefines.Action action, int tick, float proc = 1)
    {
        this.caster = attacker;
        this.target = target;
        this.action = action;
        this.tick = tick;
        this.proc = proc;
    }

    public void Resolve()
    {
        if (action == CombatDefines.Action.Attack)
        {
            DetermineHitType();
            caster.FireEventOnSelf(AbilityDefines.Event.BeforeAttack);
        }
        Combat.main.Inspect($"Resolve {action} from {caster} to {target}");
        hit = Random.value < caster.stats.realStats.BlockChance;
        foreach (var effect in caster.abilities._effects)
        {
            if (effect.appliedEffect.action == action)
                effect.appliedEffect.Resolve(this, proc);
            else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnCrit)
                effect.appliedEffect.Resolve(this, proc*2);
            else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnParried)
                effect.appliedEffect.Resolve(this, proc * .1f);
            else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnMiss)
                effect.appliedEffect.Resolve(this, proc * .5f);
        }
        if (action == CombatDefines.Action.OnParried)
        {
            target.abilities.Action(caster, CombatDefines.Action.ParryAttack, tick);
            target.FireEventOnSelf(AbilityDefines.Event.SuccessfulParry);
        }
        if (action == CombatDefines.Action.OnMiss || action == CombatDefines.Action.OnDodge)
        {
            target.abilities.Action(caster, CombatDefines.Action.EvadeAttack, tick);
            target.FireEventOnSelf(AbilityDefines.Event.SuccessfulEvade);
        }
        else
        {
            target.FireEventOnSelf(AbilityDefines.Event.OnHitByEnemy);
            caster.FireEventOnSelf(AbilityDefines.Event.AttackHit);
        }
        if (hit)
        {
            target.FireEventOnSelf(AbilityDefines.Event.SuccessfulBlock);
            target.abilities.Action(caster, CombatDefines.Action.Block, tick);
        }
        if (directHit)
        {
            caster.FireEventOnSelf(AbilityDefines.Event.AfterAttack);
            target.abilities.Action(caster, CombatDefines.Action.Retaliate, tick);
    }
    }

    void DetermineHitType()
    {
        directHit = true;
        action = CombatDefines.Action.OnHit;

        if (caster.stats.realStats.DodgeChance > 0)
        {
            if (UnityEngine.Random.value < target.stats.realStats.DodgeChance * (target.dodgeCounter / 2)) //Dodge chance
            {
                action = CombatDefines.Action.OnDodge;
                target.dodgeCounter = 1;
                return;
            }
            else
            {
                target.dodgeCounter++;
            }
        }
        float accuracy = caster.stats.realStats.Offense / Mathf.Max(target.stats.realStats.Defense);
         accuracy = accuracy * .6f + Mathf.Min(.4f, caster.hitCounter / 2 * .5f); //TODO DEFINE

        float critChance = 15 * accuracy;   //TODO DEFINE
        float hitChance = 50 * accuracy;
        float missChance = 30 / accuracy;
        float parryChance = 20 / accuracy;

        float ranval = Random.value * (critChance + hitChance + missChance + parryChance);
        if (ranval < missChance)
        {
            action = ranval< parryChance ? CombatDefines.Action.OnParried : CombatDefines.Action.OnMiss;
            caster.hitCounter++;
        }
        else
        {
            action = (ranval > parryChance + missChance + hitChance) ? CombatDefines.Action.OnCrit : CombatDefines.Action.OnHit;
            caster.hitCounter = 1;

            caster.abilities.Trigger(target, CombatDefines.Events.HitsLanded, 1);
            target.abilities.Trigger(target, CombatDefines.Events.HitsTaken, 1);
        }
    }
}

[Serializable]
public class CombatantAbilityTable
{
    public AbilityData abilityEvent;

    public ApplyAttack[] attacks;
    public ApplyModifier[] modifiers;
    public ApplyProc[] procs;
}
[Serializable]
public class AbilityData
{
    public CombatDefines.Action abilityEvent;
    public CombatDefines.Events abilityCondition;
    public CombatDefines.TargetType abilityTarget;
    public int abilityCooldown;
    public float abilityStrength;
    public virtual string GetDescription()
    {
        return $"{abilityEvent} {abilityTarget} every {abilityCooldown} {abilityCondition}";
    }
}
