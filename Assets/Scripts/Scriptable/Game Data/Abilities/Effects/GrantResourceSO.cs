using UnityEngine;

[CreateAssetMenu(fileName = "Grant Resource", menuName = "Abilities/Effects/Grant Resource")]
public class GrantResourceSO : AttackEffectSO
{
    public ResourceCost[] resourceAmount;
    public override ApplyEffects Translate()
    {
        return new GrantResources() { resourceAmount = resourceAmount};
    }
}
