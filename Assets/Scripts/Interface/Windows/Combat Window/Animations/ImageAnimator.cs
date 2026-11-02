using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public  class UiAnimator : FlatAnimator
{
    [SerializeField] private Image targetImage;
    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }
    protected override void ApplyCurrentFrame()
    {
        var sprite = GetSpriteAtTime(currentAnimation, animTime);
        if (sprite != null && targetImage != null) {
            targetImage.sprite = sprite;
    }
    }
}