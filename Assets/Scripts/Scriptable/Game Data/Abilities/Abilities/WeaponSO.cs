using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Abilities/Weapon")]
public class WeaponSO : ActionSO
{
    public WeaponData data;
    public ModifierSO innate;
    public CombatDefines.AttackFlag[] weaponFlags;

    private void OnValidate()
    {
        int flags = 0;
        foreach (var flag in weaponFlags)
        {
            flags |= (int)flag;
        }
        data.abilityFlags = flags;

        data.innateModifier = (ModifierData)innate.Translate();
    }
}
[Serializable]
public class WeaponData : ActionData
{
    public int apCost, mpCost, spCost, castTime, abilityFlags;
    public bool castOnce = false;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.ArmyPriorityMode targetPriority;
    public CombatDefines.CombatantRangeMode rangeMode;
    public CombatDefines.CombatantTargetingArea areaMode;

    public enum AuraType
    {
        self,
        troop,
        aura,
    }
    public AuraType innateType;
    public ModifierData innateModifier;
}
