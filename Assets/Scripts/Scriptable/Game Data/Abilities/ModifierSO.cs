using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Modifier")]
public class ModifierSO : AlterationSO
{
    public int duration = 1;
    public ModifierDefines.Flag flag;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;

    public override TagData Translate()
    {
        return new ModifierData()
        {
            InternalName = InternalName,
            sprite = sprite,
            behavior = behavior,
            uibehavior = uibehavior,
            priority = priority,
            properties = properties,
            states = states,
            duration = duration,
            flag = flag,
            expireType = expireType,
        };
    }
}

