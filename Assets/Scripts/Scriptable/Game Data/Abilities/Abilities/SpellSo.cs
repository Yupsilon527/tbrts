using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Abilities/Spell")]
public class SpellSo : ActionSO
{
    public SpellData data;
}
public class ActionSO : ScriptableObject
{
    public AttackEffectSO[] effects;
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

public class ActionData : BaseData
{
    public ApplyEffects[] effects;
}

