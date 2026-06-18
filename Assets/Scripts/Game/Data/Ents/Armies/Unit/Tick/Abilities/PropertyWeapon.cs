using UnityEngine;

public class PropertyWeapon : PropertyAbility
{
    public WeaponData original;
    public override bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return original.attackPhase == phase && base.CanBeCast(phase);
    }
    public override bool HasResourcesToCast()
    {
        return base.HasResourcesToCast();
    }
    public override void SpendResources()
    {
        base.SpendResources();
    }
    public override bool CastFromTable(CastTable table)
    {
     if (table is AttackTable at && table.attacker.CanAct(original.attackPhase) && base.CastFromTable(table))
        {
            at.ComputeTargets();
            at.Precast();
            foreach (var attack in original.effects)
            {
                attack.Activate(table);
            }
            return true;
        }
        return false;
    }
    public DataItemUnit GetBestUnitForAbility()
    {
        switch (original.targetPriority)
        {
            default:
                return null;
        }
    }

    public override DataItemUnit[] GetMainTargets(CastTable table)
    {
        return new DataItemUnit[] { Combat.main.GetUnitAt(!table.attackingSide, table.targetPoint.x, table.targetPoint.y) };
    }

    public override DataItemUnit[] GetAreaTargets(CastTable table)
    {
        switch (original.areaMode)
        {
            default:
                return GetMainTargets(table);
            case CombatDefines.ArmyTargetingArea.row:
                return Combat.main.GetUnitsInRow(!table.attackingSide, table.targetPoint.x);
            case CombatDefines.ArmyTargetingArea.column:
                return Combat.main.GetUnitsInColumn(!table.attackingSide, table.targetPoint.y);
            case CombatDefines.ArmyTargetingArea.all:
                return Combat.main.GetTroopsInSide(!table.attackingSide);
        }
    }
}
