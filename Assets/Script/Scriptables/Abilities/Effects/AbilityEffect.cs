using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbilityEffect : ScriptableObject
{
    public AbilityDefines.TargetType Targeting;
    public SpecialEffectSO.EmitTime EmitDelay;
    public void Activate(CastTable table, float animdelay)
    {
        if (Targeting == AbilityDefines.TargetType.caster || Targeting == AbilityDefines.TargetType.caster_and_targets)
        {
            ActivateOnTargets(table, new Mob[] {table.caster}, animdelay + GetDelay());
        }
        if (Targeting == AbilityDefines.TargetType.targets || Targeting == AbilityDefines.TargetType.caster_and_targets)
        {
            ActivateOnTargets(table, table.GetHitEntities(false), animdelay + GetDelay());
        }
        if (Targeting == AbilityDefines.TargetType.ability_target)
        {
            ActivateOnTargets(table, new Mob[] { table.target }, animdelay + GetDelay());
        }
    }
    public float GetDelay()
    {
        return (float)EmitDelay * .1f;
    }
    public abstract void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay);
    #region Ability Alterations
    /*public Dictionary<AbilityDefines.AbilityAlterations, float> alterations;//new Dictionary<AbilityDefines.AbilityAlterations, float>()
    public void SetAlteration(AbilityDefines.AbilityAlterations prop, float value)
    {
        if (alterations.ContainsKey(prop))
        {
            alterations[prop] = value;
        }
        else
        {
            alterations.Add(prop, value);
        }
    }
    public float GetAlteration(AbilityDefines.AbilityAlterations property)
    {
        if (!alterations.ContainsKey(property))
            return 0;
        return alterations[property];
    }*/
    #endregion
}
