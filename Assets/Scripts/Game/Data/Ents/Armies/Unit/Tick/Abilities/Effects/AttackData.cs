using System;

[Serializable]
public class ApplyAttack : ApplyEffects
{
    public AttackDefines.ActionType attack;
    public float BaseDamage = 0;
    public ScaleData[] scaling;

    public override void ActivateOnUnit(CastTable table, DataItemUnit target, float strength = 1)
    {
        if (!base.Resolve(table)) return;
        float realDamage = BaseDamage * strength;
        foreach (var scale in scaling)
        {
            realDamage = scale.GetScaleStrength(table.caster, target, realDamage);
        }
        target.damageable.DealDamage(realDamage * table.proc, attack, table.caster,  table.hit);
    }

    public override string GetDescription()
    {
        string effect = $"{BaseDamage} {attack}";

        return base.GetDescription()
            .Replace("%effect%", "deal " + effect);
    }
}
[Serializable]
public class ScaleData
{
    public AttackDefines.ScaleType scaleMode;
    public float scaleDamage = 0;

    public AttackDefines.ScaleMode scaleoff = AttackDefines.ScaleMode.caster;
    public AttackDefines.ScaleRate scaleRate;
    public float GetScaleStrength(DataItemUnit attacker, DataItemUnit target, float baseDamage)
    {
        float bonusDamage = scaleDamage;
        if (attacker != null)
        {
            switch (scaleMode)
            {
               /* case ScaleType.AttackStat:
                    bonusDamage *= attacker.stats.realStats.AttackDamage;
                    break;
                case ScaleType.AttackPercent:
                    return baseDamage * attacker.stats.realStats.AttackDamage * scaleDamage;
                case ScaleType.AttackCrit:
                    return baseDamage * (bonusDamage + attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.critical_damage));
                case ScaleType.CurBlock:
                    bonusDamage *= attacker.damageable.Block.GetValue();
                    break;
                case ScaleType.SpeedStat:
                    bonusDamage *= attacker.stats.realStats.Speed;
                    break;
                case ScaleType.EmptySlots:
                    if (attacker is Hero attackingHero)
                        bonusDamage *= attackingHero.inventory.GetEmptyInventorySlots();
                    break;
                case ScaleType.UnarmedBonus:
                    bonusDamage *= attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.unarmed_bonus);
                    break;
                case ScaleType.AttackBonus:
                    bonusDamage *= attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.attack_bonus_base) + attacker.stats.realStats.AttackDamage * (attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.attack_bonus_percent) - 1);
                    break;
                case ScaleType.BlockBonus:
                    bonusDamage *= attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.block_bonus);
                    break;
                case ScaleType.ArmorBonus:
                case ScaleType.ArmorBonusRaw:
                    bonusDamage *= attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.armor_bonus);
                    if (scale == ScaleType.ArmorBonus)
                        bonusDamage *= attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.incoming_armor_from_bonus);
                    break;
                case ScaleType.Random:
                    bonusDamage *= UnityEngine.Random.value;
                    break;
                case ScaleType.TotHp:
                    bonusDamage *= target.stats.realStats.Health;
                    break;
                case ScaleType.MissHp:
                    bonusDamage *= target.damageable.Health.GetDifference();
                    break;
                case ScaleType.CurHp:
                    bonusDamage *= target.damageable.Health.GetValue();
                    break;
                case ScaleType.RollNumber:
                    DiceComponent dice = DiceRollManager.main.GetDiceResultForPlayer(attacker, diceIndex);
                    if (dice != null)
                    {
                        int diceSide = dice.GetUpSide();
                        bonusDamage *= 1 + attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.bonus_side_1 + diceSide) + +attacker.modifiers.GetPropertyAdditive(ModifierDefines.Property.bonus_all_sides);
                    }
                    break;
                case ScaleType.Poison:
                    if (target.modifiers.GetState(ModifierDefines.State.poison_immune))
                        return 0;
                    return Mathf.Max(1, baseDamage + target.modifiers.GetPropertyAdditive(ModifierDefines.Property.poison_damage_bonus)) * target.modifiers.GetPropertyMultiplicative(ModifierDefines.Property.poison_damage_incoming);
                case ScaleType.Fire:
                    if (target.modifiers.GetState(ModifierDefines.State.fire_immune))
                        return 0;
                    return Mathf.Max(1, baseDamage + target.modifiers.GetPropertyAdditive(ModifierDefines.Property.fire_damage_bonus)) * target.modifiers.GetPropertyMultiplicative(ModifierDefines.Property.fire_damage_incoming);
                default:
                    break;*/
            }
        }
        return bonusDamage * baseDamage;
    }
}