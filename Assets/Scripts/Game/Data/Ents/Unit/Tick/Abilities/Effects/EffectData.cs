using NUnit.Framework;
using System.Linq;
using UnityEngine;

public abstract class ApplyEffects
{
    public float applyChance = 1;
    public CombatDefines.ChanceMult chance;
    public CombatDefines.TargetType targeting;

    public virtual void Activate(CastTable table, float strength = 1)
    {
        switch (targeting)
        {
            case CombatDefines.TargetType.caster:
                ActivateOnUnit(new EventTable(table, table.attacker), strength);
                break;
            case CombatDefines.TargetType.main_target:
                foreach (var target in table.maintarget)
                    ActivateOnUnit(new EventTable(table, target), strength);
                break;
            case CombatDefines.TargetType.all_targets:
                foreach (var target in table.sidetarget)
                ActivateOnUnit(new EventTable(table, target), strength);
                break;
            case CombatDefines.TargetType.side_targets:
                foreach (var target in table.sidetarget.Where(t => !table.maintarget.Contains(t)))
                ActivateOnUnit(new EventTable(table, target), strength);
                break;
            case CombatDefines.TargetType.randomEnemy:
                //var randomEnemy = table.caster.troop.Formation[Mathf.FloorToInt(table.caster.troop.Formation.Length * Random.value)];
                //ActivateOnUnit(table, randomEnemy, strength);
                // TODO
                break;
            case CombatDefines.TargetType.randomAlly:
                var targets = table.attacker.troop.formation.GetUnits();
                var randomAlly = targets[Mathf.FloorToInt(targets.Length * Random.value)];
                ActivateOnUnit(new EventTable(table, randomAlly), strength);
                break;
        }
    }
    public abstract void ActivateOnUnit(EventTable table, float strength = 1);
    public virtual bool Resolve(CastTable table, float strength = 1)
    {
        if (Passes(table, strength))
        {
            Activate(table, strength);
            return true;
        }
        return false;
    }
    public virtual string GetDescription()
    {
        string desc = "%effect%";
        if (applyChance < 1 && chance != CombatDefines.ChanceMult.always)
            desc = $"{Mathf.Round(applyChance * 100)}% {chance} to " + desc;
        return desc;
    }
    public bool Passes(CastTable table, float pass = 1)
    {
        if (chance == CombatDefines.ChanceMult.always) return true;
        float c = applyChance / 100 * pass;
        if (c >= 1) return true;
        float ranVal = Random.value;
        switch (chance)
        {
            case CombatDefines.ChanceMult.True:
                return ranVal < c;
            case CombatDefines.ChanceMult.Luck:
                return ranVal < c * table.attacker.stats.realStats.LuckCoefficient;
            case CombatDefines.ChanceMult.Proc:
                return ranVal < (c + table.attacker.stats.realStats.ProcChance) * table.attacker.stats.realStats.LuckCoefficient;
            default: return true;
        }
    }
}
