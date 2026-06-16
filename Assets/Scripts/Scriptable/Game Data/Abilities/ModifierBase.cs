using UnityEngine;

public abstract class ModifierBase : ScriptableObject
{
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
}