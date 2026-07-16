using System;

[Serializable]
public class ApplyModifier : ApplyEffects
{
    public ModifierData appliedModifier;
    public ModifierParameterAlteration[] alterations;

    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        table. target.modifiers.ApplyNewModifierFromData(appliedModifier,table.caster,1, table.tick, out PropertyModifier modifier);
        foreach (var parameter in modifier.parameters)
        {
            foreach (var alteration in alterations)
            {
                if (alteration.parameter == parameter.Key)
                {
                    modifier.parameters[alteration.parameter] = AttackDefines.GetScaleStrength(table.caster, table.target, alteration.scaleoff, parameter.Value, alteration.scaleMode, alteration.scaleRate, alteration.scaleDamage);
                }
            }
        }
    }
    public override bool Resolve(CastTable table, float strength = 1)
    {
        if (!base.Resolve(table, strength)) return false;
        return true;

    }
    public override string GetDescription()
    {
        return base.GetDescription()
            .Replace("%effect%", "apply " + appliedModifier.InternalName) ;
    }
}
[Serializable]
public class ModifierParameterAlteration
{
    public string parameter;
    public AttackDefines.ScaleType scaleMode = AttackDefines.ScaleType.Nothing;
    public AttackDefines.ScaleMode scaleoff = AttackDefines.ScaleMode.caster;
    public AttackDefines.ScaleRate scaleRate = AttackDefines.ScaleRate.additive;
    public float scaleDamage = 0;

}
