using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsTableSO : ScriptableObject
{
    public MobDefines.Archetype HeroArchetype;
    public string InternalName = "MISSING";
    [Header("Stats")]
    public float AttackDamage = 5;

    public float Health = 25;
    public float HealthRegen = 0;
    public float Armor = 0;
    public float Resistance = 0;

    public float SpecialDamage = 5;

    public float MovementSpeed = 5;
    public float GetCombatValue()
    {
        return 100;
    }

    [Header("Abilities")]
    public AbilitySO[] Abilities;
}
