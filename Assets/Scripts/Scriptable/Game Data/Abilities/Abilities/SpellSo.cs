using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Abilities/Spell")]
public class SpellSO : ActionSO
{
    public SpellData data;
    public CombatDefines.SpellFlag[] spellFlags;

    private void OnValidate()
    {
        int flags = 0;
        foreach (var flag in spellFlags)
        {
            flags |= (int)flag;
        }
        data.abilityFlags = flags;
        data.InternalName = name;
    }

    public override ActionData Translate()
    {
        WorldManager.main.Inspect("Unload data " + data.InternalName);
        SpellData output = data.Clone() as SpellData;
        output.effects = effects.Select(x => x.Translate()).ToArray();
        return output;
    }

}
[Serializable]
public class SpellData : ActionData
{
    public int MetalCost = 0;
    public int GoldCost = 0;
    public int ManaCost = 0;
    public int SupplyCost = 0;

    public int min_range, max_range, area_range;

    public CombatDefines.AbilityCastMode targetMode;
    public CombatDefines.TileTargetingMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;

    public bool HasFlag(CombatDefines.SpellFlag flag)
    {
        return (abilityFlags & (int)flag) != 0;
    }
}

