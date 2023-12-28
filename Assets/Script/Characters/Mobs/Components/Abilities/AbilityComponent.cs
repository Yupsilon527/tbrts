using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PropertyAbility;

public class AbilityComponent : AbilityBaseComponent, IOnIntrerupt
{
    List<PropertyAbility> attacks;
    List<PropertyAbility> spells;

    public override void InitAbilities()
    {
        attacks = new List<PropertyAbility>();
        spells = new List<PropertyAbility>();
    }
    public void OnSpawn()
    {
        foreach (PropertyAbility ability in GetAvailableAbilities(false))
        {
            ability.Reset();
            ability.SetActive(true);

        }
    }
    /*public override void OnDespawn()
    {
        base.OnDespawn();
    }*/

    public override void AddAbility(PropertyAbility ability, bool active = false)
    { if (ability.IsBasicAttack())
        {
            attacks.Add(ability);
        }
        else
        {
            spells.Add(ability);
        }
        base.AddAbility(ability, active);
    }


    public void AddAbility( AbilitySO ability, bool active = false)
    {
         AddAbility(new PropertyAbility(parent,ability), active);
    }
    public override void RemoveAbility(PropertyAbility ability)
    {
        if (ability.IsBasicAttack())
        {
            attacks.Remove(ability);
        }
        else
        {
            spells.Remove(ability);
        }
        base.RemoveAbility(ability);
    }
    public bool HasAbility(PropertyAbility ability)
    {

        foreach (PropertyAbility ab in (ability.IsBasicAttack() ? attacks:spells))
        {
            if (ab == ability || ab.original == ability.original)
                return true;
        }
        return false;
    }
    public bool HasAbility(AbilitySO ability)
    {
        foreach (PropertyAbility ab in (ability.GetAbilityBehavior() == AbilityDefines.Behavior.target  ? attacks : spells))
        {
            if (ab.original == ability)
                return true;
        }
        return false;
    }
    public PropertyAbility FindAbilityByType(AbilityDefines.AbilityType abilityType)
    {
        foreach (PropertyAbility ab in spells)
        {
            if (ab.GetAbilityType() == abilityType)
                return ab;
        }
        foreach (PropertyAbility ab in attacks)
        {
            if (ab.GetAbilityType() == abilityType)
                return ab;
        }
        return null;
    }
    public bool ResolveCastData(CastTable castData)
    {
        if (parent.CanCast() && CanIntrerruptCasting() && castData.ability.IsFullyCastable() )
        {
            if (castData.ability.CanCastOnPoint(castData.point) && (!castData.ability.RequiresUnitTarget() || castData.target != null))
            {
                StartCasting(castData);
                return true;
            }
        }
        return false;
    }
    public CastTable CastAbility(PropertyAbility ability)
    {
        return CastAbilityOnPoint(ability, transform.position);
    }
    public CastTable CastAbilityOnPoint(PropertyAbility ability, Vector2 point)
    {
        CastTable castData = new CastTable(parent, ability);
        castData.UpdatePointTarget(point);
        return castData;
    }
    public CastTable CastAbilityOnTarget(PropertyAbility ability, Mob target)
    {
        CastTable castData = new CastTable(parent, ability);
        castData.UpdatePointTarget(target);
        return castData;
    }
    void DoAbilityVisuals(CastTable castData)
    {
            foreach (SpecialEffectSO specialEffect in castData.ability.original.AbilityVisuals)
        {
            EffectCastData(specialEffect, castData);
        }
        /*foreach (IAbilityEffect ef in castData.ability.GetEffects())
        {
            if (ef != null && ef.AttackVisuals!=null)
            {
                foreach (SpecialEffectSO effect in ef.AttackVisuals)
                {
                    EffectCastData(effect, castData);
                }
            }
        }*/
    }
    void EffectCastData(SpecialEffectSO specialEffect, CastTable castData)
    {
        if (specialEffect == null)
        {
            Debug.LogWarning("[EffectSO] Failed to create effect for ability " + name);
            return;
        }

            //castData.caster.GetComponent<Animation>().QueueAttackAnimation(castData.ability, delay, EntityDefines.AttackAnimationTime * repeatMult, castData.point);
            specialEffect.MakeEffect(castData, 0);
        
    }
    void StartCasting(CastTable castData)
    {
        StopCasting();
        cCastData = castData;
        StartCoroutine(ChannelSpell());
    }
    void ConcludeChanneling(bool success)
    {
        cCastData.UpdateHits(true);
        cCastData.ability.FireEvent(success ? AbilityDefines.Event.AbilitySuccess : AbilityDefines.Event.AbilityFail, cCastData);
        if (success && cCastData.ability.IsBasicAttack()) cCastData.caster.FireEventOnSelf( AbilityDefines.Event.AttackLanded);

        castCoroutine = null;
    }
    public bool IsCasting()
    {
        return castCoroutine != null;
    }
    public void StopCasting()
    {
        if (castCoroutine!= null)
        {
            ConcludeChanneling(false);
        }
    }
    public bool CanIntrerruptCasting()
    {
        if (IsCasting())
        {
            if (!cCastData.ability.IsBasicAttack())
            {
                return false;
            }
        }
        return true;
    }
    Coroutine castCoroutine;
    CastTable cCastData;
    IEnumerator ChannelSpell()
    {
        //Init        
         if (!SanityCheck() || !cCastData.ability.CanBeCast())
        {
            yield return null;
        }
            else if (cCastData.target != null)
        {
            cCastData.point = cCastData.target.transform.position;
            cCastData.target.FireEventOnSelf(AbilityDefines.Event.OnTargetedByAbility);
        }
        else if (cCastData.ability.RequiresUnitTarget())
        {
            Debug.LogWarning($"Ability {cCastData.ability.original.name} tried to cast without a target.");
            yield return null;
        }
        
        cCastData.origin = cCastData.caster.transform.position;

        print(cCastData.caster.name + " casts ability " + cCastData.ability.original.GetTooltip(false) + " from " + cCastData.origin + " to " + cCastData.point);

        cCastData.ability.SpendResources();


        float channelDuration = cCastData.ability.GetCastTime();
        float interval = cCastData.ability.GetChannelInterval();
        //targets

        cCastData.ability.FireEvent(AbilityDefines.Event.AbilityBegin, cCastData);


        //effects
        DoAbilityVisuals(cCastData);


        if (interval > 0)
        {
            DisplayAbilityThreat(interval);

        //each channel
        channelloop:
            {
                if (!SanityCheck() || (cCastData.ability.IsBasicAttack() && !cCastData.caster.CanAttack()) || (!cCastData.ability.IsBasicAttack() && !cCastData.caster.CanCast()))
                {
                    ConcludeChanneling(false);
                }
                else
                {
                    cCastData.UpdateHits(true);
                    cCastData.ability.FireEvent(AbilityDefines.Event.AbilityChannel, cCastData);
                    if (channelDuration >= interval)
                    {
                        channelDuration -= interval;
                        DisplayAbilityThreat(interval);

                        yield return new WaitForSeconds(interval);
                        goto channelloop;
                    }
                    else
                    {
                        yield return new WaitForSeconds(channelDuration);
                    }
                }
            }
        }
        else 
        {
            if (channelDuration > 0)
            {
                DisplayAbilityThreat(channelDuration);
                yield return new WaitForSeconds(channelDuration);
            }
            if (!SanityCheck() || (cCastData.ability.IsBasicAttack() && !cCastData.caster.CanAttack()) || (!cCastData.ability.IsBasicAttack() && !cCastData.caster.CanCast()))
            {
                ConcludeChanneling(false);
            }
        }
        //post - cast
        ConcludeChanneling(true);
    }
    void DisplayAbilityThreat(float duration)
    {
        if (!parent.IsPlayerControlled() && cCastData.ability.GetAreaRange(false) > 0)
        {
            ThreatAssistant.main.PoolThreatIndicator(
                cCastData.ability.GetCastBehavior() == AbilityDefines.Behavior.self ? cCastData.origin : cCastData.point,
                duration,
                cCastData.ability.GetAreaRange(false)
                );
        }
    }
    #region Random Target And Closest Enemy

   /* Mob GetClosestTarget(HexCoords center, int searchrange, Mob caster, Mob.Alignment reqAlignment)
    {
        Mob[] elist = EntityController.main.FindEntitiesInCircle(center, searchrange);

        Mob ClosestTarget = elist[0];
        float closestDist = float.PositiveInfinity;
        foreach (Mob ebase in elist)
        {
            float sqrDist = ebase.movement.coords.DistanceFrom(center);
            if (sqrDist < closestDist && ClosestTarget.GetAlignment(caster) == reqAlignment)
            {
                ClosestTarget = ebase;
                closestDist = sqrDist;
            }
        }

        if (ClosestTarget.GetAlignment(caster) != reqAlignment)
            ClosestTarget = null;
        return ClosestTarget;
    }
    Mob GetRandomTarget(HexCoords center, int searchrange, Mob caster, Mob.Alignment reqAlignment)
    {
        List<Mob> tempList = new List<Mob>();
        tempList.AddRange(EntityController.main.FindEntitiesInCircle(center, searchrange));

        tempList.RemoveAll((Mob e) =>
        {
            return (e.GetAlignment(caster) != reqAlignment);
        });
        if (tempList.Count > 0)
        {
            return tempList[Mathf.FloorToInt(Random.Range(0, tempList.Count))];
        }
        return null;
    }*/

    #endregion

    List<PropertyAbility> available = new List<PropertyAbility>();
    public PropertyAbility[] GetAvailableAttacks(bool castable)
    {
        available.Clear();
        if (!castable || parent.CanAttack())
        {
            foreach (PropertyAbility ability in attacks)
            {
                if (ability == null)
                    continue;
                if (!castable || (ability.IsFullyCastable()))
                    available.Add(ability);
            }
        }
        return available.ToArray();
    }
    public PropertyAbility[] GetAvailableSpells(bool castable)
    {
        available.Clear();
        if (!castable || parent.CanCast())
        {
            foreach (PropertyAbility ability in spells)
            {
                if (ability == null)
                    continue;
                if (!castable || (ability.IsFullyCastable()))
                    available.Add(ability);
            }
        }
        return available.ToArray();
    }
    public override PropertyAbility[] GetAvailableAbilities(bool castable)
    {
        available.Clear();
        available.AddRange(GetAvailableSpells(castable));
        available.AddRange(GetAvailableAttacks(castable));
        return available.ToArray();
    }

    public void OnIntrerupt()
    {
        if (CanIntrerruptCasting())
        StopCasting();
    }
}
