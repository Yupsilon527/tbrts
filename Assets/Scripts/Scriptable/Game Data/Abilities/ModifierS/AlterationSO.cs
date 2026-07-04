using UnityEngine;

[CreateAssetMenu(fileName = "Properties", menuName = "Abilities/Effects/Modifiers/Properties")]
public class AlterationSO : TagSO
{
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.Flag flag = ModifierDefines.Flag.Tag;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
    public override TagData Translate()
    {
        return new AlterationData(
            InternalName,
            sprite,
            uibehavior,
            priority,
            flag,
            properties,
            states
            );
    }
}
