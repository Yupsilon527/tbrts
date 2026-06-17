using UnityEngine;

public class PropertyWeapon : PropertyAbility
{
    public WeaponData original;
    public override bool IsUsable()
    {
        return base.IsUsable();
    }
    public override bool HasResourcesToCast()
    {
        return base.HasResourcesToCast();
    }
    public override void SpendResources()
    {
        base.SpendResources();
    }
}
