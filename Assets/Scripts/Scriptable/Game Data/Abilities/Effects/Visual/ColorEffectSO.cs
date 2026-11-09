using UnityEngine;

[CreateAssetMenu(fileName = "Flash Color", menuName = "Abilities/Effects/Visual/Flash Color")]
public class ColorEffectSO : VisualEffectSO
{
    public Color color;
    public override ApplyEffects[] Translate()
    {
        return new[]{new ColorEffect()
        {
            delay = delay,
            duration = duration,
            color = color
        }
        };
    }
}