using UnityEngine;

public class PropertySpell : PropertyAbility
{
    public SpellData original;


    #region Resource
    public override bool HasResourcesToCast()
    {
        return base.HasResourcesToCast();
    }
    public override void SpendResources()
    {
        base.SpendResources();
    }
    #endregion
    #region Range
    public float GetMinRange()
    {
        return Mathf.Max(GetAreaRange(), original.min_range);
    }

    public float GetMaxRange()
    {
        return Mathf.Max(0, original.max_range + parent.GetPropertyAdditive(ModifierDefines.Property.ability_cast_range));
    }
    public float GetAreaRange()
    {
        return Mathf.Max(0, original.area_range + parent.GetPropertyAdditive(ModifierDefines.Property.ability_aoe_range));
    }

    public override DataItemUnit[] GetMainTargets(CastTable table)
    {
        throw new System.NotImplementedException();
    }

    public override DataItemUnit[] GetSideTargets(CastTable table)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
