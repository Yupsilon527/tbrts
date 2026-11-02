using UnityEngine;

[CreateAssetMenu(fileName = "Custom Animation", menuName = "Abilities/Effects/Visual/Custom Animation")]
public class CustomAnimationEffectSO : AnimationEffectSO
{
    public SpriteAnimation animation;
    public override SpriteAnimation GetAnimation()
    {
        return animation;
    }
}
