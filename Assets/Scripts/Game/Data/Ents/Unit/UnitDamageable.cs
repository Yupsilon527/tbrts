
using UnityEngine;

public class UnitDamageable : UnitComponent
{
    public ResourceInt Health, Armor, Block;

    public DataItemUnit guardian;
    public DamageTable lastDamage;
    protected bool dead = false;
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        base.TriggerFuncs(act);
        if (act == AbilityDefines.Event.OnTurnBegin)
        {
            float regen = parent.innates.GetAbilityCombined(UnitDefines.ArmyAbilities.regen);
            Health.GiveValue(regen);
        }
        if (act == AbilityDefines.Event.OnSpawn)
        {
            Health.SetValue(parent.health.GetValue());
            ClearKiller();
        }
        if (act == AbilityDefines.Event.CombatBegin)
        {
            Armor.SetPercentage(1);
            Block.SetPercentage(0);
            ClearGuardian();
        }
        if (act == AbilityDefines.Event.Action)
        {
            ClearGuardian();
        }
        if (act == AbilityDefines.Event.CombatEnd)
        {
            Armor.SetPercentage(0);
            Block.SetPercentage(0);
        }
    }
    public UnitDamageable(DataItemUnit owner) : base(owner)
    {
        Health = new ResourceInt(1, "Health", false, true);
        Health.LimitUnder = Resource.LimitRule.percent_value;
        Health.LimitOver = Resource.LimitRule.percent_value;

        Armor = new ResourceInt(1, "Armor", false, true);
        Armor.LimitOver = Resource.LimitRule.leave_value;
        Armor.LimitUnder = Resource.LimitRule.leave_value;

        Block = new ResourceInt(1, "Block", false, false);
        Block.SetValue(0);
    }

    public void DealDamage(float value, AttackDefines.AttackType damage)
    {
        lastDamage.CalcAttack(damage, value);
    }
    public virtual void ResolveDamate()
    {
        DealDamage(lastDamage);
    }
    public virtual void DealDamage(DamageTable damage)
    {
        lastDamage = damage;
        if (damage.resolved) return;
        damage.Calculate();
        foreach (var d in damage.realDamage)
        {
            Combat.main.Inspect($"{parent} recevies {d.Value} {d.Key} ({damage.blockType}) total damage from {damage.attacker} at t{Combat.main.currentTick}");
    
                float realDamage = d.Value;
            switch (d.Key)
            {
                case AttackDefines.AttackType.Slashing:
                case AttackDefines.AttackType.Piercing:
                case AttackDefines.AttackType.Crushing:
                case AttackDefines.AttackType.Magical:
                case AttackDefines.AttackType.Poison:
                case AttackDefines.AttackType.Pure:

                    UpdateKiller(damage.attacker);
                    TakeDirectDamage(realDamage);
                    if (d.Key == AttackDefines.AttackType.Slashing
                        || d.Key == AttackDefines.AttackType.Crushing
                        || d.Key == AttackDefines.AttackType.Piercing)
                        Vampirism(realDamage);

                    parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                    break;
                case AttackDefines.AttackType.ShieldHeal:
                    Block.GiveValue(realDamage);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnShieldReceived);
                    break;
                case AttackDefines.AttackType.ArmorHeal:
                    Armor.GiveValue(realDamage);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnArmorReceived);
                    break;
                case AttackDefines.AttackType.ArmorBreak:
                    TakeShieldDamage(false, realDamage, out float guardblock); //temp shield
                    realDamage -= guardblock;
                    TakeShieldDamage(true, realDamage, out float armorblock);    //armor
                    realDamage -= armorblock;
                    break;
                case AttackDefines.AttackType.Assassinate:
                    if (Health.GetValue() <= realDamage)
                        Kill(damage.attacker);
                    break;
                case AttackDefines.AttackType.LifeHealNoOverheal:
                case AttackDefines.AttackType.LifeHealOverhealShield:
                case AttackDefines.AttackType.LifeHealOverhealArmor:
                    float healValue = Mathf.Min(realDamage, Health.GetDifference());
                    float overheal = realDamage - healValue;

                    Health.GiveValue(healValue);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnHealReceived);

                    if (overheal > 0)
                    {
                        if (d.Key == AttackDefines.AttackType.LifeHealOverhealShield)
                        {
                            Block.GiveValue(overheal);
                            parent.FireEventOnSelf(AbilityDefines.Event.OnShieldReceived);
                        }
                        if (d.Key == AttackDefines.AttackType.LifeHealOverhealArmor)
                        {
                            Armor.GiveValue(overheal);
                            parent.FireEventOnSelf(AbilityDefines.Event.OnArmorReceived);
                        }
                    }
                    break;
                case AttackDefines.AttackType.GrantAP:
                    parent.actions.ActionPoint.GiveValue(realDamage);
                    break;
                case AttackDefines.AttackType.GrantRP:
                    parent.actions.ReactionPoints.GiveValue(realDamage);
                    break;
                case AttackDefines.AttackType.GrantSP:
                    parent.actions.SupplyPoints.GiveValue(realDamage);
                    break;
                case AttackDefines.AttackType.Guard:
                    damage.target.damageable.ApplyGuardian(parent) ;
                    break;
            }
        }
        damage.resolved = true;
    }
    void Vampirism(float damage)
    {
        if (damage > 0)
        {
            float healVamp = parent.GetProperty(ModifierDefines.Property.vampirism_constant) + (parent.GetProperty(ModifierDefines.Property.vampirism_percent) - 1) * damage;
            Heal(healVamp);
        }
    }
    float TakeDirectDamage(float damage)
    {
        float guardblock = 0;
        float armorblock = 0;
        /*if (damage.dmt == AttackDefines.DamageType.Poison)
        {
            resultDamage = Mathf.Clamp(resultDamage, 0, Health.GetValue() - 1);
        }
        else*/
        {
            TakeShieldDamage(false, damage, out guardblock); //temp shield
            damage -= guardblock;

            TakeShieldDamage(true, damage, out armorblock);    //armor
            damage -= armorblock;
        }
        TakeLifeDamage(damage, out float finaldamage);
        return finaldamage;
    }
    public static float AccountResistances(float damage, float resistance)
    {
        float arMult = AttackDefines.ArmorMitigation / (AttackDefines.ArmorMitigation + resistance);
        return damage * arMult;
    }
    void TakeShieldDamage(bool armor, float damagevalue, out float blockeddamage)
    {
        blockeddamage = 0;
        if (damagevalue == 0)
            return;

        Resource defense = armor ? Armor : Block;
        float shValue = defense.GetValue();
        if (shValue > 0)
        {
            float efficiency = 1;
            if (shValue > damagevalue * efficiency)
            {
                blockeddamage = defense.SubstractedValue(damagevalue * efficiency);
                parent.FireEventOnSelf(armor ? AbilityDefines.Event.OnArmorBlock : AbilityDefines.Event.OnShieldBlock);
            }
            else
            {
                blockeddamage = shValue / efficiency;
                defense.SetValue(0);
                parent.FireEventOnSelf(armor ? AbilityDefines.Event.OnArmorBreak : AbilityDefines.Event.OnShieldBreak);
            }
        }

    }
    void TakeLifeDamage(float damagevalue, out float Receivedamage)
    {
        Receivedamage = 0;
        if (damagevalue == 0)
            return;

        Receivedamage = Health.SubstractedValue(damagevalue);
        parent.FireEventOnSelf(AbilityDefines.Event.OnTakeLifeDamage);
        parent.FireEventOnSelf(AbilityDefines.Event.OnLifeChange);

        Combat.main.Inspect($"{parent} takes {Receivedamage} damage!");
        CheckDeath();
    }
    public float Heal(float amount)
    {
        float overheal = Mathf.Max(0, amount - Health.GetValue());
        Health.GiveValue(amount);
        CheckDeath();
        parent.FireEventOnSelf(AbilityDefines.Event.OnHealReceived);
        parent.FireEventOnSelf(AbilityDefines.Event.OnLifeChange);
        return overheal;
    }
    public void FullHeal(bool health = true, bool armor = true)
    {
        if (health) Health.SetPercentage(1);
        if (armor) Armor.SetPercentage(1);
        CheckDeath();
    }
    public void RegenerateHealth()
    {
        if (IsAlive())
        {
            Heal(parent.GetProperty(ModifierDefines.Property.health_regen_bonus) * parent.GetProperty(ModifierDefines.Property.health_regen_percentage) + parent.GetProperty(ModifierDefines.Property.total) * Health.GetLimit());
            CheckDeath();
        }
        else
        {
            Revive();
        }
    }
    public void CheckDeath()
    {
        if (!IsAlive())
        {
            Kill();
        }
        else
        {
            Revive(false);
        }
    }
    public void Kill(DataItemUnit k = null)
    {
        UpdateKiller(k);
        if (!dead)
            Die();
    }
    public void Revive(bool fullHeal = true)
    {
        dead = false;
        ClearKiller();
        if (fullHeal)
            FullHeal();
    }
    protected virtual void Die()
    {
        if (IsAlive() || !dead)
        {
            dead = true;

            Health.SetPercentage(0);
            Armor.SetPercentage(0);
            Block.SetPercentage(0);

            Combat.main.Inspect(parent + " has died!");
            parent.FireEventOnTarget(AbilityDefines.Event.OnKilled, killer, true);
            if (killer != null)
            {
                killer.FireEventOnTarget(AbilityDefines.Event.OnScoreKill, parent, true);
            }
        }
    }

    #region Killing Entity
    public DataItemUnit killer;

    void UpdateKiller(DataItemUnit killer)
    {
        if (killer != null && killer != parent)
            this.killer = killer;
    }
    void ClearKiller()
    {
        killer = null;
    }
    #endregion

    public virtual bool IsAlive()
    {
        return !dead && Health.GetValue() > 0;
    }
    bool CanAcceptGuardian(DataItemUnit guard)
    {
        return guard.damageable.IsAlive() && guard.GetAlignment(parent) == PlayerDefines.Alignment.ally && guard.IsInCombat() && !guard.GetState( ModifierDefines.State.cannot_guard);
    }
    public void ApplyGuardian(DataItemUnit guard)
    {
        if (CanAcceptGuardian(guard))
            guardian = guard;
    }
    void ClearGuardian()
    {
        guardian = null;
    }
}
