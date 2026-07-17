using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropertySpell : PropertyAbility
{
    public SpellData original;
    public override string ToString()
    {
        return original.InternalName;
    }
    public PropertySpell(DataItemUnit caster, SpellData original) : base(caster)
    {
        InternalName = original.InternalName;
        this.original = original;
    }

    public override bool CastFromTable(CastTable table)
    {
        if (original.targetMode == CombatDefines.AbilityCastMode.random_tile)
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
            HashSet<DataItemBanner> affectedArmies = new();
            table.attacker.FireEventOnSelf(AbilityDefines.Event.CastSpell);

            foreach (var attack in original.effects)
            {
                attack.Activate(table);
            }
            foreach (var target in table.maintarget)
            {
                target.damageable.ResolveDamate();
                table.attacker.FireEventOnTarget(AbilityDefines.Event.OnHitSpell, target);
                target.FireEventOnTarget(AbilityDefines.Event.OnHitBySpell, table.attacker);
                affectedArmies.Add(target.troop);
            }
            foreach (var target in table.sidetarget)
            {
                target.damageable.ResolveDamate();
                table.attacker.FireEventOnTarget(AbilityDefines.Event.OnHitSpell, target);
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
        if (original.HasFlag(CombatDefines.SpellFlag.mustNotHaveActed) && parent.troop.movement.movedThisTurn) return false;
        
        var playerOwner = parent.GetPlayerOwner();
        return parent.actions.SupplyPoints.GetValue() >= original.SupplyCost
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Metal, original.MetalCost))
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Gold, original.GoldCost))
        && playerOwner.econ.CanAffordResource(new ResourceCost(EconomyDefines.EconomyResource.Mana, original.ManaCost));
    }
    public override void SpendResources()
    {
        parent.actions.SupplyPoints.SubstractedValue(original.SupplyCost);

        var playerOwner = parent.GetPlayerOwner();
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Metal, original.MetalCost));
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Gold, original.GoldCost));
        playerOwner.econ.Spend(new ResourceCost(EconomyDefines.EconomyResource.Mana, original.ManaCost));

        if (original.HasFlag(CombatDefines.SpellFlag.mustNotHaveActed))
            parent.troop.Exhaust();

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
        if (SidewaysMap.main?.GetTile(table.targetPoint)?.armyLayer is DataItemBanner targetArmy
            && ((targetArmy.GetAlignment(parent) == PlayerDefines.Alignment.enemy && original.HasFlag(CombatDefines.SpellFlag.targetEnemies))
            || (targetArmy.GetAlignment(parent) != PlayerDefines.Alignment.enemy && original.HasFlag(CombatDefines.SpellFlag.targetAllies))))
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
            if (tile.armyLayer != null
                && ((tile.armyLayer.GetAlignment(parent) == PlayerDefines.Alignment.enemy && original.HasFlag(CombatDefines.SpellFlag.targetEnemies))
            || (tile.armyLayer.GetAlignment(parent) != PlayerDefines.Alignment.enemy && original.HasFlag(CombatDefines.SpellFlag.targetAllies))))
                foreach (var army in tile.armyLayer.formation.GetUnits())
                    targets.Add(army);
        }
        return targets.ToArray();
    }
    #endregion
    public bool InstantCast()
    {
        return original.targetMode == CombatDefines.AbilityCastMode.self || original.targetMode == CombatDefines.AbilityCastMode.random_tile;
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
        Vector2Int delta = origin.gridPos - point.gridPos;
        switch (original.rangeMode)
        {
            case CombatDefines.TileTargetingMode.circle:
                return delta.sqrMagnitude <= original.max_range * original.max_range;
            case CombatDefines.TileTargetingMode.square:
                return Mathf.Abs(delta.x) <= original.max_range && Mathf.Abs(delta.y) <= original.max_range;
            case CombatDefines.TileTargetingMode.cross:
                return (Mathf.Abs(delta.x) <= original.max_range && delta.y == 0) || (Mathf.Abs(delta.y) <= original.max_range && delta.x == 0);
            case CombatDefines.TileTargetingMode.diagcross:
                return Mathf.Abs(delta.x) <= original.max_range && Mathf.Abs(delta.y) <= original.max_range && Mathf.Abs(delta.x)== Mathf.Abs(delta.y);
            case CombatDefines.TileTargetingMode.cross8:
                return (Mathf.Abs(delta.x) <= original.max_range && delta.y == 0) || (Mathf.Abs(delta.y) <= original.max_range && delta.x == 0)
                || Mathf.Abs(delta.x) <= original.max_range && Mathf.Abs(delta.y) <= original.max_range && Mathf.Abs(delta.x) == Mathf.Abs(delta.y);


            default://self, random tile
                return true;
        }
    }

    public DataItemTile[] GetValidCastTiles()
    {
        List<DataItemTile> staticCastTiles = new List<DataItemTile>();
        var centerTile = parent.GetOccupiedTiles()[0];

        staticCastTiles.AddRange(SidewaysMap.main.GetTilesInIrect(new (centerTile.gridPos,Mathf.CeilToInt(GetMaxRange()*2)*Vector2Int.one)));
        staticCastTiles.RemoveAll(t => !CanCastOnTile(t) );
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
