using UnityEngine;

public class PropertyWeapon : PropertyAbility
{
    public int apCost, mpCost, castTime;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.TileTargetingMode targetMode;
    public CombatDefines.TileRangeMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;
}
