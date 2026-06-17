using UnityEngine;


[CreateAssetMenu(fileName = "Apply Attacks", menuName = "Abilities/Effects/Apply Attacks")]
public class ApplyAttacksSO : AttackEffectSO
{
    public AttackDefines.ActionType attack;
    public float BaseDamage = 0;
    public ScaleData[] scaling;
    public override ApplyEffects Translate()
    {
        return new ApplyAttack()
        {
            attack = attack,
            BaseDamage = BaseDamage,
            scaling = scaling,
            applyChance = applyChance,
            targeting = targeting,
            chance = chance,
        };
    }
}