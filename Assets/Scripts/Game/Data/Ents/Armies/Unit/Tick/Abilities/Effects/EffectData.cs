using UnityEngine;

public class ApplyEffects
{
    public CombatDefines.ChanceMult chance;
    public float applyChance = 1;
    public CombatDefines.Action action;
    public AttackDefines.DamageElement element;

    public virtual bool Resolve(CastTable table, float strength = 1)
    {
        return Passes(table, strength);
    }
    public virtual string GetDescription()
    {
        string desc = "%effect%";
            if (applyChance < 1 && chance != CombatDefines.ChanceMult.always)
            desc = $"{Mathf.Round(applyChance * 100)}% {chance} to "+ desc;
        desc += $" on {action}";
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
                return ranVal <( c + table.attacker.stats.realStats.ProcChance) * table.attacker.stats.realStats.LuckCoefficient;
            default: return true;
        }
    }
}
