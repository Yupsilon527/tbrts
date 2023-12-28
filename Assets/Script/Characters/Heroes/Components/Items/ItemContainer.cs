using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public class Item
    {
        public ItemData data;
         int stackAmount;

        public Item(ItemData d)
        {
            data = d;
            stackAmount = 1;
        }
        public Item(ItemData d, int s)
        {
            data = d;
            stackAmount = s;
        }
        public void IncrementStackAmount(int val)
        {
            SetStackAmount(stackAmount + val);
        }
        public void SetStackAmount(int val)
        {
            stackAmount = val;
            OnValueChanged.Invoke();
        }
        public int GetStackAmount()
        {
            return stackAmount;
        }

        public UnityEngine.Events.UnityEvent OnValueChanged = new UnityEngine.Events.UnityEvent();
        public override string ToString()
        {
            return $"Item {data.name} ({stackAmount})";
        }
    }
    public List<Item> inventory;
    protected virtual void Awake()
    {
        inventory = new List<Item>();
    }
    public bool GiveItemFromData(ItemData fromData)
    {

        return GiveItemFromData(fromData, 1);
    }
    public bool GiveItemFromData(ItemData fromData, int amount)
    {
        if (fromData == null) return true;
        foreach (Item item in inventory)
        {
            int limit = GetStackLimit(fromData.itemType);
            if (item.data.name == fromData.name && item.GetStackAmount() < limit)
            {
                int delta = Mathf.Min(amount, limit - item.GetStackAmount());
                item.IncrementStackAmount (delta);
                amount -= delta;

                Debug.Log($"Give {delta} {fromData.name}s to {name}");
                if (amount <= 0)
                {
                    return true;
                }
            }
        }

        return GiveItem(new Item(fromData, amount));
    }
    public bool GiveItem(Item item)
    {
        if (item != null && item.GetStackAmount() > 0 && inventory.Count < GetInventoryLimit())
        {
            Debug.Log($"Give item {item.data.name} to {name}");
            inventory.Add(item);
            return true;
        }
        return false;
    }
    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }
    public virtual int GetInventoryLimit()
    {
        return 100;
    }
    public int GetStackLimit(ItemDefines.StackType stype)
    {
        return stype == ItemDefines.StackType.unique ? 1 : 100;
    }
    public int GetItemStackAmount(string itemII)
    {
        int stacks = 0;
        foreach (Item item in inventory)
        {
            if (item.data.InternalName == itemII)
            {
                stacks += item.GetStackAmount();
            }
        }
        return stacks;
    }
    public void WithdrawItemStackAmount(string itemII, int amount, out int removed)
    {
        removed = 0;
        foreach (Item item in inventory)
        {
            if (item.data.InternalName == itemII)
            {
                int delta = Mathf.Min(amount - removed, item.GetStackAmount());
                removed += delta;

                item.IncrementStackAmount(-delta);
            }
            if (removed == amount)
                break;
        }
        inventory.RemoveAll((Item item) => { return item.GetStackAmount() == 0; });
    }

}
