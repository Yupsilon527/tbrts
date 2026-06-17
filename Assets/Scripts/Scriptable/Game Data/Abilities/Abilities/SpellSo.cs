using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Abilities/Spell")]
public class SpellSo : ScriptableObject
{
    public SpellData data;
}
[Serializable]
public class SpellData
{
    public int GoldCost = 0;
    public int ManaCost = 0;

    public int min_range, max_range, area_range;

    public CombatDefines.TileTargetingMode targetMode;
    public CombatDefines.TileRangeMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;

}

