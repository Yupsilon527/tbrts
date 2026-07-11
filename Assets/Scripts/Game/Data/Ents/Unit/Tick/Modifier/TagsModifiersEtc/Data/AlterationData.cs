
using UnityEngine;

public class AlterationData : TagData
{
    public ModifierDefines.Flag flag;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];

    public AlterationData(string internalName, Sprite sprite,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null) : base(internalName, sprite, uibehavior)
    {
        this.priority = priority;
        this.flag = flag;
        this.properties = properties;
        this.states = states;
    }

    public AlterationData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior) : base(internalName, sprite, uibehavior)
    {
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
    public override ModifierDefines.Flag GetFlag()
    {
        return flag;
    }
}
