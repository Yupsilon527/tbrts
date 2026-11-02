using System;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public  class SpriteAnimator : FlatAnimator
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected override void ApplyCurrentFrame()
    {
        var sprite = GetSpriteAtTime(currentAnimation, animTime);
        if (sprite != null && spriteRenderer != null) { 
            spriteRenderer.flipX = !facesRight;
            spriteRenderer.sprite = sprite;
    }
}
}
