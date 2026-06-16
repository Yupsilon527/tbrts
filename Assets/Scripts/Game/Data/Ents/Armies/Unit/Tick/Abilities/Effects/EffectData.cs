using UnityEngine;

public abstract class ApplyEffects
{
    public CombatDefines.ChanceMult chance;
    public float applyChance = 1;
    public CombatDefines.TargetType targeting;

    public virtual void Activate(CastTable table, float strength = 1)
    {
            if (targeting == CombatDefines.TargetType.caster || targeting == CombatDefines.TargetType.caster_and_target)
            {
                ActivateOnCaster(table, strength);
            }
            if (targeting == CombatDefines.TargetType.targets || targeting == CombatDefines.TargetType.caster_and_target)
            {
                ActivateOnTargets(table, strength);
            }
    }
    public abstract void ActivateOnUnit(CastTable table, DataItemUnit target, float strength = 1);
    public virtual bool Resolve(CastTable table, float strength = 1)
    {
        if ( Passes(table, strength))
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
            desc = $"{Mathf.Round(applyChance * 100)}% {chance} to "+ desc;
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
                 return ranVal < c * table.caster.stats.realStats.LuckCoefficient;
            case CombatDefines.ChanceMult.Proc:
                return ranVal <( c + table.caster.stats.realStats.ProcChance) * table.caster.stats.realStats.LuckCoefficient;
            default: return true;
        }
    }
}
