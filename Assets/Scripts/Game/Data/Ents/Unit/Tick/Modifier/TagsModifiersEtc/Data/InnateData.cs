
using System.Collections.Generic;
using UnityEngine;

public class InnateData : FunctionalData
{
    public enum AuraType
    {
        innate,
        troop,
        aura,
    }
    public AuraType auraType;

    public InnateData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, AuraType aura = AuraType.innate, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite, uibehavior, priority, flag, properties, states, grantedAbilities, funcs)
    {
        auraType = aura;
    }
    public bool CanApplyToUnit(DataItemUnit caster, DataItemUnit unit)
    {
        if (unit.modifiers.HasModifier(InternalName)) return false;
        switch (auraType)
        {
            case AuraType.innate:
                return unit == caster;
            case AuraType.troop:
                return caster.troop == unit.troop;
        }
        return false;
    }
}
