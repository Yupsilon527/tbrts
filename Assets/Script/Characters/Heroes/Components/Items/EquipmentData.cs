using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GearData", menuName = "IdleRPG/Items/GearData")]
public class EquipmentData : ItemData
{
    public ItemDefines.EquipSlots Slot;
    public AbilitySO ItemAbility;
    public override ItemDefines.EquipSlots GetItemSlot()
    {
        return Slot;
    }
}
