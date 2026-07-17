using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public override bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return original.attackPhase == phase 
            && ((original.HasFlag(CombatDefines.AttackFlag.indirectAttack) && !parent.GetState(ModifierDefines.State.cannot_cast))
            || (!original.HasFlag(CombatDefines.AttackFlag.indirectAttack) && !parent.GetState(ModifierDefines.State.cannot_attack)))
            && (!original.HasFlag(CombatDefines.AttackFlag.usableOnce) || uses==0)
            && (original.HasFlag(CombatDefines.AttackFlag.castInFrontRow) && parent.troopPosition.y == 0
            || original.HasFlag(CombatDefines.AttackFlag.castInBackRow) && parent.troopPosition.y == 1
            || original.HasFlag(CombatDefines.AttackFlag.castInTransport) && parent.troopPosition.y < 0
            || original.HasFlag(CombatDefines.AttackFlag.castInSupport) && !parent.troop.IsInCombat())
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
        ExtendCooldown( Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient ));
        base.SpendResources();
    }
    public override bool CastFromTable(CastTable table)
    {
        if (table is AttackTable at && table.attacker.CanAct(original.attackPhase) && base.CastFromTable(table))
        {
            at.ComputeTargets();
            if (at.maintarget == null || at.maintarget.Length == 0 || at.maintarget[0] == null) return false;

            var allyTroop = table.attacker.troop.formation.GetUnits();
            var enemyTroop = Combat.main.GetOppositeSide( table.attacker.troop).formation.GetUnits();

            bool attackEnemy = !original.HasFlag(CombatDefines.AttackFlag.targetAllies);
            bool indirect = original.HasFlag(CombatDefines.AttackFlag.indirectAttack);
            bool magic = original.HasFlag(CombatDefines.AttackFlag.magicAttack);
            bool singleTarget = original.areaMode == CombatDefines.CombatantTargetingArea.single;

            if (singleTarget)
            {
                foreach (var target in table.maintarget)
                {
                    target.FireEventOnTarget(indirect ? AbilityDefines.Event.OnHitBySingleIndirect : AbilityDefines.Event.OnHitBySingleDirect, table.attacker);
                    if (magic) target.FireEventOnTarget(AbilityDefines.Event.OnEnemyUseMagicAttack, table.attacker);
                }
            }
            else
            {
                foreach (var target in table.sidetarget)
                {
                    target.FireEventOnTarget(indirect ? AbilityDefines.Event.OnHitByMultiIndirect : AbilityDefines.Event.OnHitByMultiDirect, table.attacker);
                    if (magic) target.FireEventOnTarget(AbilityDefines.Event.OnEnemyUseMagicAttack, table.attacker);
                }
            }
            foreach (var target in table.maintarget)
            {
                table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeAttack, target);
                if (magic) table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeMagicAttack, target);
                if (indirect) table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeDirectAttack, target);
                else table.attacker.FireEventOnTarget(AbilityDefines.Event.BeforeIndirectAttack, target);
            }

            foreach (var ally in allyTroop)
            {
                if (ally != table.attacker)
                    ally.FireEventOnTarget(indirect ? AbilityDefines.Event.BeforeAllyUseIndirectAttack : AbilityDefines.Event.BeforeAllyUseDirectAttack, table.attacker);
            }
            foreach (var enemy in enemyTroop)
            {
                if (attackEnemy)
                {
                    enemy.FireEventOnTarget(singleTarget ? AbilityDefines.Event.OnEnemyUseSingleAttack : AbilityDefines.Event.OnEnemyUseMultiAttack, table.attacker);
                    enemy.FireEventOnTarget(indirect ? AbilityDefines.Event.OnEnemyUseIndirectAttack : AbilityDefines.Event.OnEnemyUseDirectAttack, table.attacker);
                    if (magic) enemy.FireEventOnTarget( AbilityDefines.Event.OnEnemyUseMagicAttack, table.attacker);
            }
            }

            foreach (var attack in original.effects)
            {
                attack.Activate(table);
            }
            foreach (var target in table.maintarget)
            {
                target.damageable.ResolveDamate();
                if (attackEnemy && singleTarget)
                {
                    foreach (var ally in allyTroop)
                    {
                        if (ally != table.attacker) {
                            ally.FireEventOnTarget(indirect ? AbilityDefines.Event.AssistSingleIndirect : AbilityDefines.Event.AssistSingleDirect, target);
                            if (magic) ally.FireEventOnTarget(AbilityDefines.Event.AssistSingleMagic, target);
                        }
                    }
                    foreach (var enemy in enemyTroop)
                    {
                        if (enemy != target) { 
                            enemy.FireEventOnTarget(AbilityDefines.Event.AfterAllyHitBySingle, target);

                            enemy.FireEventOnTarget(indirect ? AbilityDefines.Event.AfterAfterHitByIndirect : AbilityDefines.Event.AfterAfterHitByDirect, target);
                            if (magic) enemy.FireEventOnTarget(AbilityDefines.Event.AfterAfterHitByMagic, target);
                        }
                    }
                }
            }
            foreach (var target in table.sidetarget)
            {
                target.damageable.ResolveDamate();

                if (attackEnemy && !singleTarget)
                {
                    foreach (var enemy in enemyTroop)
                    {
                        if (enemy != target)
                            enemy.FireEventOnTarget(AbilityDefines.Event.AfterAfterHitByMulti, target);
                    }
                }
            }
            table.attacker.FireEventOnSelf(AbilityDefines.Event.AfterAttack);
            return true;
        }
        return false;
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
            if ((original.HasFlag(CombatDefines.AttackFlag.targetSelf) && caster!=u) 
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
