using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyItem : PropertyAbility
{
    public ItemData originalItem;
    public PropertyItem(Mob owner, EquipmentData item) : base(owner, item.ItemAbility)
    {
        originalItem = item;
    }

    public PropertyItem(Mob owner, EquipmentData item, int itemStacks) : base(owner, item.ItemAbility)
    {
        originalItem = item;
        stacks = itemStacks;
    }
    #region Stacks
    public int stacks = 1;
    public int GetStackCount()
    {
        return stacks;
    }
    public void SetStackCount(int value)
    {
        stacks = value;
        FireEvent(AbilityDefines.Event.OnUpgrade, caster);
    }
    public void IncrementStackCount()
    {
        SetStackCount(stacks + 1);
    }
    public void DecrementStackCount()
    {
        SetStackCount(stacks - 1);
    }
    #endregion
}
