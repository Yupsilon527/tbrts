using UnityEngine;
using UnityEngine.U2D;

public class AlterationSO : TagSO
{
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
    public override TagData Translate()
    {
        return new AlterationData()
        {
            InternalName = InternalName,
            sprite = sprite,
            behavior = behavior,
            uibehavior = uibehavior,
            priority = priority,
            properties = properties,
            states = states,
        };
    }
}
