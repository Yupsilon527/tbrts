using UnityEngine;


[CreateAssetMenu(fileName = "Trigger Event", menuName = "Abilities/Effects/Trigger Event")]
public class ApplyEventSO : AttackEffectSO
{
    public AbilityDefines.Event abilityEvent;

    public override ApplyEffects Translate()
    {
        return new EventData()
        {
            abilityEvent = abilityEvent,
            applyChance = applyChance,
            targeting = targeting,
            chance = chance,
        };
    }
}
