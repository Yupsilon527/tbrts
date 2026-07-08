    using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageTable
{
    public bool resolved = false;
    public DataItemUnit attacker;
    public DataItemUnit target;
    public AttackDefines.HitType blockType;
    public Dictionary<AttackDefines.AttackType, float> baseDamage = new();
    public Dictionary<AttackDefines.AttackType, float> realDamage = new();

    //ability
    public DamageTable(DataItemUnit a, DataItemUnit t,   AttackDefines.HitType block)
    { attacker = a; target = t;  blockType = block;  }
    public void CalcAttack(AttackDefines.AttackType damage, float val)
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
                case AttackDefines.AttackType.Slashing:
                case AttackDefines.AttackType.Piercing:
                case AttackDefines.AttackType.Crushing:
                case AttackDefines.AttackType.Magical:
                case AttackDefines.AttackType.Poison:
                case AttackDefines.AttackType.Pure:

                    float armor = 0;

                    switch (kvp.Key)
                    {
                        case AttackDefines.AttackType.Slashing:
                            armor = target.stats.realStats.Armor;
                            break;
                        case AttackDefines.AttackType.Piercing:
                            armor = target.stats.realStats.Shield;
                            break;
                        case AttackDefines.AttackType.Crushing:
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
                                    outDamage = Mathf.Max(0, outDamage/2 - target.stats.realStats.Block  - armor);
                                    break;
                                case AttackDefines.HitType.miss:
                                    outDamage = 0;
                                    break;
                            }
                            break;
                        case AttackDefines.AttackType.Magical:
                           // outDamage = Mathf.Max(outDamage - target.stats.realStats.Resistance, outDamage / 2);
                            outDamage = UnitDamageable.AccountResistances(outDamage, target.stats.realStats.Resistance);
                            break;
                        case AttackDefines.AttackType.Poison:
                            outDamage = Mathf.Clamp(outDamage, 0, target.damageable.Health.GetValue() - 1);
                            break;
                        case AttackDefines.AttackType.ShieldHeal:
                            outDamage += target.GetProperty(ModifierDefines.Property.shielding_bonus_flat);
                            outDamage *= target.GetProperty(ModifierDefines.Property.outgoing_shielding);
                            break;
                        case AttackDefines.AttackType.LifeHealNoOverheal:
                        case AttackDefines.AttackType.LifeHealOverhealShield:
                        case AttackDefines.AttackType.LifeHealOverhealArmor:
                            outDamage *= target.GetProperty(ModifierDefines.Property.incoming_healing);
                            break;
                        case AttackDefines.AttackType.ArmorHeal:
                            outDamage *= target.GetProperty(ModifierDefines.Property.incoming_barrier);
                            break;

                    }
                    break;
            }
            realDamage[kvp.Key] = outDamage;
        }
        if (realDamage.ContainsKey(AttackDefines.AttackType.Slashing) 
            || realDamage.ContainsKey(AttackDefines.AttackType.Piercing)
            || realDamage.ContainsKey(AttackDefines.AttackType.Crushing))
        {
            float totalDamage = (realDamage.TryGetValue(AttackDefines.AttackType.Slashing, out float slash) ? slash : 0f)
                + (realDamage.TryGetValue(AttackDefines.AttackType.Piercing, out float pierce) ? pierce : 0f)
                + (realDamage.TryGetValue(AttackDefines.AttackType.Crushing, out float crush) ? crush : 0f)
                + (realDamage.TryGetValue(AttackDefines.AttackType.Pure, out float pure) ? pure : 0f);
            if (totalDamage == 0)
            {
                if (realDamage.ContainsKey(AttackDefines.AttackType.Pure))
                    realDamage[AttackDefines.AttackType.Pure] = 1;
                else
                    realDamage.Add(AttackDefines.AttackType.Pure, 1);
            }
        }
        realDamage = baseDamage;
    }
}