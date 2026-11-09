using UnityEngine;

[CreateAssetMenu(fileName = "Play Animation", menuName = "Abilities/Effects/Visual/Play Animation")]
public class BuiltinAnimationEffectSO : AnimationEffectSO
{
    public BuiltInAnimations.BuiltinAnimation animation;

    public override SpriteAnimation GetAnimation()
    {
        switch (animation)
        {
            case BuiltInAnimations.BuiltinAnimation.Death:
                return BuiltInAnimations.Death;
            case BuiltInAnimations.BuiltinAnimation.Walk:
                return BuiltInAnimations.Walk;
            case BuiltInAnimations.BuiltinAnimation.Spell:
                return BuiltInAnimations.Spell;
                default:
                return BuiltInAnimations.Attack;
        }
    }
}
