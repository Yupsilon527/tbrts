using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackDefines
{
    public enum DamageFlag
    {
        Melee = 0,
        Range = 1,
        Area = 2,
        Indirect = 3,
        Powerup = 4
    }
    public enum DamageType
    {
        Physical = 0,
        Magical =1,
        LifeHeal = 2,
        ShieldHeal = 3,
    }
    public enum AttackType
    {
        Physical = 0 ,
        Magical = 1,
        Heal = 2,
        Shield = 3,
        Taunt = 4,

    }
    public static float ArmorMitigation = 50;

    public static string DefaultEffectsDirectory = "GameData/Powers/Effects/DefaultEffects";
}
