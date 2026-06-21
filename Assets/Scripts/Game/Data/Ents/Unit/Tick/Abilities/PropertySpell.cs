using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropertySpell : PropertyAbility
{
    public SpellData original;
    public PropertySpell(DataItemUnit caster, SpellData original) : base(caster)
    {
        InternalName = original.InternalName;
        this.original = original;
    }

    public override bool CastFromTable(CastTable table)
    {
        if (base.CastFromTable(table))
        {
            table.ComputeTargets();
            table.Precast();
            table.attacker.FireEventOnSelf(AbilityDefines.Event.CastSpell);
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

    #region Resource
    public override bool HasResourcesToCast()
    {
        return base.HasResourcesToCast();
    }
    public override void SpendResources()
    {
        base.SpendResources();
    }
    #endregion
    #region Range
    public float GetMinRange()
    {
        return Mathf.Max(GetAreaRange(), original.min_range);
    }

    public float GetMaxRange()
    {
        return Mathf.Max(0, original.max_range + parent.GetPropertyAdditive(ModifierDefines.Property.ability_cast_range));
    }
    public float GetAreaRange()
    {
        return Mathf.Max(0, original.area_range + parent.GetPropertyAdditive(ModifierDefines.Property.ability_aoe_range));
    }

    public override DataItemUnit[] GetMainTargets(CastTable table)
    {
        if (SidewaysMap.main?.GetTile(table.targetPoint)?.locatedArmy is DataItemArmy targetArmy)
        {
            return targetArmy.formation.GetUnits();
        }
        return Array.Empty<DataItemUnit>();
    }

    public override DataItemUnit[] GetAreaTargets(CastTable table)
    {
        HashSet<DataItemUnit> targets = new HashSet<DataItemUnit>();
        int arange = (int)GetAreaRange();
        switch (original.areaMode)
        {
            case CombatDefines.TileTargetingArea.circle:
                var checkTiles = SidewaysMap.main.GetTilesInCircle(table.targetPoint, arange);
                foreach (var tile in checkTiles)
                {
                    if (tile.locatedArmy != null)
                        foreach (var army in tile.locatedArmy.formation.GetUnits())
                            targets.Add(army);
                }
                break;
            case CombatDefines.TileTargetingArea.square:
                var checkRect = SidewaysMap.main.GetTilesInRect(new RectInt(table.targetPoint.x - arange, table.targetPoint.y - arange, table.targetPoint.x + arange, table.targetPoint.y + arange));
                foreach (var tile in checkRect)
                {
                    if (tile.locatedArmy != null)
                        foreach (var army in tile.locatedArmy.formation.GetUnits())
                            targets.Add(army);
                }
                break;
        }
        return targets.ToArray();
    }
    #endregion
}
