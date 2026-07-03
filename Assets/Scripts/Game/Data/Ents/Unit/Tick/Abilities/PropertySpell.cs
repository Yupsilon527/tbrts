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
        if (SidewaysMap.main?.GetTile(table.targetPoint)?.armyLayer is DataItemArmy targetArmy)
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
                    if (tile.armyLayer != null)
                        foreach (var army in tile.armyLayer.formation.GetUnits())
                            targets.Add(army);
                }
                break;
            case CombatDefines.TileTargetingArea.square:
                var checkRect = SidewaysMap.main.GetTilesInRect(new RectInt(table.targetPoint.x - arange, table.targetPoint.y - arange, table.targetPoint.x + arange, table.targetPoint.y + arange));
                foreach (var tile in checkRect)
                {
                    if (tile.armyLayer != null)
                        foreach (var army in tile.armyLayer.formation.GetUnits())
                            targets.Add(army);
                }
                break;
        }
        return targets.ToArray();
    }
    #endregion
    public bool IsInCastRange( DataItemTile point)
    {
        DataItemTile origin = parent.tile;
        float rangeSqrt = (origin.gridPos-point.gridPos).sqrMagnitude;
        return rangeSqrt > original.min_range * original.min_range && rangeSqrt < original.max_range * original.max_range;
    }
    public bool CanCastOnTile( DataItemTile point)
    {
        DataItemTile origin = parent.tile;
        switch (original.targetMode)
        {
            default://self
                return true;
            case CombatDefines.TileTargetingMode.passive:
                return false;
            case CombatDefines.TileTargetingMode.direction:
                return (Mathf.Abs(origin.gridPos.x - point.gridPos.x) == 1 && origin.gridPos.y == point.gridPos.y) ||( Mathf.Abs(origin.gridPos.y - point.gridPos.y) == 1 && (origin.gridPos.x == point.gridPos.x));
            case CombatDefines.TileTargetingMode.direction8:
                return Mathf.Abs(origin.gridPos.x - point.gridPos.x) == 1 || Mathf.Abs(origin.gridPos.y - point.gridPos.y) == 1;
            case CombatDefines.TileTargetingMode.circle:
                return IsInCastRange(point);
        }
    }

    public DataItemTile[] GetValidCastTiles()
    {

        List<DataItemTile> staticCastTiles = new List<DataItemTile>();
        switch (original.targetMode)
        {
            default:
                staticCastTiles.Add(parent.tile);
                break;
            case AbilityDefines.Behavior.adjencent:
                DataItemTile centerTile = DataItemWorld.main.GetTile(GridPosition);
                staticCastTiles.AddRange(centerTile.neighbors);
                break;
            case AbilityDefines.Behavior.line:
                Vector3Int cubeCenter = GridPosition.CubeCoords;
                int range = Mathf.RoundToInt(original.maxRange);

                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.right + Vector3Int.back, range));
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.left + Vector3Int.forward, range));
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.up + Vector3Int.back, range));
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.down + Vector3Int.forward, range));
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.right + Vector3Int.down, range));
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInLine(cubeCenter, Vector3Int.left + Vector3Int.up, range));

                staticCastTiles.RemoveAll((DataItemTile T) => { return T.coords.DistanceFrom(GridPosition) <= original.minRange; });
                break;
            case AbilityDefines.Behavior.circle:
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInCirc(GridPosition, Mathf.RoundToInt(original.maxRange)));
                staticCastTiles.RemoveAll((DataItemTile T) => { return T.coords.DistanceFrom(GridPosition) <= original.minRange; });
                break;
            case AbilityDefines.Behavior.point:
                staticCastTiles.AddRange(DataItemWorld.main.GetTilesInCirc(GridPosition, Mathf.RoundToInt(original.maxRange)));
                staticCastTiles.RemoveAll((DataItemTile T) => { return T.LocatedEntity == null && T.coords.DistanceFrom(GridPosition) <= original.minRange; });
                break;
        }
        return staticCastTiles.ToArray() ;
    }

    public DataItemTile[] GetHitTiles(DataItemTile GridPosition)
    {

    }
}
