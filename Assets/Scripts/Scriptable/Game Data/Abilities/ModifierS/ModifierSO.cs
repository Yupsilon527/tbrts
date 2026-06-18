using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Effects/Modifiers/Modifier")]
public class ModifierSO : AlterationSO
{
    public int duration = 1;
    public ModifierDefines.Flag flag;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;

    public override TagData Translate()
    {
        return new ModifierData(
            InternalName,
            sprite,
            behavior,
            uibehavior,
            flag,
            expireType,
            priority,
            duration,
            properties,
            states
            );
    }
}

