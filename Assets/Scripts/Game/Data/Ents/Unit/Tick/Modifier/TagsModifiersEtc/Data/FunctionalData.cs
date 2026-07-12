using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalData : InheritAbilityData
{
    public HashSet<AbilityFunction> functions;

    public FunctionalData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null, HashSet<AbilityFunction> funcs = null) : base(internalName, sprite, uibehavior, priority, flag, properties, states, grantedAbilities)
    {
        functions = funcs;
    }
}
[Serializable]
public class AbilityFunction
{
    public bool onlyWhenActive;
    public AbilityDefines.Event[] events;
    public ModifierDefines.ModifierAction action;

    public AbilityFunction( AbilityDefines.Event events =  AbilityDefines.Event.Action, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true) :this (new AbilityDefines.Event[] { events }, action, onlyWhenActive) { }
    public AbilityFunction( AbilityDefines.Event[] events = null, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true)
    {
        this.onlyWhenActive = onlyWhenActive;
        this.events = events;
        this.action = action;
    }
    public bool IsValid()
    {
        return action == null || events == null || events.Length == 0;
    }
}
