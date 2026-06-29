
using UnityEngine;

public class UnitDamageable : UnitComponent
{
    public ResourceInt Health, Armor, Block;

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
        }
        if (act == AbilityDefines.Event.CombatEnd)
        {
            Armor.SetPercentage(0);
            Block.SetPercentage(0);
        }
    }
    public UnitDamageable(DataItemUnit owner) : base(owner)
    {
        Health = new ResourceInt(1, "Health", false, false);
        Health.LimitUnder = Resource.LimitRule.percent_value;
        Health.LimitOver = Resource.LimitRule.percent_value;

        Armor = new ResourceInt(1, "Armor", false, true);
        Armor.LimitOver = Resource.LimitRule.leave_value;
        Armor.LimitUnder = Resource.LimitRule.leave_value;

        Block = new ResourceInt(1, "Block", false, false);
        Block.SetValue(0);
    }

    public void DealDamage(float value, AttackDefines.DamageType damage)
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
        damage.Calculate();

        foreach (var d in damage.realDamage)
        {
            float realDamage = d.Value;
            switch (d.Key)
            {
                case AttackDefines.DamageType.Slashing:
                case AttackDefines.DamageType.Piercing:
                case AttackDefines.DamageType.Crushing:
                case AttackDefines.DamageType.Magical:
                case AttackDefines.DamageType.Poison:
                case AttackDefines.DamageType.Pure:

                    UpdateKiller(damage.attacker);
                    TakeDirectDamage(realDamage);
                    if (d.Key == AttackDefines.DamageType.Slashing
                        || d.Key == AttackDefines.DamageType.Crushing
                        || d.Key == AttackDefines.DamageType.Piercing)
                        Vampirism(realDamage);

                    parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                    break;
                case AttackDefines.DamageType.ShieldHeal:
                    Block.GiveValue(realDamage);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnShieldRecieved);
                    break;
                case AttackDefines.DamageType.ArmorHeal:
                    Armor.GiveValue(realDamage);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnArmorRecieved);
                    break;
                case AttackDefines.DamageType.ArmorBreak:
                    TakeShieldDamage(false, realDamage, out float guardblock); //temp shield
                    realDamage -= guardblock;
                    TakeShieldDamage(true, realDamage, out float armorblock);    //armor
                    realDamage -= armorblock;
                    break;
                case AttackDefines.DamageType.Assassinate:
                    if (Health.GetValue() <= realDamage)
                        Kill(damage.attacker);
                    break;
                case AttackDefines.DamageType.LifeHealNoOverheal:
                case AttackDefines.DamageType.LifeHealOverhealShield:
                case AttackDefines.DamageType.LifeHealOverhealArmor:
                    float healValue = Mathf.Min(realDamage, Health.GetDifference());
                    float overheal = realDamage - healValue;

                    Health.GiveValue(healValue);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnHealRecieved);

                    if (overheal > 0)
                    {
                        if (d.Key == AttackDefines.DamageType.LifeHealOverhealShield)
                        {
                            Block.GiveValue(overheal);
                            parent.FireEventOnSelf(AbilityDefines.Event.OnShieldRecieved);
                        }
                        if (d.Key == AttackDefines.DamageType.LifeHealOverhealArmor)
                        {
                            Armor.GiveValue(overheal);
                            parent.FireEventOnSelf(AbilityDefines.Event.OnArmorRecieved);
                        }
                    }
                    break;
            }
        }

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
    void TakeLifeDamage(float damagevalue, out float recievedamage)
    {
        recievedamage = 0;
        if (damagevalue == 0)
            return;

        recievedamage = Health.SubstractedValue(damagevalue);
        parent.FireEventOnSelf(AbilityDefines.Event.OnTakeLifeDamage);
        parent.FireEventOnSelf(AbilityDefines.Event.OnLifeChange);

        Combat.main.Inspect($"{parent} takes {recievedamage} damage!");
        CheckDeath();
    }
    public float Heal(float amount)
    {
        float overheal = Mathf.Max(0, amount - Health.GetValue());
        Health.GiveValue(amount);
        CheckDeath();
        parent.FireEventOnSelf(AbilityDefines.Event.OnHealRecieved);
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
}
