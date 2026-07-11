using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalData : InheritAbilityData
{
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions;

    public FunctionalData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite, uibehavior, priority, flag, properties, states, grantedAbilities)
    {
        functions = funcs;
    }
}
