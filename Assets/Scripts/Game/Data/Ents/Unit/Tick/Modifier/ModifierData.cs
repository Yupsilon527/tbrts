
using System;
using System.Collections.Generic;
using UnityEngine;

public class TagData
{
    public string InternalName;
    public Sprite sprite;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    public ModifierDefines.StackType behavior = ModifierDefines.StackType.Unique;
    //public VisualEffectSO[] visualEffects;

    public TagData(string internalName, Sprite sprite, ModifierDefines.StackType behavior, ModifierDefines.VisibleState uibehavior)
    {
        InternalName = internalName;
        this.sprite = sprite;
        this.uibehavior = uibehavior;
        this.behavior = behavior;
    }
}
public class AlterationData : TagData
{
    public ModifierDefines.Flag flag;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];

    public AlterationData(string internalName, Sprite sprite, ModifierDefines.StackType behavior = ModifierDefines.StackType.Stacking, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null) : base(internalName, sprite, behavior, uibehavior)
    {
        this.priority = priority;
        this.flag = flag;
        this.properties = properties;
        this.states = states;
    }

    public float GetProperty(ModifierDefines.Property property)
    {
        foreach (var p in properties)
        {
            if (p.Property == property)
                return p.value;
        }
        return 0;
    }
    public bool GetState(ModifierDefines.State state)
    {
        foreach (var s in states)
        {
            if (s.State == state)
                return true;
        }
        return false;
    }
}
public class FunctionalData : AlterationData
{
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions;
    public FunctionalData(string internalName, Sprite sprite, ModifierDefines.StackType behavior = ModifierDefines.StackType.Stacking, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite, behavior, uibehavior, priority, flag, properties, states)
    {
        functions = funcs;
    }
}
public class ModifierData : FunctionalData
{
    public int duration = 1;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;

    public ModifierData(string internalName, Sprite sprite, ModifierDefines.StackType behavior = ModifierDefines.StackType.Stacking, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Flag flag = ModifierDefines.Flag.Buff, ModifierDefines.ExpireType expire = ModifierDefines.ExpireType.permanent, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, int duration = 0, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite, behavior, uibehavior, priority, flag, properties, states)
    {
        this.InternalName = internalName;
        this.uibehavior = uibehavior;
        this.priority = priority;
        this.expireType = expire;
        this.duration = duration;
    }
}
public class AuraData : FunctionalData
{
    public enum AuraType
    {
        innate,
        troop,
        aura,
    }
    public AuraType innateType;
    public AuraData(string internalName, Sprite sprite, AuraType aura = AuraType.innate, ModifierDefines.StackType behavior = ModifierDefines.StackType.Stacking, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite, behavior, uibehavior, priority, flag, properties, states, funcs)
    {
        innateType = aura;
    }
}