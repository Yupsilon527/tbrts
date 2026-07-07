
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitStatsTable 
{
    [Header("Combat")]
    public float Offense = 5;
    public float Defense = 5;
    public float Luck = 0;

    [Header("Attack")]
    public float Attack = 5;
    public float Magic = 0;
    public float Speed = 100;

    [Header("Resources")]
    public float Action = 1;
    public float Mana = 1;
    public float Supply = 1;

    [Header("Health/Barrier")]
    public float Health = 25;
    public float Barrier = 4;

    [Header("Armor/Block")]
    public float Armor = 0; //flat damage reduction against all physical sources
    public float Shield = 0; 
    public float Padding = 0; 
    public float Resistance = 0; //flat damage reduction against all elemental sources
    public float Block = 0; //damage block, reduces damage when blocking

    public float GetLuckCoefficient()
    {
        if (Luck > 0)
            return (Luck + AttackDefines.attackCoefficient) / Luck;
        else
            return Mathf.Abs(Luck / (AttackDefines.attackCoefficient + Luck));
    }
    public float GetSpeedMultiplier()
    {
        float speed = Speed - 100;
        if (speed == 0)
            return 1;
        else if (speed > 0)
            return (speed) / (speed + 300);
        return 1 - (speed / 200);
    }
    public float LuckCoefficient = 1;
    public float SpeedCoefficient = 1;


    public float DodgeChance = 0;
    public float BlockChance = 0;
    public float ProcChance = 0;
    public float CritChance = 0;

    public BonusDamageTable[] bonusDamage;
    public AttackDefines.MobFlag[] GetCounters()
    {
        HashSet<AttackDefines.MobFlag> counterFlags = new();
        foreach (var counter in bonusDamage)
        {
            if (counter.damage > 0)
                counterFlags.Add(counter.flag);
        }
        return counterFlags.ToArray();
    }

    public UnitStatsTable() { }
    public UnitStatsTable Clone()
    {
        return (UnitStatsTable)MemberwiseClone();
    }


    private string FormatValue(float value)
    {
        return (Mathf.Round(value * 100) / 100).ToString();
    }

    private string FormatPercent(float value)
    {
        return $"{Mathf.Round(value * 100)}%";
    }

    public float GetPowerValue(AbilityData[] abilities = null)
    {
        return 100;
    }

    public float GetPowerValue(Dictionary<UnitDefines.ArmyAbilities, int> abilities = null)
    {
        return 100;
    }
}
[Serializable]
public class StatAlteration
{
    public enum AlterationType
    {
        Addition,
        Multiply,
        Divide,
        Min,
        Max,
    }
    public enum StatType
    {
        offense,
        defense,

        attack,
        magic,
        ctrl,

        health,
        barrier,

        armor,
        resistance,
        block,

        speed,
        luck,
    }
    public bool absolute = false;
    public StatType statType = StatType.attack;
    public AlterationType behavior = AlterationType.Addition;
    public float value = 0;
    public void ApplyToStat(ref float stats, float value)
    {
        switch (behavior)
        {
            case AlterationType.Addition:
                stats += value;
                break;
            case AlterationType.Multiply:
                stats *= Mathf.Abs(value);
                break;
            case AlterationType.Divide:
                stats /= Mathf.Abs(value);
                break;
            case AlterationType.Min:
                stats = Mathf.Min(stats, value);
                break;
            case AlterationType.Max:
                stats = Mathf.Max(stats, value);
                break;

        }
    }
    public void Apply(DataItemUnit table)
    {
        switch (statType)
        {
            case StatType.offense:
                ApplyToStat(ref table.stats.baseStats.Offense, GetEffectiveChange(table));
                break;
            case StatType.defense:
                ApplyToStat(ref table.stats.baseStats.Defense, GetEffectiveChange(table));
                break;
            case StatType.magic:
                ApplyToStat(ref table.stats.baseStats.Magic, GetEffectiveChange(table));
                break;
            case StatType.ctrl:
                ApplyToStat(ref table.stats.baseStats.Action, GetEffectiveChange(table));
                break;
            case StatType.health:
                ApplyToStat(ref table.stats.baseStats.Health, GetEffectiveChange(table));
                break;
            case StatType.barrier:
                ApplyToStat(ref table.stats.baseStats.Barrier, GetEffectiveChange(table));
                break;
            case StatType.armor:
                ApplyToStat(ref table.stats.baseStats.Armor, GetEffectiveChange(table));
                break;
            case StatType.resistance:
                ApplyToStat(ref table.stats.baseStats.Resistance, GetEffectiveChange(table));
                break;
            case StatType.block:
                ApplyToStat(ref table.stats.baseStats.Block, GetEffectiveChange(table));
                break;
            case StatType.speed:
                ApplyToStat(ref table.stats.baseStats.Speed, GetEffectiveChange(table));
                break;
            case StatType.luck:
                ApplyToStat(ref table.stats.baseStats.Luck, GetEffectiveChange(table));
                break;
        }
    }

    /*public ModifierDefines.Properties GetRelatedProperty()
    {
        switch (statType)
        {
            case StatType.offense:
                return ModifierDefines.Properties.offense_incoming_from_bonus;
            case StatType.defense:
                return ModifierDefines.Properties.defense_incoming_from_bonus;
            case StatType.magic:
                return ModifierDefines.Properties.magic_incoming_from_bonus;
            case StatType.ctrl:
                return ModifierDefines.Properties.control_bonus_percent;
            case StatType.health:
                return ModifierDefines.Properties.health_bonus_percent;
            case StatType.barrier:
                return ModifierDefines.Properties.barrier_incoming_from_bonus;
            case StatType.armor:
                return ModifierDefines.Properties.armor_incoming_from_bonus;
            case StatType.resistance:
                return ModifierDefines.Properties.resistance_incoming_from_bonus;
            case StatType.block:
                return ModifierDefines.Properties.block_incoming_from_bonus;
            case StatType.speed:
                return ModifierDefines.Properties.speed_incoming_from_bonus;
            case StatType.luck:
                return ModifierDefines.Properties.luck_incoming_from_bonus;
        }
        return ModifierDefines.Properties.total;
    }*/
    public float GetEffectiveChange(DataItemUnit player)
    {
     //   if (!absolute && GetRelatedProperty() < ModifierDefines.Properties.total)
     //       return value * player.GetPropertyMultiplicative(GetRelatedProperty());
        return value;
    }
}
