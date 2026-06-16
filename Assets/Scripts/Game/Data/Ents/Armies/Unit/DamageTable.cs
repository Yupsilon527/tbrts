using UnityEngine;

public class DamageTable
{
    public DataItemUnit attacker;
    public DataItemUnit target;
    public AttackDefines.DamageElement element;
    public AttackDefines.EffectType dmt;
    public float baseDamage;
    public float realDamage;
    public AttackDefines.BlockType blockType;

    //ability
    public DamageTable(DataItemUnit a, DataItemUnit t, float basedamage, AttackDefines.EffectType dtype, AttackDefines.DamageElement etype, AttackDefines.BlockType block)
    { attacker = a; target = t; baseDamage = basedamage; dmt = dtype; element = etype; blockType = block; Calculate(); }
    public void Calculate()
    {
        realDamage = baseDamage;
        switch (blockType)
        {
            case AttackDefines.BlockType.blocked:
                realDamage = Mathf.Max(1, realDamage - target.stats.realStats.Block - (element < AttackDefines.DamageElement.Physical ? target.stats.realStats.Armor : target.stats.realStats.Resistance));
                break;
            case AttackDefines.BlockType.normal:
                realDamage = Mathf.Max(1, realDamage - (element < AttackDefines.DamageElement.Physical ? target.stats.realStats.Armor : target.stats.realStats.Resistance));
                break;
            case AttackDefines.BlockType.halfBlock:
                realDamage = Mathf.Max((realDamage / 2), realDamage - target.stats.realStats.Armor);
                break;
        }

        if (blockType != AttackDefines.BlockType.ignoreArmor)
            realDamage = Mathf.Max(blockType == AttackDefines.BlockType.halfBlock ? (realDamage / 2) : 1, realDamage - (blockType == AttackDefines.BlockType.blocked ? target.stats.realStats.Block : 0) - target.stats.realStats.Armor);
        switch (dmt)
        {
            case AttackDefines.EffectType.DirectDamage:
                realDamage *= attacker.GetProperty(ModifierDefines.Properties.outgoing_damage);
                realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_damage);

                if (realDamage != 0)
                {
                    if (element != AttackDefines.DamageElement.Pure)
                    {
                        realDamage *= attacker.GetProperty(ModifierDefines.Properties.outgoing_pure_damage);
                    }
                    else
                    {
                        if (element < AttackDefines.DamageElement.Elemental)
                        {
                            realDamage *= attacker.GetProperty(ModifierDefines.Properties.outgoing_phys_damage);
                            realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_phys_damage);
                        }
                        else if (element >= AttackDefines.DamageElement.Elemental)
                        {
                            realDamage *= attacker.GetProperty(ModifierDefines.Properties.outgoing_elem_damage);
                            realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_elem_damage);
                        }

                        realDamage *= attacker.GetProperty(ModifierDefines.Properties.outgoing_blunt_damage + (int)element);
                        realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_blunt_damage + (int)element);
                    }
                }
                break;
            case AttackDefines.EffectType.Block:
                realDamage += target.GetProperty(ModifierDefines.Properties.shielding_bonus_flat);
                realDamage *= target.GetProperty(ModifierDefines.Properties.outgoing_shielding);
                break;
            case AttackDefines.EffectType.LifeHealNoOverheal:
            case AttackDefines.EffectType.LifeHealOverhealShield:
            case AttackDefines.EffectType.LifeHealOverhealArmor:
                realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_healing);
                break;
            case AttackDefines.EffectType.ArmorHeal:
                realDamage *= target.GetProperty(ModifierDefines.Properties.incoming_barrier);
                break;
        }
    }
    public void AccountBonusDamage()
    {
        if (dmt == AttackDefines.EffectType.Block)
        {

        }
    }
}