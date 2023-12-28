using System;
using System.Collections.Generic;
using UnityEngine;


public class PropertyAbility
{
    public bool isActive = false;
    public Mob caster;
    public override string ToString()
    {
        return "PropertyAbility " + original.name;
    }

    #region GENERAL AND STATS
    public void SetActive(bool value)
    {
        if (isActive != value)
        {
            if (value)
            {
                AbilityEvent(AbilityDefines.Event.OnAbilityEnabled, new CastTable(caster));
            }
            else if (isActive)
            {
                AbilityEvent(AbilityDefines.Event.OnAbilityDisabled, new CastTable(caster));
            }
            isActive = value;
        }
    }

    public AbilitySO original;
    public PropertyAbility(Mob owner, AbilitySO Data)
    {
        this.caster = owner;
        if (Data != null)
            LoadScriptableData(Data);
        Reset();
        FireEvent(AbilityDefines.Event.OnCreated);
    }
    public void Reset()
    {
        isActive = false;
        RefreshCooldown();
    }

    void LoadScriptableData(AbilitySO orig)
    {
        original = orig;
        AbilityBehavior = orig.GetAbilityBehavior();
        CheckHitMobs = orig.CheckTargetMobs();
        AbilityFlags = orig.GetAbilityFlags();
        AbilityFunctions = orig.TranslateFunctions();
        //AbilityType = orig.get
    }
    public AbilityDefines.Behavior AbilityBehavior;
    public AbilityDefines.CheckHitMobs CheckHitMobs;

    #region Flags
    int AbilityFlags = 0;
    public bool CheckFlag(AbilityDefines.Flag flag)
    {
        return CheckFlag((int)flag);
    }
    public bool CheckFlag(int flag)
    {
        return (flag & AbilityFlags) == flag;
    }
    #endregion

    public bool IsFullyCastable()
    {
        return CanBeCast() && HasResourcesToCast();
    }

    public virtual bool CanBeCast()
    {
        return (isActive && GetCastBehavior() != AbilityDefines.Behavior.passive);
    }

    public bool RequiresUnitTarget()
    {
        return GetCastBehavior() == AbilityDefines.Behavior.target;
    }

    public AbilityDefines.Behavior GetCastBehavior()
    {
        return AbilityBehavior;
    }
    public bool IsBasicAttack()
    {
        return CheckFlag(AbilityDefines.Flag.basicattack);
    }
    public AbilityDefines.AbilityType GetAbilityType()
    {
        return original.GetAbilityType();
    }

    public float GetMinRange(bool raw)
    {
        if (!raw && GetAreaRange(true) > 0)
            return Mathf.Max(GetAreaRange(false), original.GetMinRange());
        return original.GetMinRange();
    }

    public float GetMaxRange(bool raw)
    {
        float range = original.GetMaxRange();
        /* if (!raw && owner.modifiers!=null)
         {
             range = Mathf.Max(0, (range + owner.modifiers.GetPropertyAdditive(ModifierDefines.modProps.ability_cast_range) + GetAlterationAdditive(AbilityDefines.AbilityAlterations.range)));
         }*/
        return range;
    }
    public int GetAreaRange(bool raw)
    {
        float range = original.GetAoERange();
        /*if (!raw && owner.modifiers != null)
        {
            range = Mathf.Max(0, (range + owner.modifiers.GetPropertyAdditive(ModifierDefines.modProps.ability_aoe_range) + GetAlterationAdditive(AbilityDefines.AbilityAlterations.area)));
        }*/
        return Mathf.RoundToInt(range);
    }
    #endregion

    #region Targeting
    public bool IsValidTarget(Mob target)
    {
        if (target == caster && CheckFlag(AbilityDefines.Flag.selfcast))
        {
            return true;
        }
        if (target.IsInvulnerable() && !CheckFlag(AbilityDefines.Flag.invulnerable))
        {
            return false;
        }

        if (!CheckFlag(AbilityDefines.Flag.hurt) || target.damageable.Health.GetPercentage() < 1)
        {

            if ((caster.GetAlignment(target) == Mob.Alignment.enemy && CheckFlag(AbilityDefines.Flag.enemies)) ||
                (caster.GetAlignment(target) > Mob.Alignment.enemy && CheckFlag(AbilityDefines.Flag.allies)))

            { return true; }
        }
        return false;
    }

    public bool CanCastOnPoint(Vector2 point)
    {
        return CanCastOnPoint(caster.transform.position, point);
    }
    public bool CanCastOnTarget(Mob target)
    {
        return IsValidTarget(target) && CanCastOnPoint(caster.transform.position, target.transform.position);
    }
    public bool IsInCastRange(Vector2 origin, Vector2 point)
    {
        float rangeSqrt = (origin - point).sqrMagnitude;

        float minRange = GetMinRange(true) == 0 ? 0 : GetMinRange(false);
        float maxRange = GetMaxRange(false);
        return rangeSqrt > minRange * minRange && rangeSqrt < maxRange * maxRange;
    }
    public bool CanCastOnPoint(Vector2 origin, Vector2 point)
    {
        //Debug.Log("[Ability] Check if ability type "+ GetAbilityType() + " castable on title from " + origin + " to " + point);
        switch (GetCastBehavior())
        {
            default://self
                return true;
            case AbilityDefines.Behavior.passive://passive
                return false;
            case AbilityDefines.Behavior.point:
            case AbilityDefines.Behavior.target:
                return IsInCastRange(origin, point);
        }
    }
    #endregion
    #region Resources
    public virtual bool HasResourcesToCast()
    {
        if (caster.IsPlayerControlled())
        {
            if (PlayerController.main.resources.mana.GetValue() < GetAbilityCost())//TODO nullchecks
                return false;
        }
        return (GetCooldownTimeRemaining() < 0);
    }
    public void SpendResources()
    {
        if (caster.IsPlayerControlled())
        {
            PlayerController.main.resources.mana.ChargeValue(GetAbilityCost());//TODO nullchecks
        }

        FireCooldown(GetCooldownTime());
    }
    #endregion
    #region Cooldown
    float LastCastTime = -1;
    public float GetCooldownTime()
    {
        return original.GetBaseCooldown();
    }
    public float GetCooldownTimeRemaining()
    {
        return LastCastTime - Time.time;
    }
    public void FireCooldown(float cooldownTime)
    {
        LastCastTime = Time.time + cooldownTime;
    }
    public void ExtendCooldown(int cooldownTime)
    {
        if (GetCooldownTimeRemaining() > 0)
            LastCastTime += cooldownTime;
    }
    public void ReduceCooldown(int value, bool absolute)
    {
        if (absolute)
        {
            LastCastTime -= value;
        }
        else
        {
            LastCastTime = Mathf.Min(Time.time + 1, LastCastTime - value);
        }
    }
    public void RefreshCooldown()
    {
        LastCastTime = -1;
    }
    #endregion
    public float GetAbilityCost()
    {
        return original.GetBaseCost();
    }
    #region Events
    public void FireEvent(AbilityDefines.Event fct, Mob target)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new CastTable(caster, this, target, new Mob[] { target }));
    }
    public void FireEvent(AbilityDefines.Event fct, Mob[] targets)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new CastTable(caster, this, null, targets));
    }
    public void FireEvent(AbilityDefines.Event fct)
    {
        FireEvent(fct, new CastTable(caster, this));
    }
    public void FireEvent(AbilityDefines.Event fct, CastTable table)
    {
        if (!HasEvent(fct)) return;

        AbilityEvent(fct, table);
    }
    public bool HasEvent(AbilityDefines.Event evt)
    {
        if (AbilityFunctions != null)
            foreach (var fct in AbilityFunctions)
                if (fct.aEvent == evt)
                    return true;
        return false;
    }
    #endregion
    #region Ability Events
    public List<AbilitySO.AbilityListener> AbilityFunctions;
    void AbilityEvent(AbilityDefines.Event fct, CastTable table)
    {
        Debug.Log("[PropertyAbility] " + caster.name + " Fire Function " + fct);
        if (AbilityFunctions != null)
        {
            foreach (var item in AbilityFunctions)
            {
                if (item.aEvent == fct)
                {
                    item.aFunction.Invoke(table);
                }
            }
        }
    }
    #endregion
    #region Cast Time
    public float GetChannelInterval()
    {
        return original.GetChannelInterval();
    }
    public float GetCastTime()
    {
        return original.GetCastTime();
    }
    #endregion
}
