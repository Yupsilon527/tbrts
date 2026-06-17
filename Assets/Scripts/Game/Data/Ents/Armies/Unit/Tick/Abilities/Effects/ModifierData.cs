using System;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class ApplyModifier : ApplyEffects
{
    public ModifierData appliedModifier;
    public override bool Resolve(CastTable table, float strength = 1)
    {
        if (!base.Resolve(table, strength)) return false;
        table.maintarget.modifiers.ApplyNewModifierFromData(appliedModifier, table.tick, out PropertyModifier resulting) ;
        return true;

    }
    public override string GetDescription()
    {
        return base.GetDescription()
            .Replace("%effect%", "apply " + appliedModifier.name) ;
    }
}
