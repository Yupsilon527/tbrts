
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitStatsTable 
{
    //combat
    public float Offense = 5;
    public float Defense = 5;

    //attack
    public float Attack = 5;
    public float Action = 1;
    //magic
    public float Magic = 5;
    public float Mana = 1;

    //healthbars
    public float Health = 25;
    public float Barrier = 4;

    //damage mitigation
    public float Armor = 0; //flat damage reduction against all physical sources
    public float Resistance = 0; //flat damage reduction against all elemental sources
    public float Block = 0; //damage block, reduces damage when blocking

    //modifiers
    public float Speed = 100;
    public float Luck = 0;
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
    public string OutputTable()
    {
        string output = "";
        // Combat
        output += "<b>Combat</b><br>";
        output += $"Offense: {FormatValue(Offense)}<br>";
        output += $"Defense: {FormatValue(Defense)}<br>";

        // Attack
        output += "<br><b>Attack</b><br>";
        output += $"Attack: {FormatValue(Attack)}<br>";
        output += $"Magic: {FormatValue(Magic)}<br>";
        output += $"Control: {FormatValue(Action)}<br>";

        // Health
        output += "<br><b>Health</b><br>";
        output += $"Health: {FormatValue(Health)}<br>";
        output += $"Barrier: {FormatValue(Barrier)}<br>";

        // Mitigation
        output += "<br><b>Mitigation</b><br>";
        output += $"Armor: {FormatValue(Armor)}<br>";
        output += $"Resistance: {FormatValue(Resistance)}<br>";
        output += $"Block: {FormatValue(Block)}<br>";

        // Modifiers
        output += "<br><b>Modifiers</b><br>";
        output += $"Speed: {FormatValue(Speed)}<br>";
        output += $"Luck: {FormatValue(Luck)}<br>";

        // Chances
        output += "<br><b>Chances</b><br>";
        output += $"Dodge: {FormatPercent(DodgeChance)}<br>";
        output += $"Block: {FormatPercent(BlockChance)}<br>";
        output += $"Proc: {FormatPercent(ProcChance)}<br>";

        return output;
    }

    private string FormatValue(float value)
    {
        return (Mathf.Round(value * 100) / 100).ToString();
    }

    private string FormatPercent(float value)
    {
        return $"{Mathf.Round(value * 100)}%";
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
