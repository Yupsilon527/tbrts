using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Abilities/Weapon")]
public class WeaponSO : ActionSO
{
    public WeaponData data;
}
[Serializable]
public class WeaponData : ActionData
{
    public int apCost, mpCost, castTime;
    public bool castOnce = false;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.ArmyPriorityMode targetPriority;
    public CombatDefines.ArmyRangeMode rangeMode;
    public CombatDefines.ArmyTargetingArea areaMode;
}
