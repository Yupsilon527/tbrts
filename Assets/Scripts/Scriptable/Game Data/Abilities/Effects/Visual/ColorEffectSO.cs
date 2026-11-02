using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Abilities/Effects/Visual/Character")]
public class ColorEffectSO : VisualEffectSO
{
    public Color color;
    public override ApplyEffects Translate()
    {
        return new ColorEffect()
        {
            delay = delay,
            duration = duration,
            color = color
        };
    }
}