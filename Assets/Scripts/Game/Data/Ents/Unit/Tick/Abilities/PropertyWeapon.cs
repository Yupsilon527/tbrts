using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropertyWeapon : PropertyAbility
{
    public WeaponData original;
    public PropertyWeapon(DataItemUnit caster, WeaponData original) : base(caster)
    {
        startupDelay = original.castDelay;
        actionInterval = original.castTime;

        InternalName = original.InternalName;
        this.original = original;
    }

    public override bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return original.attackPhase == phase 
            && (!original.HasFlag(CombatDefines.AttackFlag.usableOnce) || uses==0)
            && (original.HasFlag(CombatDefines.AttackFlag.castInFrontRow) && parent.troopPosition.y == 0
            || original.HasFlag(CombatDefines.AttackFlag.castInBackRow) && parent.troopPosition.y == 1
            || original.HasFlag(CombatDefines.AttackFlag.castInTransport) && parent.troopPosition.y < 0
            || original.HasFlag(CombatDefines.AttackFlag.castInSupport) && !parent.troop.IsInCombat())
            && base.CanBeCast(phase);
    }
    public override bool HasResourcesToCast()
    {
        return parent.actions.Mp.GetValue() >= original.mpCost
        && parent.actions.Ap.GetValue() >= original.apCost
        && parent.actions.Sp.GetValue() >= original.spCost;

    }
    public override void SpendResources()
    {
        uses++;
        parent.actions.Mp.SubstractedValue(original.mpCost);
         parent.actions.Ap.SubstractedValue(original.apCost);
         parent.actions.Sp.SubstractedValue(original.spCost);
        ExtendCooldown( Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient ));
        base.SpendResources();
    }
    public override bool CastFromTable(CastTable table)
    {
        if (table is AttackTable at && table.attacker.CanAct(original.attackPhase) && base.CastFromTable(table))
        {
            at.ComputeTargets();
            if (at.maintarget == null || at.maintarget.Length == 0 || at.maintarget[0] == null) return false;
            at.Precast();
            table.attacker.FireEventOnSelf(AbilityDefines.Event.BeforeAttack);
            foreach (var attack in original.effects)
            {
                attack.Activate(table);
            }
            foreach (var target in table.maintarget)
            {
                target.damageable.ResolveDamate();
                table.attacker.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, target);
                target.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, table.attacker);
            }
            foreach (var target in table.sidetarget)
            {
                target.damageable.ResolveDamate();
                table.attacker.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, target);
                target.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, table.attacker);
            }
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
            if ((original.HasFlag(CombatDefines.AttackFlag.targetSelf) || caster!=u) 
                && (original.HasFlag(CombatDefines.AttackFlag.targetFrontRow) && (fPos.y == 0 || troop.formation.CountLivingTroopsInRow(0) == 0))
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

        if (caster.troopPosition.y == 0 && targets.Any(t => t.GetState(ModifierDefines.State.absolute_melee_priority)))
            targets = targets.Where(t => t.GetState(ModifierDefines.State.absolute_melee_priority)).ToArray();
        else if (caster.troopPosition.y == 1 && targets.Any(t => t.GetState(ModifierDefines.State.absolute_range_priority)))
            targets = targets.Where(t => t.GetState(ModifierDefines.State.absolute_range_priority)).ToArray();

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
