using UnityEngine;

[CreateAssetMenu(fileName = "Apply Status", menuName = "Abilities/Effects/Apply Troop Status")]
public class ApplyStatusesSO : AttackEffectSO
{
    public ModifierDefines.TroopEffect effect;
    public float effectPower;
    public override ApplyEffects Translate()
    {
        return new ApplyTroopEffect()
        {
            applyChance = applyChance,
            targeting = targeting,
            chance = chance,
             effect = effect,
               effectPower = effectPower
        };
    }
}