using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ApplyConditionalEffect : AbilityEffect
{
    public string ModifierCheck = "";
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        foreach (Mob mob in targets)
        {
            if (PassCheck(mob))
                {
                ActivateOnTarget(table, mob, animdelay);
            }
        }
    }
    bool PassCheck(Mob target)
    {
        if (ModifierCheck != "" && (!target?.modifiers?.HasModifier(ModifierCheck) ?? false))
            return false;
        return true;
    }
    public abstract void ActivateOnTarget(CastTable table, Mob target, float animdelay);
}
