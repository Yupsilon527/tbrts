using UnityEngine;
using Random = UnityEngine.Random;

public class CastTable
{
    public bool attackingSide = false;
    public float proc = 0;
    public DataItemUnit caster;
    public Vector2Int targetPoint;
    public PropertyAbility ability;
    public AttackDefines.HitType hit = AttackDefines.HitType.normal;

    public DataItemUnit[] maintarget;
    public DataItemUnit[] sidetarget;

    public CastTable(DataItemUnit caster, Vector2Int targetPoint, PropertyAbility ability)
    {
        this.caster = caster;
        this.targetPoint = targetPoint;
        this.ability = ability;
        ComputeTargets();
    }


    public void ComputeTargets()
    {
        maintarget = ability.GetMainTargets(this);
        sidetarget = ability.GetSideTargets(this);
    }
    public virtual void Resolve()
    {

    }
        /*
        public void Resolve()
        {
            if (action == CombatDefines.Action.Attack)
            {
                DetermineHitType();
                caster.FireEventOnSelf(AbilityDefines.Event.BeforeAttack);
            }
            Combat.main.Inspect($"Resolve {action} from {caster} to {maintarget}");
            hit = Random.value < caster.stats.realStats.BlockChance;
            foreach (var effect in caster.abilities._effects)
            {
                if (effect.appliedEffect.action == action)
                    effect.appliedEffect.Resolve(this, proc);
                else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnCrit)
                    effect.appliedEffect.Resolve(this, proc * 2);
                else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnParried)
                    effect.appliedEffect.Resolve(this, proc * .1f);
                else if (effect.appliedEffect.action == CombatDefines.Action.OnHit && action == CombatDefines.Action.OnMiss)
                    effect.appliedEffect.Resolve(this, proc * .5f);
            }
            if (action == CombatDefines.Action.OnParried)
            {
                maintarget.abilities.Action(caster, CombatDefines.Action.ParryAttack, tick);
                maintarget.FireEventOnSelf(AbilityDefines.Event.SuccessfulParry);
            }
            if (action == CombatDefines.Action.OnMiss || action == CombatDefines.Action.OnDodge)
            {
                maintarget.abilities.Action(caster, CombatDefines.Action.EvadeAttack, tick);
                maintarget.FireEventOnSelf(AbilityDefines.Event.SuccessfulEvade);
            }
            else
            {
                maintarget.FireEventOnSelf(AbilityDefines.Event.OnHitByEnemy);
                caster.FireEventOnSelf(AbilityDefines.Event.AttackHit);
            }
            if (hit)
            {
                maintarget.FireEventOnSelf(AbilityDefines.Event.SuccessfulBlock);
                maintarget.abilities.Action(caster, CombatDefines.Action.Block, tick);
            }
            if (directHit)
            {
                caster.FireEventOnSelf(AbilityDefines.Event.AfterAttack);
                maintarget.abilities.Action(caster, CombatDefines.Action.Retaliate, tick);
            }
        }*/
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
    public int tick = 0;
    void DetermineHitType()
    {
        directHit = true;
        action = CombatDefines.Action.OnHit;

        if (caster.stats.realStats.DodgeChance > 0)
        {
            if (UnityEngine.Random.value < maintarget.stats.realStats.DodgeChance * (maintarget.dodgeCounter / 2)) //Dodge chance
            {
                action = CombatDefines.Action.OnDodge;
                maintarget.dodgeCounter = 1;
                return;
            }
            else
            {
                maintarget.dodgeCounter++;
            }
        }
        float accuracy = caster.stats.realStats.Offense / Mathf.Max(maintarget.stats.realStats.Defense);
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

            caster.abilities.Trigger(maintarget, CombatDefines.Events.HitsLanded, 1);
            maintarget.abilities.Trigger(maintarget, CombatDefines.Events.HitsTaken, 1);
        }
    }
}

