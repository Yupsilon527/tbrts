
using UnityEngine;

public class UnitDamageable : UnitComponent
{
    public ResourceFloat Health;
    public ResourceFloat Armor;
    public ResourceFloat Block;

    public DamageTable lastDamage;
    protected bool dead = false;
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        base.TriggerFuncs(act);
        if (act == AbilityDefines.Event.OnSpawn)
        {
            Health.SetPercentage(1);
            ClearKiller();
        }
        if (act == AbilityDefines.Event.CombatBegin)
        {
            Armor.SetPercentage(1);
            Block.SetPercentage(0);
        }
    }
    public UnitDamageable(DataItemUnit owner) : base(owner)
    {
        Health = new ResourceFloat(1, "Health", false, false);
        Health.LimitUnder = Resource.LimitRule.percent_value;
        Health.LimitOver = Resource.LimitRule.percent_value;

        Armor = new ResourceFloat(1, "Armor", false, true);
        Armor.LimitOver = Resource.LimitRule.leave_value;
        Armor.LimitUnder = Resource.LimitRule.leave_value;

        Block = new ResourceFloat(1, "Block", false, false);
        Block.SetValue(0);
    }

    public void DealDamage(float value, AttackDefines.ActionType damage, DataItemUnit attacker,  AttackDefines.HitType block = AttackDefines.HitType.normal)
    {
        var dmt = new DamageTable(attacker == null ? parent : attacker, parent, value, damage, block);
        DealDamage(dmt, false);
    }
    public virtual void DealDamage(DamageTable damage, bool bonusDamage = false)
    {

        lastDamage = damage;
        damage.Calculate();
        if (bonusDamage) damage.AccountBonusDamage();

        switch (damage.dmt)
        {
            case AttackDefines.ActionType.DirectDamage:
                float outDamage = damage.realDamage;
                UpdateKiller(damage.attacker);
                float healthDamage = TakeDirectDamage(outDamage);
                Vampirism(healthDamage);
                parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                break;
            case AttackDefines.ActionType.Assassinate:
                if (Health.GetValue() <= damage.realDamage)
                    Kill(damage.attacker);
                break;
            case AttackDefines.ActionType.NonLethalIgnoreArmorDamage:
                float magicDamage = Mathf.Min(Health.GetValue() - 1, damage.realDamage);
                if (magicDamage > 0)
                {
                    TakeLifeDamage(magicDamage, out float resulting);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                }
                break;
            case AttackDefines.ActionType.NonLethalDamage:
                float nonLethalDamage = Mathf.Min(Health.GetValue() + Armor.GetValue() + Block.GetValue() - 1, damage.realDamage);
                if (nonLethalDamage > 0)
                {
                    TakeLifeDamage(nonLethalDamage, out float resulting);
                    parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                }
                break;
            case AttackDefines.ActionType.ArmorBreak:
                float armorDamage = damage.realDamage;
                TakeShieldDamage(true, armorDamage, out outDamage);
                parent.FireEventOnSelf(AbilityDefines.Event.OnTakeDamage);
                break;
            case AttackDefines.ActionType.LifeHealNoOverheal:
            case AttackDefines.ActionType.LifeHealOverhealShield:
            case AttackDefines.ActionType.LifeHealOverhealArmor:
                float overheal = Heal(damage.realDamage);
                if (overheal > 0)
                {
                    if (damage.dmt == AttackDefines.ActionType.LifeHealOverhealShield)
                    {
                        GiveShield(overheal);
                    }
                    if (damage.dmt == AttackDefines.ActionType.LifeHealOverhealArmor)
                    {
                        GiveArmor(overheal);
                    }
                }
                break;
            case AttackDefines.ActionType.ArmorHeal:
                GiveArmor(damage.realDamage);
                break;
            case AttackDefines.ActionType.Block:
                GiveShield(damage.realDamage);
                break;
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
    void GiveArmor(float amt)
    {
        Armor.GiveValue(amt);
        parent.FireEventOnSelf(AbilityDefines.Event.OnArmorRecieved);
    }
    void GiveShield(float amt)
    {
        Block.GiveValue(amt);
        parent.FireEventOnSelf(AbilityDefines.Event.OnShieldRecieved);
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
