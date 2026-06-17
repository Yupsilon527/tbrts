using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Abilities/Effects/Modifiers/Tag")]
public class TagSO : ScriptableObject
{
    public Sprite sprite;
    public string InternalName;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;

    public virtual TagData Translate()
    {
        return new TagData()
        {
            InternalName = InternalName,
            sprite = sprite,
            behavior = behavior,
            uibehavior = uibehavior,
        };
    }
}
