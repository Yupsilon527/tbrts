using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Abilities/Effects/Modifiers/Tag")]
public class TagSO : ScriptableObject
{
    public Sprite sprite;
    public string InternalName;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    public ModifierDefines.StackType behavior = ModifierDefines.StackType.Unique;

    public virtual TagData Translate()
    {
        return new TagData(
             InternalName,
             sprite,
            uibehavior
        );
    }
}
