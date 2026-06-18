using System.Collections.Generic;
using UnityEngine;

public class DamageTable
{
    public DataItemUnit attacker;
    public DataItemUnit target;
    public AttackDefines.HitType blockType;
    public Dictionary<AttackDefines.ActionType, float> damages = new();

    //ability
    public DamageTable(DataItemUnit a, DataItemUnit t,   AttackDefines.HitType block)
    { attacker = a; target = t;  blockType = block; Calculate(); }
    public void CalcAttack(AttackDefines.ActionType damage, float val)
    {
        if (damages.ContainsKey(damage))
            damages[damage] += val;
        else
            damages.Add(damage, val);
    }
    public void Calculate()
    {
        realDamage = baseDamage;
        switch (blockType)
        {
            case AttackDefines.HitType.blocked:
                realDamage = Mathf.Max(1, realDamage - target.stats.realStats.Block - (element < AttackDefines.DamageElement.Physical ? target.stats.realStats.Armor : target.stats.realStats.Resistance));
                break;
            case AttackDefines.HitType.normal:
                realDamage = Mathf.Max(1, realDamage - (element < AttackDefines.DamageElement.Physical ? target.stats.realStats.Armor : target.stats.realStats.Resistance));
                break;
            case AttackDefines.HitType.halfBlock:
                realDamage = Mathf.Max((realDamage / 2), realDamage - target.stats.realStats.Armor);
                break;
        }

        if (blockType != AttackDefines.HitType.ignoreArmor)
            realDamage = Mathf.Max(blockType == AttackDefines.HitType.halfBlock ? (realDamage / 2) : 1, realDamage - (blockType == AttackDefines.HitType.blocked ? target.stats.realStats.Block : 0) - target.stats.realStats.Armor);
        switch (dmt)
        {
            case AttackDefines.ActionType.DirectDamage:
                realDamage *= attacker.GetProperty(ModifierDefines.Property.outgoing_damage);
                realDamage *= target.GetProperty(ModifierDefines.Property.incoming_damage);

                if (realDamage != 0)
                {
                    if (element != AttackDefines.DamageElement.Pure)
                    {
                        realDamage *= attacker.GetProperty(ModifierDefines.Property.outgoing_pure_damage);
                    }
                    else
                    {
                        if (element < AttackDefines.DamageElement.Elemental)
                        {
                            realDamage *= attacker.GetProperty(ModifierDefines.Property.outgoing_phys_damage);
                            realDamage *= target.GetProperty(ModifierDefines.Property.incoming_phys_damage);
                        }
                        else if (element >= AttackDefines.DamageElement.Elemental)
                        {
                            realDamage *= attacker.GetProperty(ModifierDefines.Property.outgoing_elem_damage);
                            realDamage *= target.GetProperty(ModifierDefines.Property.incoming_elem_damage);
                        }

                        realDamage *= attacker.GetProperty(ModifierDefines.Property.outgoing_blunt_damage + (int)element);
                        realDamage *= target.GetProperty(ModifierDefines.Property.incoming_blunt_damage + (int)element);
                    }
                }
                break;
            case AttackDefines.ActionType.Block:
                realDamage += target.GetProperty(ModifierDefines.Property.shielding_bonus_flat);
                realDamage *= target.GetProperty(ModifierDefines.Property.outgoing_shielding);
                break;
            case AttackDefines.ActionType.LifeHealNoOverheal:
            case AttackDefines.ActionType.LifeHealOverhealShield:
            case AttackDefines.ActionType.LifeHealOverhealArmor:
                realDamage *= target.GetProperty(ModifierDefines.Property.incoming_healing);
                break;
            case AttackDefines.ActionType.ArmorHeal:
                realDamage *= target.GetProperty(ModifierDefines.Property.incoming_barrier);
                break;
        }
    }
}