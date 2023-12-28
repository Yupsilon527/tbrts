using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStatsComponent : MobComponent
{
    //attack
    [HideInInspector]    public float RealAttackDamage = 1;

    //health
    [HideInInspector] public float RealHealth = 1;
    [HideInInspector] public float RealHealthRegen = 0;
    [HideInInspector] public float RealArmor = 0;
    [HideInInspector] public float RealResistance = 0;

    //Magic
    [HideInInspector] public float RealSpecialDamage = 1;
    //[HideInInspector] public float RealMana = 1;
    //[HideInInspector] public float RealManaRegen = 1;

    [HideInInspector] public float RealMovementSpeed = 1;
    public virtual void Recalculate()
    {
        UpdateMaxHealth();
        //UpdateMana();
        UpdateHealthRegen();
        //UpdateManaRegen();
        UpdateAttackDamage();
        UpdateMagicDamage();
        UpdateArmor();
        UpdateResistance();
        UpdateMoveSpeed();
    }
    #region Attack
    public float BaseAttackDamage = 1;
    public virtual void UpdateAttackDamage()
    {
        RealAttackDamage = BaseAttackDamage;
        if (parent.modifiers != null)
        {
            RealAttackDamage += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.base_attack_bonus);
            RealAttackDamage *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.base_attack_mult);
            RealAttackDamage += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_attack_bonus);
        }
        RealAttackDamage = Mathf.Max(1, RealAttackDamage);
    }
    #endregion
    #region Magic
    public float BaseSpecialDamage = 1;
    public float BaseMana = 5;
    public float BaseManaRegen = 0;
    public virtual void UpdateMagicDamage()
    {
        RealSpecialDamage = BaseSpecialDamage;
        if (parent.modifiers != null)
        {
            RealSpecialDamage += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.base_magic_bonus);
            RealSpecialDamage *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.base_magic_mult);
            RealSpecialDamage += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_magic_bonus);
        }
        RealSpecialDamage = Mathf.Max(1, RealSpecialDamage);
    }
  /*  public virtual void UpdateMana()
    {
        RealMana = BaseMana;
        if (parent.modifiers != null)
        {
            RealMana *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.base_mana_mult);
            RealMana += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_mana);
        }
        RealMana = Mathf.Max(1, RealMana);
    }
    public virtual void UpdateManaRegen()
    {
        RealManaRegen = BaseHealthRegen;
        if (parent.modifiers != null)
        {
            RealManaRegen *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.mana_regen_percentage);
            RealManaRegen += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.mana_regen_constant);
            RealManaRegen *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.mana_regen_percentage_total);
        }
        float fraction = parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.mana_regen_fraction);
        if (fraction > 0)
        {
            RealManaRegen += RealMana * fraction;
        }
        RealManaRegen = Mathf.Max(1, RealManaRegen);
    }*/
    #endregion
    #region Endurance
    public float BaseHealth = 10;
    public float BaseHealthRegen = 0;
    public float BaseArmor = 0;
    public float BaseResistance = 0;
    public virtual void UpdateMaxHealth()
    {
        RealHealth = BaseHealth;
        if (parent.modifiers != null)
        {
            RealHealth *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.base_health_mult);
            RealHealth += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_health);
        }
        RealHealth = Mathf.Max(1, RealHealth);
        if (parent.damageable!= null)
        {
            parent.damageable.Health.SetLimit(RealHealth);
        }
    }
    public virtual void UpdateHealthRegen()
    {
        RealHealthRegen = BaseHealthRegen;
        if (parent.modifiers != null)
        {
            RealHealthRegen *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.laif_regen_multiplier);
            RealHealthRegen += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.laif_regen_constant);
            RealHealthRegen *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.laif_regen_percentage);
        }
        float fraction = parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.laif_regen_fraction);
        if (fraction > 0)
        {
            RealHealthRegen += RealHealth * fraction;
        }
        RealHealthRegen = Mathf.Max(0, RealHealthRegen);
    }
    public virtual void UpdateArmor()
    {
        RealArmor = BaseArmor;
        if (parent.modifiers != null)
        {
            RealArmor *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.bonus_armor_percentage);
            RealArmor += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_armor_constant);
        }
        RealArmor = Mathf.Max(1, RealArmor);
    }

    public virtual void UpdateResistance()
    {
        RealResistance = BaseResistance;
        if (parent.modifiers != null)
        {
            RealResistance *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.bonus_resist_percentage);
            RealResistance += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.bonus_resist_constant);
        }
        RealResistance = Mathf.Max(1, RealResistance);
    }
    #endregion
    #region Movement Speed
    public float BaseMovementSpeed = 1;
    public virtual void UpdateMoveSpeed()
    {
        RealMovementSpeed = BaseMovementSpeed;
        if (parent.modifiers != null)
        {
            RealMovementSpeed *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.move_speed);
            RealMovementSpeed += parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.move_speed_mult);
        }

        float totaloverride = parent.modifiers.GetPropertyAdditive(ModifierDefines.modProps.move_speed_override);
        if (totaloverride > 0)
        {
            RealMovementSpeed = totaloverride;
        }
        else
        {
            RealMovementSpeed = Mathf.Max(1, RealMovementSpeed);
        }
        if (parent.movement != null)
        {
            parent.movement.MovementSpeed = RealMovementSpeed;
        }
    }
    #endregion

    public virtual void LoadStatsTable(StatsTableSO stats)
    {
        if (stats is null)
        {
            return;
        }
        parent.archetype = stats.HeroArchetype;

        BaseAttackDamage = stats.AttackDamage;
        BaseHealth = stats.Health;
        BaseHealthRegen = stats.HealthRegen;
        BaseArmor = stats.Armor;
        BaseResistance = stats.Resistance;
        BaseSpecialDamage = stats.SpecialDamage;
        BaseMovementSpeed = stats.MovementSpeed;
    }
    public float GetCombatValue()
    {
        return 100;
    }
}
