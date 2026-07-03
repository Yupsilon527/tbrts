    using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageTable
{
    public bool resolved = false;
    public DataItemUnit attacker;
    public DataItemUnit target;
    public AttackDefines.HitType blockType;
    public Dictionary<AttackDefines.DamageType, float> baseDamage = new();
    public Dictionary<AttackDefines.DamageType, float> realDamage = new();

    //ability
    public DamageTable(DataItemUnit a, DataItemUnit t,   AttackDefines.HitType block)
    { attacker = a; target = t;  blockType = block;  }
    public void CalcAttack(AttackDefines.DamageType damage, float val)
    {
        if (baseDamage.ContainsKey(damage))
            baseDamage[damage] += val;
        else
            baseDamage.Add(damage, val);
    }
    public void Calculate()
    {
        foreach (var kvp in baseDamage.ToArray())
        {
            float outDamage = kvp.Value;
            switch (kvp.Key)
            {
                case AttackDefines.DamageType.Slashing:
                case AttackDefines.DamageType.Piercing:
                case AttackDefines.DamageType.Crushing:
                case AttackDefines.DamageType.Magical:
                case AttackDefines.DamageType.Poison:
                case AttackDefines.DamageType.Pure:

                    float armor = 0;

                    switch (kvp.Key)
                    {
                        case AttackDefines.DamageType.Slashing:
                            armor = target.stats.realStats.Armor;
                            break;
                        case AttackDefines.DamageType.Piercing:
                            armor = target.stats.realStats.Shield;
                            break;
                        case AttackDefines.DamageType.Crushing:
                            armor = target.stats.realStats.Padding;
                            break;
                    }

                    switch (kvp.Key)
                    {
                        default:
                            switch (blockType)
                            {
                                case AttackDefines.HitType.criticalHit:
                                    outDamage = Mathf.Max(outDamage / 2, outDamage * 2 - armor);
                                    break;
                                case AttackDefines.HitType.normal:
                                    outDamage = Mathf.Max(0, outDamage - armor);
                                    break;
                                case AttackDefines.HitType.blocked:
                                    outDamage = Mathf.Max(0, outDamage - target.stats.realStats.Block - armor);
                                    break;
                                case AttackDefines.HitType.blockCrit:
                                    outDamage = Mathf.Max(0, outDamage - target.stats.realStats.Block * 2 - armor);
                                    break;
                                case AttackDefines.HitType.miss:
                                    outDamage = 0;
                                    break;
                            }
                            break;
                        case AttackDefines.DamageType.Magical:
                            outDamage = Mathf.Max(outDamage - target.stats.realStats.Resistance, outDamage / 2);
                            break;
                        case AttackDefines.DamageType.Poison:
                            outDamage = Mathf.Clamp(outDamage, 0, target.damageable.Health.GetValue() - 1);
                            break;
                        case AttackDefines.DamageType.ShieldHeal:
                            outDamage += target.GetProperty(ModifierDefines.Property.shielding_bonus_flat);
                            outDamage *= target.GetProperty(ModifierDefines.Property.outgoing_shielding);
                            break;
                        case AttackDefines.DamageType.LifeHealNoOverheal:
                        case AttackDefines.DamageType.LifeHealOverhealShield:
                        case AttackDefines.DamageType.LifeHealOverhealArmor:
                            outDamage *= target.GetProperty(ModifierDefines.Property.incoming_healing);
                            break;
                        case AttackDefines.DamageType.ArmorHeal:
                            outDamage *= target.GetProperty(ModifierDefines.Property.incoming_barrier);
                            break;

                    }
                    break;
            }
            realDamage[kvp.Key] = outDamage;
        }
        if (realDamage.ContainsKey(AttackDefines.DamageType.Slashing) 
            || realDamage.ContainsKey(AttackDefines.DamageType.Piercing)
            || realDamage.ContainsKey(AttackDefines.DamageType.Crushing))
        {
            float totalDamage = (realDamage.TryGetValue(AttackDefines.DamageType.Slashing, out float slash) ? slash : 0f)
                + (realDamage.TryGetValue(AttackDefines.DamageType.Piercing, out float pierce) ? pierce : 0f)
                + (realDamage.TryGetValue(AttackDefines.DamageType.Crushing, out float crush) ? crush : 0f)
                + (realDamage.TryGetValue(AttackDefines.DamageType.Pure, out float pure) ? pure : 0f);
            if (totalDamage == 0)
            {
                if (realDamage.ContainsKey(AttackDefines.DamageType.Pure))
                    realDamage[AttackDefines.DamageType.Pure] = 1;
                else
                    realDamage.Add(AttackDefines.DamageType.Pure, 1);
            }
        }
        realDamage = baseDamage;
    }
}