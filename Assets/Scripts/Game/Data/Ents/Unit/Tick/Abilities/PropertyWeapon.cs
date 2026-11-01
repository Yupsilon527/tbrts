using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PropertyWeapon : PropertyAbility
{
    public WeaponData original;
    public override string ToString()
    {
        return $"{original.InternalName} {original.castTime}/{original.castDelay}";
    }
    public PropertyWeapon(DataItemUnit caster, WeaponData original) : base(caster)
    {
        startupDelay = original.castDelay;
        actionInterval = original.castTime;

        InternalName = original.InternalName;
        this.original = original;
    }
    public override bool GetFlag(int flag)
    {
        return original.HasFlag((CombatDefines.AttackFlag)flag);
    }

    public override bool IsUsableInPhase(CombatDefines.AttackPhase phase)
    {
        return original.attackPhase == phase
            && (original.HasFlag(CombatDefines.AttackFlag.castInFrontRow) && parent.troopPosition.y == 0
            || original.HasFlag(CombatDefines.AttackFlag.castInBackRow) && parent.troopPosition.y == 1
            || original.HasFlag(CombatDefines.AttackFlag.castInTransport) && parent.troopPosition.y < 0
            || original.HasFlag(CombatDefines.AttackFlag.castInSupport) && !parent.troop.IsInCombat());
    }
    public override bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return IsUsableInPhase(phase)
            && ((original.HasFlag(CombatDefines.AttackFlag.indirectAttack) && !parent.GetState(ModifierDefines.State.cannot_cast))
            || (!original.HasFlag(CombatDefines.AttackFlag.indirectAttack) && !parent.GetState(ModifierDefines.State.cannot_attack)))
            && (!original.HasFlag(CombatDefines.AttackFlag.usableOnce) || uses == 0)
            && base.CanBeCast(phase);
    }
    public override bool HasResourcesToCast()
    {
        return parent.actions.ReactionPoints.GetValue() >= original.rpCost
        && parent.actions.ActionPoint.GetValue() >= original.apCost
        && parent.actions.SupplyPoints.GetValue() >= original.spCost;

    }
    public override void SpendResources()
    {
        uses++;
        parent.actions.ReactionPoints.SubstractedValue(original.rpCost);
        parent.actions.ActionPoint.SubstractedValue(original.apCost);
        parent.actions.SupplyPoints.SubstractedValue(original.spCost);
        ExtendCooldown(Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient));
        base.SpendResources();
    }
    public override bool CastFromTable(CastTable table)
    {
        if (table is AttackTable at && table.attacker.CanAct(original.attackPhase) && base.CastFromTable(table))
        {
            at.ComputeTargets();
            if (at.maintarget == null || at.maintarget.Length == 0 || at.maintarget[0] == null) return false;
            Combat.main.Inspect($"{table.attacker} casts ability {table.ability}");

            var allyTroop = table.attacker.troop.formation.GetUnits();
            var enemyTroop = Combat.main.GetOppositeSide(table.attacker.troop).formation.GetUnits();

            bool attackEnemy = !original.HasFlag(CombatDefines.AttackFlag.targetAllies);
            bool indirect = original.HasFlag(CombatDefines.AttackFlag.indirectAttack);
            bool magic = original.HasFlag(CombatDefines.AttackFlag.magicAttack);
            bool singleTarget = original.areaMode == CombatDefines.CombatantTargetingArea.single;

            //alert before cast
            Events(indirect, singleTarget, attackEnemy, magic, table, AbilityDefines.ReactionTiming.Before);

            foreach (var target in table.maintarget)
            {
                table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeAttack, target);
                if (magic) table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeMagicAttack, target);
                if (indirect) table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeDirectAttack, target);
                else table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeIndirectAttack, target);
            }


            foreach (var attack in original.effects)
            {
                attack.Activate(table);
            }
            foreach (var target in table.maintarget)
            {
                target.damageable.ResolveDamage();
            }
            foreach (var target in table.sidetarget)
            {
                target.damageable.ResolveDamage();
            }

            table.attacker.FireEventOnSelf(AbilityDefines.Event.AfterAttack);
            if (attackEnemy )
            {
                foreach (var ally in allyTroop)
                {
                    if (ally != table.attacker)
                    {
                        if (singleTarget)
                        {
                            ally.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseSingleIndirect : AbilityDefines.ReactionEvent.UseSingleDirect, AbilityDefines.ReactionTiming.Assist, table.attacker);
                            if (magic) ally.ReactToAction(AbilityDefines.ReactionEvent.UseSingleMagic, AbilityDefines.ReactionTiming.Assist, table.attacker);
                        }
                        else
                        {
                            ally.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseMultiIndirect : AbilityDefines.ReactionEvent.UseMultiDirect, AbilityDefines.ReactionTiming.Assist, table.attacker);
                            if (magic) ally.ReactToAction(AbilityDefines.ReactionEvent.UseMultiMagic, AbilityDefines.ReactionTiming.Assist, table.attacker);
                        }
                    }
                }
            }

            Events(indirect, singleTarget, attackEnemy, magic, table, AbilityDefines.ReactionTiming.After);
            return true;
        }
        return false;
    }
    void Events(bool indirect, bool singleTarget, bool attackenemy, bool magic, CastTable table, AbilityDefines.ReactionTiming timing)
    {
        if (attackenemy)
            foreach (var unit in Combat.main.combatants)
            {
                unit.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseIndirectAttack : AbilityDefines.ReactionEvent.UseDirectAttack, timing, table.attacker);
                if (singleTarget)
                    unit.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseSingleIndirect : AbilityDefines.ReactionEvent.UseSingleDirect, timing, table.attacker);
                else
                    unit.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseMultiIndirect : AbilityDefines.ReactionEvent.UseMultiDirect, timing, table.attacker);

                if (magic)
                {
                    unit.ReactToAction(AbilityDefines.ReactionEvent.UseMagicAttack, timing, table.attacker);
                    unit.ReactToAction(indirect ? AbilityDefines.ReactionEvent.UseSingleMagic : AbilityDefines.ReactionEvent.UseMultiMagic, timing, table.attacker);
                }
            }
        if (singleTarget)
        {
            foreach (var target in table.maintarget)
            {
                target.ReactToAction(AbilityDefines.ReactionEvent.HitBySingle, timing, table.attacker);
                target.ReactToAction(indirect ? AbilityDefines.ReactionEvent.HitByIndirect : AbilityDefines.ReactionEvent.HitByDirect, timing, table.attacker);
                target.ReactToAction(indirect ? AbilityDefines.ReactionEvent.HitBySingleIndirect : AbilityDefines.ReactionEvent.HitBySingleDirect, timing, table.attacker);
                if (magic)
                {
                    target.ReactToAction(AbilityDefines.ReactionEvent.HitByMagic, timing, table.attacker);
                }
            }
        }
        else
        {
            foreach (var target in table.sidetarget)
            {
                target.ReactToAction(AbilityDefines.ReactionEvent.HitByMulti, timing, table.attacker);
                target.ReactToAction(indirect ? AbilityDefines.ReactionEvent.HitByIndirect : AbilityDefines.ReactionEvent.HitByDirect, timing, table.attacker);
                target.ReactToAction(indirect ? AbilityDefines.ReactionEvent.HitByMultiIndirect : AbilityDefines.ReactionEvent.HitByMultiIndirect, timing, table.attacker);
                if (magic)
                {
                    target.ReactToAction(AbilityDefines.ReactionEvent.HitByMagic, timing, table.attacker);
                }
            }
        }
    }
    public override DataItemUnit[] GetValidTargets(DataItemUnit caster)
    {
        Vector2Int cPos = caster.troopPosition;
        var troop = original.HasFlag(CombatDefines.AttackFlag.targetAllies) ? caster.troop : Combat.main.GetOppositeSide(caster.troop);

        HashSet<DataItemUnit> units = new();
        foreach (var u in troop.formation.Formation)
        {
            if (u == null) continue;
            Vector2Int fPos = u.troopPosition;
            if ((original.HasFlag(CombatDefines.AttackFlag.targetSelf) && caster != u)
                || (original.HasFlag(CombatDefines.AttackFlag.targetFrontRow) && (fPos.y == 0 || troop.formation.CountLivingTroopsInRow(0) == 0))
                || (original.HasFlag(CombatDefines.AttackFlag.targetBackRow) && (fPos.y == 1 || troop.formation.CountLivingTroopsInRow(1) == 0))
                || (original.HasFlag(CombatDefines.AttackFlag.targetOwnCol) && fPos.x == cPos.x))
                units.Add(u);
        }
        return units.ToArray();
    }
    public DataItemUnit GetBestTargetForAbility(DataItemUnit caster)
    {
        var targets = GetValidTargets(caster);
        if (targets.Length == 0) return null;

        if (caster.troopPosition.y == 0 && targets.Any(t => t.GetState(ModifierDefines.State.priority_melee_target)))
            targets = targets.Where(t => t.GetState(ModifierDefines.State.priority_melee_target)).ToArray();
        else if (caster.troopPosition.y == 1 && targets.Any(t => t.GetState(ModifierDefines.State.priority_range_target)))
            targets = targets.Where(t => t.GetState(ModifierDefines.State.priority_range_target)).ToArray();

        switch (original.targetPriority)
        {
            default:
                return targets[Mathf.FloorToInt(targets.Length * Random.value)];
        }
    }

    public override DataItemUnit[] GetMainTargets(CastTable table)
    {
        return new DataItemUnit[] { GetBestTargetForAbility(table.attacker) };
    }

    public override DataItemUnit[] GetAreaTargets(CastTable table)
    {
        switch (original.areaMode)
        {
            default:
                return GetMainTargets(table);
            case CombatDefines.CombatantTargetingArea.row:
                return Combat.main.GetUnitsInRow(!table.attackingSide, table.targetPoint.x);
            case CombatDefines.CombatantTargetingArea.column:
                return Combat.main.GetUnitsInColumn(!table.attackingSide, table.targetPoint.y);
            case CombatDefines.CombatantTargetingArea.all:
                return Combat.main.GetTroopsInSide(!table.attackingSide);
        }
    }
}
