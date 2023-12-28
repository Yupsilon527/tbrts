using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : ItemContainer
{
    Hero hero;
    public Resource Gold;
    protected override void Awake()
    {
        base.Awake();
        Gold = new Resource(0, name + " gold", false, false);

        if (hero == null)
        hero = GetComponent<Hero>();
    }

    public void EquiptItem(Item item)
    {
        if (item.data is EquipmentData gear)
        {
            hero.equipment.RemoveItem(gear.Slot, out ItemData oldItem);

            GiveItemFromData(oldItem);

            hero.equipment.EquiptItem(gear);

            inventory.Remove(item);
        }
    }
    #region Item Stack Amounts
    public int GetItemStackAmount(string itemII, bool checkStorage)
    {
        int stacks = GetItemStackAmount(itemII);
        if (checkStorage && hero.IsPlayerControlled())
        {
            stacks += PlayerController.main.sharedInventory.GetItemStackAmount(itemII);
        }
        return stacks;
    }
    #endregion

}
