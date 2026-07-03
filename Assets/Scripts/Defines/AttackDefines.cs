using UnityEngine;

public static class AttackDefines
{
    public enum HitType
    {
        ignoreArmor,
        blockCrit,
        normal,
        blocked,
        criticalHit,
        miss,
    }
    public enum BonusType
    {
        nothing = -1,
        additive = 0,
        multiplicative = 1,
    }
    public enum DamageType
    {
        Slashing = 0,
        Piercing = 1,
        Crushing = 2,
        Magical = 3,
        Pure = 4,
        Poison = 5,
        LifeHealNoOverheal = 6,
        ShieldHeal = 7,
        ArmorHeal = 8,
        ArmorBreak = 9,

        LifeHealOverhealShield = 10,
        LifeHealOverhealArmor = 11,
        Assassinate = 12,
        
        Stun = 13,

        Total = 14,
    }
    public enum AttackType
    {
        Physical = 0,
        Piercing = 1,
        Crushing = 2,
        Magical = 3,
        Pure = 4,
        Poison = 5,
        Heal = 6,
        Shield = 7,
        Taunt = 8,

    }
    public enum MobFlag
    {
        civilian,
        Spearman,
        Infantry,
        Skirmisher,
        Archer,
        Cavalry,
        Knight,
        SiegeEngine,
        Building,
        UniqueUnit,
        FootUnit,
        RangedUnit,
        MountedUnit,
        FlyingUnit,
        SupportUnit,
    }
    public enum ScaleType
    {
        Nothing = 0,

       

        Random = 1,
        BonusDamage = 2,
        Padding = 3,
        Shield = 4,
        Armor = 5,
        HealthBar = 6,
        Magic = 7,
        Attack = 8,
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
                case ScaleType.Attack:
                    bonusDamage *= owner.stats.realStats.Attack;
                    break;
                case ScaleType.Magic:
                    bonusDamage *= owner.stats.realStats.Magic;
                    break;
                case ScaleType.HealthBar:
                    bonusDamage *= owner.damageable.Health.GetValue();
                    break;
                case ScaleType.Armor:
                    bonusDamage *= owner.damageable.Health.GetValue();
                    break;
                case ScaleType.Shield:
                    bonusDamage *= owner.damageable.Health.GetValue();
                    break;
                case ScaleType.Padding:
                    bonusDamage *= owner.damageable.Health.GetValue();
                    break;
                case ScaleType.BonusDamage:
                    bonusDamage =  (owner.bonuses.CalculateDamageAgainstTarget(target, baseDamage)- baseDamage)*owner.GetPropertyMultiplicative( ModifierDefines.Property.outgoing_bonus_damage);
                    return bonusDamage + baseDamage;
                default:
                    return baseDamage * Random.value;
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
    internal static float minAccuracy = .2f;
}
