using System;

[Serializable]
public class ApplyProc : ApplyEffects
{
    public CombatDefines.Action proc;
    public override bool Resolve(CastTable table, float strength = 1)
    {
        if (!base.Resolve(table)) return false;
        table.attacker.abilities.Action(table.target, proc, table.tick, table.proc * strength);
        return true;
    }
    public override string GetDescription()
    {
        return base.GetDescription()
            .Replace("%effect%", proc.ToString());
    }
}
