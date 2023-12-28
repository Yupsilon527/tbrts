using UnityEngine;
using UnityEditor;

public class DamageTable
{

    public Mob attacker;
    public Mob target;
    public float damage;
    public AttackDefines.DamageType dmt;
    public AttackDefines.DamageFlag flags;
    public float real_damage;
    public float  time = Time.time;

    //ability
    public DamageTable(Mob a, Mob t, float basedamage, AttackDefines.DamageType dtype, AttackDefines.DamageFlag dflag)
    { attacker = a; target = t; damage = basedamage; Calculate(); dmt = dtype; flags = dflag; }
    public bool IsDirectDamage()
    {
        return flags == AttackDefines.DamageFlag.Melee || flags == AttackDefines.DamageFlag.Range || flags == AttackDefines.DamageFlag.Area;
    }
    public void Calculate()
    {
        Debug.Log("[Health] Calculate damage type on " + target.name);
        real_damage = damage;

        switch (dmt)
        {
            default:
                if (attacker.modifiers != null)
                    real_damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_damage);
                if (target.modifiers != null)
                    real_damage *= target.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.incoming_damage);
                break;
            case AttackDefines.DamageType.LifeHeal:
                if (attacker.modifiers != null)
                    real_damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_healing);
                if (target.modifiers != null)
                    real_damage *= target.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.incoming_healing);
                break;

            case AttackDefines.DamageType.ShieldHeal:
                if (attacker.modifiers != null)
                    real_damage *= attacker.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.outgoing_shielding);
                break;

        }
    }

}