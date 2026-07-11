using UnityEngine;

public class InheritAbilityData : AlterationData
{
    public AbilityData[] grantedAbilities;

    public InheritAbilityData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null) : base(internalName, sprite, uibehavior, priority, flag, properties, states)
    {
        this.grantedAbilities = grantedAbilities;
    }
}
