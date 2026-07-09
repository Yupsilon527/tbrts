using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Effects/Modifiers/Modifier")]
public class ModifierSO : AlterationSO
{
    public int duration = 1;
    public int thinker = 0;
    public int level = 0;
    public ModifierDefines.ExpireType destroyEvt = ModifierDefines.ExpireType.ticks;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.ticks;

    public override TagData Translate()
    {
        return new ModifierData(
            InternalName,
            sprite,
            uibehavior,
            priority,
            flag,
            properties,
            states,
            new System.Collections.Generic.Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>(),
            behavior,
            duration,
            expireType,
            destroyEvt,
            thinker,
            level
            );
    }
}

