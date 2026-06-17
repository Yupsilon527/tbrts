
using UnityEngine;

[CreateAssetMenu(fileName = "Apply Modifier", menuName = "Abilities/Effects/Apply Modifier")]
public class ApplyModifierSO : AttackEffectSO
{
    public TagSO appliedModifier;
    public ModifierParameterAlteration[] alteredParameters;

    public override ApplyEffects Translate()
    {
        return new ApplyModifier()
        {
            appliedModifier = appliedModifier.Translate(),
            alterations = alteredParameters,
            applyChance = applyChance,
            targeting = targeting,
            chance = chance,
        };
    }
}
