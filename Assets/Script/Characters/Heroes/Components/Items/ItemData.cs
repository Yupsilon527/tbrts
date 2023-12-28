using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemData", menuName = "IdleRPG/Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string InternalName;
    public ItemDefines.StackType itemType;
    public Sprite itemSprite;

    public virtual ItemDefines.EquipSlots GetItemSlot ()
    {
        return ItemDefines.EquipSlots.Unequiptable;
    }
}
