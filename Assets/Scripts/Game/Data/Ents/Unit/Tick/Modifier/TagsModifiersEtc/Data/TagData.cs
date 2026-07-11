
using UnityEngine;

public class TagData
{
    public string InternalName;
    public Sprite sprite;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    //public VisualEffectSO[] visualEffects;

    public TagData(string internalName, Sprite sprite,  ModifierDefines.VisibleState uibehavior)
    {
        InternalName = internalName;
        this.sprite = sprite;
        this.uibehavior = uibehavior;
    }
    public virtual ModifierDefines.Flag GetFlag()
    {
        return ModifierDefines.Flag.Tag;
    }
}
