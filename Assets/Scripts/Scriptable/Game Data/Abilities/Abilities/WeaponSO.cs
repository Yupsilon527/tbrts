using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Abilities/Weapon")]
public class WeaponSO : ScriptableObject
{
    public WeaponData data;
}
[Serializable]
public class WeaponData
{
    public int apCost, mpCost, castTime;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.TileTargetingMode targetMode;
    public CombatDefines.TileRangeMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;

}
