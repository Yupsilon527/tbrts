using System;

[Serializable]
public class ApplyModifier : ApplyEffects
{
    public ModifierSO appliedModifier;
    public override bool Resolve(CastTable table, float strength = 1)
    {
        if (!base.Resolve(table, strength)) return false;
        table.target.modifiers.ApplyNewModifierFromData(appliedModifier, table.tick, out PropertyModifier resulting) ;
        return true;

    }
    public override string GetDescription()
    {
        return base.GetDescription()
            .Replace("%effect%", "apply " + appliedModifier.name) ;
    }
}
