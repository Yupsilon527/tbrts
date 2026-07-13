using UnityEngine;

[CreateAssetMenu(fileName = "Apply Status", menuName = "Abilities/Effects/Apply Troop Status")]
public class ApplyBannerStatusSO : AttackEffectSO
{
    public ModifierDefines.TroopState effect;
    public int turnDuration;
    public override ApplyEffects Translate()
    {
        return new ApplyTroopStatus()
        {
            applyChance = applyChance,
            targeting = targeting,
            chance = chance,
             effect = effect,
              turnDuration = turnDuration
        };
    }
}