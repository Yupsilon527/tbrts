using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableComponent : MobComponent, IEntityEvent
{
    public Resource Health;
    public Resource Shield;

    protected override void Awake()
    {
        Health = new Resource( 1, "Health", true, true);
        Shield = new Resource( 1, "Shield", false, false);

        base.Awake();
        if (parent.healthBar!=null)
        { parent.healthBar.AssignDamageable(this); }
    }
    /*public override void OnSpawn()
    {
    }
    public override void OnReset()
    {
        Health.SetLimit(1, Resource.LimitRule.fullheal_value);
        Shield.SetValue(0);
    }
    public override void OnDespawn()
    {
        Health.SetValue(0);
        base.OnDespawn();
    }*/
    public void ApplyAttack(PropertyAbility sourceAbility, Mob[] targets, AttackData atk, AttackDefines.DamageFlag flags, float animdelay)
    {
        if (atk == null) return;
        foreach (Mob target in targets)
        {
            ApplyAttack(sourceAbility, target, atk, flags, animdelay);
        }
    }
    public void ApplyAttack(PropertyAbility sourceAbility, Mob target, AttackData atk, AttackDefines.DamageFlag flags, float animdelay)
    {
        if (atk == null) return;
        float damage_total = atk.BaseDamage;
        float bonus_damage = 0;

        if (atk.ScaleType ==  AttackData.ScaleOff.attack)
        {
            bonus_damage = atk.ScaleDamage * parent.stats.RealAttackDamage;
        }
        else if (atk.ScaleType == AttackData.ScaleOff.special)
        {
            bonus_damage = atk.ScaleDamage * parent.stats.RealSpecialDamage;
        }

        damage_total += bonus_damage;

        
        target.damageable?.RecieveDamage(parent, sourceAbility, damage_total, atk.attack, flags, animdelay);

    }

    public void RecieveDamage(Mob attacker, PropertyAbility sourceAbility, float damage, AttackDefines.AttackType attack, AttackDefines.DamageFlag flags, float animdelay)
    {
        ResolveDamage(attacker, parent, sourceAbility, damage, attack, flags, animdelay);
    }
    public static void ResolveDamage(Mob attacker, Mob target, PropertyAbility sourceAbility,  float damage, AttackDefines.AttackType attack, AttackDefines.DamageFlag flags, float animdelay)
    {
        if (target == null || target.damageable == null || !target.damageable.SanityCheck())
            return;
        switch (flags)
        {
            case AttackDefines.DamageFlag.Melee:
                damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_melee);
                damage *= target.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.incoming_melee);
                break;
            case AttackDefines.DamageFlag.Range:
                damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_range);
                damage *= target.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_range);
                break;
            case AttackDefines.DamageFlag.Area:
                damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_area);
                damage *= target.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.incoming_area);
                break;
        }
        Debug.Log($"Apply attack {attack} to {target.name} for {damage} damage");
        switch (attack)
        {
            //  ----------------
            // ----  DIRECT  ----
            //  ----------------

            case AttackDefines.AttackType.Physical:
                target.damageable.DealDamage(new DamageTable(attacker, target, damage, AttackDefines.DamageType.Physical, flags), animdelay);
                break;
            case AttackDefines.AttackType.Magical:
                target.damageable.DealDamage(new DamageTable(attacker, target, damage, AttackDefines.DamageType.Magical, flags), animdelay);
                break;
            case AttackDefines.AttackType.Heal:
                target.damageable.DealDamage(new DamageTable(attacker, target, damage, AttackDefines.DamageType.LifeHeal, flags), animdelay);
                break;
            case AttackDefines.AttackType.Shield:
                target.damageable.DealDamage(new DamageTable(attacker, target, damage, AttackDefines.DamageType.ShieldHeal, flags), animdelay);
                break;
            case AttackDefines.AttackType.Taunt:
                if (target.combatant != null)
                    target.combatant.IssueAttackOrder(attacker);
                break;
            default:
                Debug.LogWarning("[AttackData] " + attack + " has not been implemented!");
                break;
        }
    }
    public DamageTable lastDamage;
    protected bool dead = false;
    public virtual void DealDamage(DamageTable damage, float animdelay)
    {

        lastDamage = damage;
        damage.Calculate();

            Debug.Log("[Health] " + damage.attacker?.name + " deals "+damage.real_damage+" "+ damage.dmt+ " to " + name);


        switch (damage.dmt)
        {
            case AttackDefines.DamageType.Physical:
            case AttackDefines.DamageType.Magical:
                float outDamage = AccountResistances(damage.real_damage, damage.dmt == AttackDefines.DamageType.Physical ? damage.target.stats.RealArmor : damage.target.stats.RealResistance);


        UpdateKiller(damage.attacker);
                TakeDirectDamage(outDamage);

                parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                if (damage.IsDirectDamage())
                {
                    parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDirectDamage);
                }
                break;
            case AttackDefines.DamageType.LifeHeal:
                Health.GiveValue(damage.real_damage);
                parent.FireEventOnSelf(AbilityDefines.Event.OnHealRecieved);
                break;
            case AttackDefines.DamageType.ShieldHeal:
                Shield.GiveValue(damage.real_damage);
                parent.FireEventOnSelf(AbilityDefines.Event.OnShieldRecieved);

                break;
        }

    }
    void TakeDirectDamage(float damage)
    {

        /*if (damage.dmt == AttackDefines.DamageType.Poison)
        {
            resultDamage = Mathf.Clamp(resultDamage, 0, Health.GetValue() - 1);
        }
        else*/
        {
            TakeShieldDamage(damage, out float shieldblock);
            damage -= shieldblock;
        }

        TakeLifeDamage(damage, out float finaldamage);

        //parent.sayer.IncreaseTakenDamage(finaldamage, animdelay);
    }
    public static float AccountResistances(float damage, float resistance)
    {
        float arMult = AttackDefines.ArmorMitigation / (AttackDefines.ArmorMitigation + resistance);
        return damage * arMult;
    }
    void TakeShieldDamage(float damagevalue, out float blockeddamage)
    {
        blockeddamage = 0;
        if (damagevalue == 0)
            return;

        float shValue = Shield.GetValue();
        if (shValue > 0)
        {
            float efficiency = 1;
            if (parent.modifiers != null)
                efficiency = parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.inc_damage_shield);
            if (shValue > damagevalue * efficiency)
            {
                Shield.SubstractValue(damagevalue * efficiency);
                blockeddamage = damagevalue;
                parent.FireEventOnSelf(AbilityDefines.Event.OnShieldBlock);
            }
            else
            {
                blockeddamage = shValue / efficiency;
                Shield.SetValue(0);
                parent.FireEventOnSelf(AbilityDefines.Event.OnShieldBreak);
            }
        }
        
    }
    void TakeLifeDamage( float damagevalue, out float recievedamage)
    {
        recievedamage = 0;
        if (damagevalue == 0)
            return;
        if (parent.modifiers != null)
            recievedamage = damagevalue * parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.inc_damage_life);
        else
            recievedamage = damagevalue;

        Health.SubstractValue(recievedamage);
        parent.FireEventOnSelf(AbilityDefines.Event.OnTakeLifeDamage);
        Debug.Log("[Damageable] " + name + " recieves " + damagevalue + " leaving it at " + Health.GetValue() + " life");
        

            CheckDeath();
        
    }
    public void CheckDeath()
    {
        if (!isAlive())
        {
            Kill();
        }

    }
    public  void Kill()
    {
        if (!dead)
            Die( );
    }
    protected virtual void Die( )
    {
        if (!dead )
        {
            dead = true;
            Debug.Log("[Health] "+name+" has died!");

            parent.Die();
           deathCycle = StartCoroutine(DeathCoroutine());

            //stateInGame.main.triggermanager.TriggerAction(TriggerDefines.TriggerEvent.unitDies, this);
        }
    }
    
    Coroutine deathCycle;
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(parent.DeathTime);
        parent.FireEventOnTarget(AbilityDefines.Event.OnDeath, killer);
        if (killer != null)
        {
            killer.FireEventOnTarget(AbilityDefines.Event.OnScoreKill, parent);
        }
        parent.Despawn();
        deathCycle = null;
    }
    #region Killing Entity
    public Mob killer;
    void UpdateKiller(Mob attacker)
    {
        if (attacker != parent)
            killer = attacker;
    }
    void ClearKiller()
    {
        killer = null;
    }
    #endregion


    public virtual bool isAlive()
    {
        return !dead && Health.GetValue() > 0;
    }
    void Undie()
    {
        if (deathCycle!=null)
        {
            StopCoroutine(deathCycle);
        }
        dead = false;
        ClearKiller();
        Health.SetPercentage(1);
        Shield.SetPercentage(0);
    }
    public void EventReaction(AbilityDefines.Event evt, Mob[] affectedCritters)
    {
         if (evt == AbilityDefines.Event.OnCreated)
        {
            Undie();
        }
    }
}
