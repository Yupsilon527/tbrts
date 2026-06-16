using UnityEngine;
using static UnityEngine.UI.Image;

public class PropertySpell : PropertyAbility
{
    public int GoldCost = 0;
    public int ManaCost = 0;

    public CombatDefines.TileTargetingMode targetMode ;
    public CombatDefines.TileRangeMode rangeMode;
    public CombatDefines.TileTargetingArea areaMode;


    public float GetMinRange(bool raw)
    {
        if (!raw && GetAreaRange(true) > 0)
            return Mathf.Max(GetAreaRange(false), original.GetMinRange());
        return original.GetMinRange();
    }

    public float GetMaxRange(bool raw)
    {
        float range = original.GetMaxRange();
        if (!raw)
        {
            range = Mathf.Max(0, range + caster.GetPropertyAdditive(ModifierDefines.modProps.ability_cast_range) + (IsBasicAttack() ? caster.stats.realStats.AttackRange : 0));
        }
        return range;
    }
    public int GetAreaRange(bool raw)
    {
        float range = original.GetAoERange();
        if (!raw && caster.modifiers != null)
        {
            range = Mathf.Max(1, range + caster.GetPropertyAdditive(ModifierDefines.modProps.ability_aoe_range));
        }
        return Mathf.RoundToInt(range);
    }
}
