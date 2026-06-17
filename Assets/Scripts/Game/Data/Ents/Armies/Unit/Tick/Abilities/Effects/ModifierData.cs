using System;
using UnityEngine;

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

public class ModifierData
{

    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];

    public int duration = 1;
    public Sprite sprite;
    public ModifierDefines.Flag flag;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;

    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
}