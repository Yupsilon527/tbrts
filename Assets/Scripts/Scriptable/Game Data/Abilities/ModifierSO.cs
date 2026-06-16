using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Modifier")]
public class ModifierSO : ModifierBase
{
    public int duration = 1;
    public Sprite sprite;
    public ModifierDefines.Flag flag;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;

    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
}

