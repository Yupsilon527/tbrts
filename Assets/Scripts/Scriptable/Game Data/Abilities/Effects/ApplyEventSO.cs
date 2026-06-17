using UnityEngine;

namespace VikingParty
{

    [CreateAssetMenu(fileName = "Apply Event", menuName = "Abilities/Effects/Apply Event")]
    public class ApplyEventSO : ApplyPhaseSO
    {
        public AbilityDefines.Event abilityEvent;
        public override void ActivateOnCaster(AttackTable table, RollTable roll)
        {
            if (targeting == AbilityDefines.TargetType.nobody) return;
            table.attacker.FireEventOnSelf(abilityEvent);
        }
        public override void ActivateOnTargets(AttackTable table, RollTable roll)
        {
            if (targeting == AbilityDefines.TargetType.nobody) return;
            table.target.FireEventOnSelf(abilityEvent);
        }
    }
}