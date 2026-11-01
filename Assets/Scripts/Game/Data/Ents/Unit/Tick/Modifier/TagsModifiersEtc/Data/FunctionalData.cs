using System;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalData : InheritAbilityData
{
    public HashSet<AbilityFunction> functions;
    public HashSet<AbilityReaction> reactions;

    public FunctionalData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null, HashSet<AbilityFunction> funcs = null, HashSet<AbilityReaction> list = null) : base(internalName, sprite, uibehavior, priority, flag, properties, states, grantedAbilities)
    {
        functions = funcs;
        reactions = list;
    }
}
public abstract class ListenerBase
{
    public bool onlyWhenActive;
    public ModifierDefines.ModifierAction action;
    public abstract bool IsValid();
}
[Serializable]
public class AbilityReaction : ListenerBase
{
    public AbilityDefines.ReactionEvent[] listeners;
    public AbilityDefines.ReactionTarget target;
    public AbilityDefines.ReactionTiming timing;

    public AbilityReaction(AbilityDefines.ReactionEvent evt = AbilityDefines.ReactionEvent.UseDirectAttack, AbilityDefines.ReactionTarget target = AbilityDefines.ReactionTarget.enemy, AbilityDefines.ReactionTiming timing = AbilityDefines.ReactionTiming.Before, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true) : this(new AbilityDefines.ReactionEvent[] { evt }, target, timing, action, onlyWhenActive) { }
    public AbilityReaction(AbilityDefines.ReactionEvent[] events , AbilityDefines.ReactionTarget target, AbilityDefines.ReactionTiming timing, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true)
    {
        this.onlyWhenActive = onlyWhenActive;
        listeners = events;
        this.target = target;
        this.timing = timing;
        this.action = action;
    }
    public override bool IsValid()
    {
        return action != null && listeners != null && listeners.Length > 0;
    }
}

[Serializable]
public class AbilityFunction : ListenerBase
{
    public AbilityDefines.Event[] events;

    public AbilityFunction(AbilityDefines.Event events = AbilityDefines.Event.Action, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true) : this(new AbilityDefines.Event[] { events }, action, onlyWhenActive) { }
    public AbilityFunction(AbilityDefines.Event[] events = null, ModifierDefines.ModifierAction action = null, bool onlyWhenActive = true)
    {
        this.onlyWhenActive = onlyWhenActive;
        this.events = events;
        this.action = action;
    }
    public override bool IsValid()
    {
        return action != null && events != null && events.Length > 0;
    }
}
