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
        if (original.targetMode == CombatDefines.TileTargetingMode.random_tile)
        {
            var hitTiles = GetValidCastTiles();
            if (hitTiles.Length > 0)
            {
                var randomTile = hitTiles[Mathf.FloorToInt(hitTiles.Length * UnityEngine.Random.value)];
                table.targetPoint = randomTile.gridPos;
            }
            else return false;
        }


        if (base.CastFromTable(table))
        {
            table.ComputeTargets();
            HashSet<DataItemArmy> affectedArmies = new();
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
                affectedArmies.Add(target.troop);
            }
            foreach (var target in table.sidetarget)
            {
                target.damageable.ResolveDamate();
                table.attacker.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, target);
                target.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, table.attacker);
                affectedArmies.Add(target.troop);
            }
            foreach (var army in affectedArmies)
            {
                army.status.ResolvePendingStatuses();
            }
            return true;
        }
        return false;
    }

    #region Resource
    public override bool HasResourcesToCast()
    {
        var playerOwner = parent.GetPlayerOwner();
        return parent.actions.Sp.GetValue() >= original.SupplyCost
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Metal, original.MetalCost))
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Gold, original.GoldCost))
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Mana, original.ManaCost));
    }
    public override void SpendResources()
    {
        parent.actions.Sp.SubstractedValue(original.SupplyCost);

        var playerOwner = parent.GetPlayerOwner();
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Metal, original.MetalCost));
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Gold, original.GoldCost));
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Mana, original.ManaCost));

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
        foreach (var tile in GetHitTiles(SidewaysMap.main.GetTile(table.targetPoint)))
        {
            if (tile.armyLayer != null)
                foreach (var army in tile.armyLayer.formation.GetUnits())
                        targets.Add(army);
        }
        return targets.ToArray();
    }
    #endregion
    public bool InstantCast()
    {
        return original.targetMode == CombatDefines.TileTargetingMode.self || original.targetMode == CombatDefines.TileTargetingMode.random_tile;
    }
    public bool IsInCastRange(DataItemTile point)
    {
        DataItemTile origin = parent.GetOccupiedTiles()[0];
        float rangeSqrt = (origin.gridPos - point.gridPos).sqrMagnitude;
        return rangeSqrt > original.min_range * original.min_range && rangeSqrt < original.max_range * original.max_range;
    }
    public bool CanCastOnTile(DataItemTile point)
    {
        DataItemTile origin = parent.GetOccupiedTiles()[0];
        switch (original.targetMode)
        {
            default://self
                return true;
            case CombatDefines.TileTargetingMode.passive:
                return false;
            case CombatDefines.TileTargetingMode.direction:
                return (Mathf.Abs(origin.gridPos.x - point.gridPos.x) == 1 && origin.gridPos.y == point.gridPos.y) || (Mathf.Abs(origin.gridPos.y - point.gridPos.y) == 1 && (origin.gridPos.x == point.gridPos.x));
            case CombatDefines.TileTargetingMode.direction8:
                return Mathf.Abs(origin.gridPos.x - point.gridPos.x) == 1 || Mathf.Abs(origin.gridPos.y - point.gridPos.y) == 1;
            case CombatDefines.TileTargetingMode.circle:
                return IsInCastRange(point);
        }
    }

    public DataItemTile[] GetValidCastTiles()
    {
        List<DataItemTile> staticCastTiles = new List<DataItemTile>();
        var centerTile = parent.GetOccupiedTiles()[0];
        switch (original.targetMode)
        {
            default:
                staticCastTiles.AddRange(parent.GetOccupiedTiles());
                break;
            case CombatDefines.TileTargetingMode.direction:
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.up));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.down));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.left));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.right));
                break;
            case CombatDefines.TileTargetingMode.direction8:
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.up));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.down));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.left));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.right));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.upright));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.upleft));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.downright));
                staticCastTiles.Add(centerTile.GetNeighbor(DataItemTile.GridDirection.downleft));
                break;
            case CombatDefines.TileTargetingMode.random_tile:
            case CombatDefines.TileTargetingMode.circle:
                staticCastTiles.AddRange(SidewaysMap.main.GetTilesInCircle(centerTile.gridPos, (int)GetMaxRange()));

                int minRange = (int)GetMinRange();
                staticCastTiles.RemoveAll(t => (t.gridPos - centerTile.gridPos).sqrMagnitude < minRange * minRange);
                break;
        }
        staticCastTiles.RemoveAll(t => t == null);
        return staticCastTiles.ToArray();
    }

    public DataItemTile[] GetHitTiles(DataItemTile targetTile)
    {
        List<DataItemTile> staticCastTiles = new List<DataItemTile>();
        DataItemTile centerTile = parent.GetOccupiedTiles()[0];
        int areaRange = (int)GetMaxRange();
        switch (original.areaMode)
        {
            case CombatDefines.TileTargetingArea.tile:
                staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos));
                break;
            case CombatDefines.TileTargetingArea.circle:
                staticCastTiles.AddRange(SidewaysMap.main.GetTilesInCircle(targetTile.gridPos, areaRange));
                break;
            case CombatDefines.TileTargetingArea.square:
                staticCastTiles.AddRange(SidewaysMap.main.GetTilesInRect(targetTile.gridPos - Vector2Int.one * areaRange, targetTile.gridPos + Vector2Int.one * areaRange));
                break;
            case CombatDefines.TileTargetingArea.cross:
                for (int i = 0; i < areaRange; i++)
                {
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + Vector2Int.up * i));
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + Vector2Int.down * i));
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + Vector2Int.left * i));
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + Vector2Int.right * i));
                }
                break;
            case CombatDefines.TileTargetingArea.cone:
                Vector2Int dir = targetTile.gridPos - centerTile.gridPos;
                Vector2Int ang = Vector2Int.zero;
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    dir = dir.x > 0 ? Vector2Int.right : Vector2Int.left;
                    ang = Vector2Int.up;
                }
                else
                {
                    dir = dir.y > 0 ? Vector2Int.down : Vector2Int.up;
                    ang = Vector2Int.right;
                }
                for (int i = 0; i < areaRange; i++)
                {
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i));
                    for (int j = 0; j < i; j++)
                    {
                        staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i + ang * j));
                        staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i - ang * j));
                    }
                }
                break;
            case CombatDefines.TileTargetingArea.cone_narrow:
                dir = targetTile.gridPos - centerTile.gridPos;
                ang = Vector2Int.zero;
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    dir = dir.x > 0 ? Vector2Int.right : Vector2Int.left;
                    ang = Vector2Int.up;
                }
                else
                {
                    dir = dir.y > 0 ? Vector2Int.down : Vector2Int.up;
                    ang = Vector2Int.right;
                }
                for (int i = 0; i < areaRange; i++)
                {
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i));
                    for (int j = 0; j < i; j++)
                    {
                        staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i + ang * j / 2));
                        staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i - ang * j / 2));
                    }
                }
                break;
            case CombatDefines.TileTargetingArea.line:
                dir = targetTile.gridPos - centerTile.gridPos;
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    dir = dir.x > 0 ? Vector2Int.right : Vector2Int.left;
                }
                else
                {
                    dir = dir.y > 0 ? Vector2Int.down : Vector2Int.up;
                }
                for (int i = 0; i < areaRange; i++)
                {
                    staticCastTiles.Add(SidewaysMap.main.GetTile(targetTile.gridPos + dir * i));
                }
                break;
            default:
                staticCastTiles.Add(centerTile);
                break;
        }
        staticCastTiles.RemoveAll(t => t == null);
        return staticCastTiles.ToArray();
    }
}
