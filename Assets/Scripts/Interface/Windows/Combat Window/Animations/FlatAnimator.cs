using UnityEngine;

public abstract class FlatAnimator : Initializable
{

    public CharacterSheet character ;

    protected SpriteAnimation currentAnimation;
    protected float animTime;
    private int loopsCompleted;
    public Countdown readyTimer = new(3);
    public Countdown hurtTimer = new(.5f);

    protected bool facesRight = false;
    public bool FacesRight
    {
        get => facesRight;
        set => facesRight = value;
    }
    public bool IsPlaying => currentAnimation != null;
    public string CurrentAnimationName => currentAnimation?.name;
    public int CurrentPriority => currentAnimation?.priority ?? int.MinValue;


    private void Update()
    {
        if (currentAnimation == null)
            return;

        animTime += Time.deltaTime;

        if (animTime >= currentAnimation.duration)
        {
            loopsCompleted++;

            bool infinite = currentAnimation.repeats < 0;
            bool doneLooping = !infinite && loopsCompleted > currentAnimation.repeats;

            if (doneLooping)
            {
                ClearAnimation();
                return;
            }

            animTime %= Mathf.Max(currentAnimation.duration, 0.0001f);
        }

        ApplyCurrentFrame();
    }

    public Sprite GetSpriteAtTime(SpriteAnimation anim, float time)
    {
        if (anim == null)
            return null;

        int spriteIndex = anim.GetFrameSpriteIndex(time);

        if (spriteIndex == (int)UnitDefines.SpriteFrame.Idle)
        {
            if (hurtTimer.IsRunning())
            {
                spriteIndex = (int)UnitDefines.SpriteFrame.Hurt;
            }
        }

        return character. GetSprite(spriteIndex);
    }
    public bool PlayAnimation(SpriteAnimation anim)
    {
        if (anim == null)
            return false;

        if (currentAnimation != null && currentAnimation.priority > anim.priority)
            return false;

        currentAnimation = anim;
        animTime = anim.startTime;
        loopsCompleted = 0;
        ApplyCurrentFrame();
        return true;
    }

    public void ClearAnimation()
    {
        currentAnimation = null;
        animTime = 0f;
        loopsCompleted = 0;
        ApplyCurrentFrame(); 
    }

    protected abstract void ApplyCurrentFrame();
}
