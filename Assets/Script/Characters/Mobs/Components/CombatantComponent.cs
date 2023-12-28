using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatantComponent : MobComponent,IOnIntrerupt, IEntityEvent
{
    public void EngageCombat(Mob other)
    {
        if (SanityCheck() && CanFightTarget(other))
        {
            Debug.Log($"{name} starts fighting with {other.name}");
            attackTarget = other;
            enabled = true;
        }
    }
    public void EndCombat()
    {
        if (enabled)
            enabled = false;
    }
    void Disengage()
    {
        attackTarget = null;
        nextThink = 0;
    }
    private void OnDisable()
    {
        Disengage();
    }
    public bool CanFightTarget(Mob other)
    {
        if (other == parent)
            return false;

        Mob.Alignment alignment = parent.GetAlignment(other);
        if (alignment == Mob.Alignment.enemy)
            return TargetsEnemies;
        else
            return TargetsAllies;
    }

    public Mob attackTarget;
    private void Update()
    {
        HandleCombat();
    }
    float nextThink = 0;
    public bool IsInRange = false;
    protected bool CombatCheck()
    {
        if (nextThink > Time.time  ) return false;
        if (!SanityCheck() || attackTarget == null || !attackTarget.gameObject.activeSelf || !attackTarget.damageable.isAlive() || attackTarget.IsInvulnerable())
        {
            EndCombat();
            return false;
        }
        if (parent.abilities.IsCasting())
        {
            return false;
        }
        return true;
    }
    void HandleCombat()
    {
        if (!CombatCheck()) return;
        if ( IsInRangeOfTarget())
        {
            AttackTarget();
            if (!attackTarget.damageable.isAlive())
            {
                EndCombat();
            }
        }
        else if (parent.movement != null && !parent.movement.IsWalking() && !anchored)
        {
            ApproachTarget();
        }
        else
        {
            OnTargetOutOfRange();
        }
    }
    protected virtual void OnTargetOutOfRange()
    {

    }
    protected virtual void AttackTarget()
    {
        parent.movement.FacesRight = attackTarget.transform.position.x > transform.position.x;
        CastAbilityOnTarget(activeAbility, attackTarget);
    }
    protected void CastAbilityOnTarget(PropertyAbility ability, Mob target)
    {
        if (parent.CanCast() && ability.CanBeCast() && ability.CanCastOnTarget(target))//TODO account attacks
        {
            CastTable cast = parent.abilities.CastAbilityOnTarget(ability, target);
            if (parent.abilities.ResolveCastData(cast)) 
                nextThink = Time.time + ability.GetCastTime();
        }
    }
  protected virtual  void ApproachTarget()
    {
        if (attackTarget != null)
        {
            parent.movement.Follow(attackTarget.gameObject);
            nextThink = Time.time + .5f;//TODO calc
        }
    }
    PropertyAbility activeAbility;

    float castRange = 0;
    bool TargetsSelf;
    bool TargetsEnemies;
    bool TargetsAllies;
    public void AutoAssignActiveAbility()
    {
        if (activeAbility==null && parent.abilities!=null)
        {
            foreach (PropertyAbility ab in parent.abilities.GetAvailableAttacks(false))
            {
                if (ab.CanBeCast())
                {
                    SetActiveAbility(ab);
                    return;
                }
            }
        }
    }
    public void SetActiveAbility(PropertyAbility ab)
    {
        if (!ab.IsBasicAttack())
        {
            Debug.LogWarning($"{name} tried to equipt non basic ability {ab.original.name}!");
            return;
        }
        activeAbility = ab;
        nextThink = 0;

        castRange = ab.GetMaxRange(false);
        TargetsEnemies = ab.CheckFlag(AbilityDefines.Flag.enemies);
        TargetsAllies = ab.CheckFlag(AbilityDefines.Flag.allies);
        TargetsSelf = ab.CheckFlag(AbilityDefines.Flag.selfcast);
    }
    public PropertyAbility GetActiveAbility()
    {
        return activeAbility;
    }
    public bool IsInRangeOfTarget()
    {
        if (attackTarget == null || activeAbility==null) return false;
        IsInRange =((Vector2)attackTarget.transform.position - (Vector2)transform.position).sqrMagnitude < castRange* castRange;
        return IsInRange;
    }

    public void OnIntrerupt()
    {
        EndCombat();
    }
    #region Aggro Targets
    public void AggroRandomTarget()
    {
        bool alignment = parent.IsPlayerControlled();
        alignment = TargetsAllies ? alignment : !alignment;

        Mob[] targets = parent.transition.CurrentRoom.LocalMobs.Filter(includePlayer: alignment, includeEnemies:!alignment);
        if (targets.Length > 0)
            IssueAttackOrder(targets[Random.Range(0, targets.Length)]);
    }
    public void SearchNextTarget(float AggroRange)
    {
        if (activeAbility == null) return;
        if (activeAbility.GetAbilityType() == AbilityDefines.AbilityType.heal ||
            activeAbility.GetAbilityType() == AbilityDefines.AbilityType.buff)
        {
            AggroWeakestTarget(AggroRange);
        }
        else
        {
            AggroClosestTarget(AggroRange);
        }
    }
    public void AggroClosestTarget(float AggroRange)
    {
        bool alignment = parent.IsPlayerControlled();
        alignment = TargetsAllies ? alignment : !alignment;

        Mob closestTarget = null;
        float minDistance = 0;
        foreach (Mob target in
            ((AggroRange<=0) ? parent.transition.CurrentRoom.LocalMobs.Filter(includeEnemies:!alignment, includePlayer: alignment) : parent.transition.CurrentRoom.LocalMobs.FindEntitiesInCircle(transform.position, AggroRange, includeEnemies: !alignment, includePlayer: alignment)))
        {
            if (target != null && !target.IsInvulnerable())
            {
                float mDist = (transform.position - target.transform.position).sqrMagnitude;
                if (closestTarget == null || mDist < minDistance)
                {
                    closestTarget = target;
                    minDistance = mDist;
                }
            }
        }
        if (closestTarget != null)
            IssueAttackOrder(closestTarget);
    }
    public void AggroWeakestTarget(float AggroRange)
    {
        bool alignment = parent.IsPlayerControlled();
        alignment = TargetsAllies ? alignment : !alignment;

        Mob closestTarget = null;
        float weakestLife = 0;
        foreach (Mob target in
            ((AggroRange < 0) ? parent.transition.CurrentRoom.LocalMobs.Filter(includePlayer:alignment, includeEnemies:!alignment) : parent.transition.CurrentRoom.LocalMobs.FindEntitiesInCircle(transform.position, AggroRange, includePlayer: alignment, includeEnemies: !alignment)))
        {
            if (target != null && !target.IsInvulnerable())
            {
                float mlaif = target.damageable.Health.GetPercentage();
                if(mlaif < 1 && (closestTarget == null ||  mlaif < weakestLife))
                {
                    closestTarget = target;
                    weakestLife = mlaif;
                }
            }
        }
        if (closestTarget != null)
            IssueAttackOrder(closestTarget);
    }
    public virtual void IssueAttackOrder(Mob target)
    {
        if (target != null && !target.IsInvulnerable())
            parent.orders.GiveOrder(new OrdersComponent.AttackOrder(target), -1);
    }
    #endregion
    #region Anchored
    bool anchored = false;
    public void SetAnchored(bool value)
    {
        anchored = value;
    }
    #endregion
    #region Retaliation


    public void EventReaction(AbilityDefines.Event evt, Mob[] affectedCritters)
    {
        if (evt == AbilityDefines.Event.OnTakeDamage && !parent.combatant.enabled && parent.damageable.lastDamage != null && parent.orders.IsIdle())
        {
            IssueAttackOrder(parent.damageable.lastDamage.attacker);
        }
    }
    #endregion
}
