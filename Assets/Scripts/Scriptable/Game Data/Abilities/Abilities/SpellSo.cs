using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Abilities/Spell")]
public class SpellSo : ActionSO
{
    public SpellData data;
    public SpellData Translate()
    {
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

    public CombatDefines.TileTargetingMode targetMode;
    public CombatDefines.TileAreaMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;

}

