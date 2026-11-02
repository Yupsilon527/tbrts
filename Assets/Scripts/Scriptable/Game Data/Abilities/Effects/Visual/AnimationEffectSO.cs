public abstract class AnimationEffectSO : VisualEffectSO
{
    public abstract SpriteAnimation GetAnimation();
    public override ApplyEffects Translate()
    {
        return new AnimationEffect()
        {
            delay = delay,
            duration = duration,
            animation = GetAnimation()
        };
    }
}

