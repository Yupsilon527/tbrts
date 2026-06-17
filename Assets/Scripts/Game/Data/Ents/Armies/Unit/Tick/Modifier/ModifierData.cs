
using System.Collections.Generic;
using UnityEngine;

public class TagData
{
    public string InternalName;
    public Sprite sprite;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    //public VisualEffectSO[] visualEffects;

}
public class AlterationData : TagData
{
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
}
public class ModifierData : AlterationData
{
    public int duration = 1;
    public ModifierDefines.Flag flag;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions;

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