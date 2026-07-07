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
        data.InternalName = name;
    }

    public WeaponData Translate()
    {
        WorldManager.main.Inspect("Unload data " + data.InternalName);
        WeaponData output = data.Clone() as WeaponData;
        output.effects = effects.Select(x => x.Translate()).ToArray();
        return output;
    }
}
[Serializable]
public class WeaponData : ActionData
{
    public int apCost, rpCost, spCost, castTime, castDelay;
    public CombatDefines.AttackPhase attackPhase;

    public CombatDefines.ArmyPriorityMode targetPriority;
    public CombatDefines.CombatantTargetingArea areaMode;

    public bool HasFlag(CombatDefines.AttackFlag flag)
    {
        return (abilityFlags & (int)flag) != 0;
    }

}
