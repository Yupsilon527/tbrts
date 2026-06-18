
using UnityEngine;

public static class AttackDefines
{
    public enum HitType
    {
        ignoreArmor,
        halfBlock,
        normal,
        blocked,
        criticalHit,
        miss,
    }
    public enum ActionType
    {
        DirectDamage = 0,
        Block = 1,
        LifeHealNoOverheal = 2,
        ArmorBreak = 3,
        ArmorHeal = 4,
        NonLethalIgnoreArmorDamage = 5,
        LifeHealOverhealShield = 6,
        LifeHealOverhealArmor = 7,
        NonLethalDamage = 8,
        Assassinate = 9,
        Total = 10,
    }
    public enum ScaleType
    {
        Nothing = 0,

        //Stats
        AttackStat = 1,
        MagicStat = 2,
        ControlStat = 3,

        //Cur Props
        CurGold = 4,
        CurHp = 5,
        MissHp = 6,
        TotHp = 7,
        HealthBar = 8,
        CurArmor = 9,
        CurBlock = 10,

        Random = 11,
    }
    public enum ScaleMode
    {
        caster,
        target
    }
    public enum ScaleRate
    {
        additive,
        multiplicative,
    }
    public static float GetScaleStrength(DataItemUnit attacker, DataItemUnit target, ScaleMode scaleTarget, float baseDamage, ScaleType scale, ScaleRate rate, float scaleDamage)
    {
        if (scale == ScaleType.Nothing) return baseDamage;
        float bonusDamage = scaleDamage;
        var owner = scaleTarget == ScaleMode.target ? target : attacker;
        var victim = scaleTarget == ScaleMode.target ? attacker : target;
        if (owner != null)
        {
            switch (scale)
            {
                case ScaleType.AttackStat:
                    bonusDamage *= owner.stats.realStats.Attack;
                    break;
                /*case ScaleType.AttackCrit:
                    bonusDamage = scaleDamage + owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.critical_damage);
                    break;
                case ScaleType.ItemLevel:
                    bonusDamage *= (roll == null || roll.sourceItem == null) ? 0 : roll.sourceItem.GetLevel();
                    break;
                case ScaleType.CurBlock:
                    bonusDamage *= owner.damageable.Block.GetValue();
                    break;
                case ScaleType.SpeedStat:
                    bonusDamage *= owner.stats.realStats.Speed;
                    break;
                case ScaleType.EmptySlots:
                    if (owner is Hero attackingHero)
                        bonusDamage *= attackingHero.inventory.GetEmptyInventorySlots();
                    break;
                case ScaleType.UnarmedBonus:
                    bonusDamage *= owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.unarmed_bonus);
                    break;
                case ScaleType.AttackBonus: //Accounts bonus attack only
                    bonusDamage *= owner.stats.realStats.AttackDamage - owner.stats.baseStats.AttackDamage;
                    break;
                case ScaleType.BlockBonus:
                    bonusDamage *= owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.block_bonus);
                    break;
                case ScaleType.ArmorBonus:
                case ScaleType.ArmorBonusRaw:
                    bonusDamage *= owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.armor_bonus);
                    if (scale == ScaleType.ArmorBonus)
                        bonusDamage *= owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.incoming_armor_from_bonus);
                    break;
                case ScaleType.Random:
                    bonusDamage *= Random.value;
                    break;
                case ScaleType.TotHp:
                    bonusDamage *= owner.stats.realStats.Health;
                    break;
                case ScaleType.TotHpArmor:
                    bonusDamage *= owner.stats.realStats.Health + owner.damageable.ConstantArmor.GetValue() + owner.damageable.PersistentArmor.GetValue();
                    break;
                case ScaleType.MissHp:
                    bonusDamage *= owner.damageable.Health.GetDifference();
                    break;
                case ScaleType.CurHp:
                    bonusDamage *= owner.damageable.Health.GetValue();
                    break;
                case ScaleType.MissHpPercent:
                    bonusDamage *= 1 - owner.damageable.Health.GetPercentage();
                    break;
                case ScaleType.CurHpPercent:
                    bonusDamage *= owner.damageable.Health.GetPercentage();
                    break;
                case ScaleType.RollNumberCanCrit:
                case ScaleType.RollNumber:
                    bonusDamage *= roll.rollAmount;
                    if (roll.isDiceCrit && scale == ScaleType.RollNumberCanCrit)
                        bonusDamage *= 2;
                    break;
                case ScaleType.Level:
                    bonusDamage *= owner.level;
                    break;
                case ScaleType.Poison:
                    if (victim.modifiers.GetState(ModifierDefines.State.poison_immune))
                        return 0;
                    return Mathf.Max(1, baseDamage + owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.poison_damage_bonus)) * owner.modifiers.GetPropertyMultiplicative(ModifierDefines.Property.poison_damage_incoming);
                case ScaleType.Fire:
                    if (victim.modifiers.GetState(ModifierDefines.State.fire_immune))
                        return 0;
                    return Mathf.Max(1, baseDamage + owner.modifiers.GetPropertyAdditive(ModifierDefines.Property.fire_damage_bonus)) * owner.modifiers.GetPropertyMultiplicative(ModifierDefines.Property.fire_damage_incoming);*/
                default:
                    return baseDamage;
            }
        }
        switch (rate)
        {
            case ScaleRate.multiplicative:
                return bonusDamage * baseDamage;
            default:
                return bonusDamage + baseDamage;
        }
    }
    public static float ArmorMitigation = 50;

    public static string DefaultEffectsDirectory = "GameData/Powers/Effects/DefaultEffects";
    public static float HeroUpgradePriceBase = 100;
    public static float HeroUpgradePriceIncrement = 1.15f;
    public static float GetValueAtLevel(float baseValue, float levelIncrement, int level)
    {
        if (levelIncrement > 1)
        {
            return baseValue * Mathf.Pow(levelIncrement, level);
        }
        if (levelIncrement < 0)
            return 0;
        return baseValue;
    }
    public static float attackCoefficient = 100;
    internal static float minAccuracy;
}
