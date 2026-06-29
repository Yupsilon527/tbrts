using UnityEngine;

public class PropertyAura : PropertyAttribute
{
    public bool globalAura = false;
    public override void ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        if (act == AbilityDefines.Event.OnMoveTile || act == AbilityDefines.Event.CombatBegin)
        {
            ReviseAura();
        }
        base.ExecuteEvent(act, target);
    }
    void ReviseAura()
    {
        if (parent.troop != caster.troop)
        {
            if (globalAura &&  (parent.troop.gridPos - caster.troop.gridPos).magnitude > caster.innates.GetAbilityLevel("aura"))
            {
                Die(true);
            }
        }
    }
}
