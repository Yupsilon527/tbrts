using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Abilities/Weapon")]
public class WeaponSO : ActionSO
{
    public WeaponData data;
    public CombatDefines.AttackFlag[] weaponFlags;

    private void OnValidate()
    {
        int flags = 0;
        foreach (var flag in weaponFlags)
        {
            flags |= (int)flag;
        }
        data.abilityFlags = flags;
    }

    public WeaponData Translate()
    {
        WeaponData output = data.Clone() as WeaponData;
        output.effects = effects.Select(x => x.Translate()).ToArray();
        return output;
    }
}
[Serializable]
public class WeaponData : ActionData
{
    public int apCost, mpCost, spCost, castTime, abilityFlags;
    public bool castOnce = false;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.ArmyPriorityMode targetPriority;
    public CombatDefines.CombatantTargetingArea areaMode;

    public bool HasFlag(CombatDefines.AttackFlag flag)
    {
        return (abilityFlags & (int)flag) != 0;
    }

}
