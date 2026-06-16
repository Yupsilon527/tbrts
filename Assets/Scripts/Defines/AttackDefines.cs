
using UnityEngine;

public static class AttackDefines
{
    public enum HitType
    {
        ignoreArmor,
        halfBlock,
        normal,
        blocked,
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
}
