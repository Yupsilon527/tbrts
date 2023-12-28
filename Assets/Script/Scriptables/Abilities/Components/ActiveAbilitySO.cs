using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilitySO : AbilitySO
{
    public bool RecievesWeaponBonuses = false;
    public AbilityEvent[] BaseFunctions;
    public override List<AbilityListener> TranslateFunctions()
    {
        var value = base.TranslateFunctions();
        foreach (var evt in BaseFunctions)
        {
            value.Add(new AbilityListener(evt));
        }
        return value;
    }
   protected void ApplyAttacks(CastTable CastData, AbilityEffect[] effects)
    {
        foreach (AbilityEffect attack in effects)
        {
            attack.Activate(CastData, 0);

        }
        foreach (Mob target in CastData.GetHitEntities(false))
        {
            if (target.GetAlignment(CastData.caster) == Mob.Alignment.enemy)
            target.FireEventOnSelf(AbilityDefines.Event.OnRecieveHit);
        }
        if (RecievesWeaponBonuses)
        {
            foreach (PropertyOrb orb in CastData.caster.modifiers.GetAttackModifiers())
            {
                foreach (AbilityEffect atk in orb.appliedAttacks)
                {
                    atk.Activate(CastData, 0);
                }
            }
        }
    }
    public AbilityDefines.AbilityType AbilityType;
    public override AbilityDefines.AbilityType GetAbilityType()
    {
        return AbilityType;
    }
    public float CastTime = 1;
    public override float GetCastTime()
    {
        return CastTime;
    }
}
