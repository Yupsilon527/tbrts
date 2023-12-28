using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentComponent : AbilityBaseComponent
{
    #region Base Functions
    List<PropertyAbility> available = new List<PropertyAbility>();
    public override PropertyAbility[] GetAvailableAbilities(bool castable)
    {
        available.Clear();
        foreach (PropertyItem ability in equippedItems)
        {
            if (ability == null)
                continue;
            if (!castable || ability.IsFullyCastable())
                available.Add(ability as PropertyAbility);

        }
        return available.ToArray();
    }
   

    public override void InitAbilities()
    {
        equippedItems = new PropertyItem[(int)ItemDefines.EquipSlots.Total];
    }
    #endregion

    PropertyItem[] equippedItems;
    #region Give Items
    public void EquiptItem(EquipmentData item)
    {
        if (item == null) return;
        if (TryGetItemInSlot(item.Slot, out PropertyItem found))
        {
            print($"[ItemComponent] Give item {item.name} failure in slot in {item.Slot}");
        }
        else
        {
            print("[ItemComponent] Give item " + item.name);
            PropertyItem genItem = new PropertyItem(parent, item);
            AddAbility(genItem);
            print("[ItemComponent] Give item success in slot " + item.Slot);
        }
    }
    public void AddAbility(PropertyItem ability)
    {
        equippedItems[(int)ability.originalItem.GetItemSlot()] = ability;
        AddAbility(ability);
    }

    public void RemoveAbility(PropertyItem ability)
    {
        equippedItems[(int)ability.originalItem.GetItemSlot()] = null;
        RemoveAbility(ability);
    }
    #endregion
    #region Remove Items
    public void RemoveItem(ItemDefines.EquipSlots equipmentSlot, out ItemData outItem)
    {
        outItem = null;

        PropertyItem currentItem = equippedItems[(int)equipmentSlot];
        if (currentItem != null) {
            outItem = currentItem.originalItem;
        }
        equippedItems[(int)equipmentSlot] = null;
    }
    #endregion
    #region Find Items
    public bool TryGetItemInSlot(ItemDefines.EquipSlots slot, out PropertyItem found)
    {
        found = equippedItems[(int)slot];
        return found != null;
    }
    #endregion
}
