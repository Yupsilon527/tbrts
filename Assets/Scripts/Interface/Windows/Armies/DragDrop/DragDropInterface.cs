using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDragDropInterface : Initializable
{
    public DataItemArmy unitA, unitB;
    #region DD Slots
    public DragDropSlot[] UnitSlots;
    public DragDropSlot[] DiscardSlots;

    void FindSlots()
    {
        UnitSlots = GetComponentsInChildren<DragDropSlot>();
    }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        if (TokenPool == null)
            TokenPool = GetComponent<ObjectPool>();
        FindSlots();
    }
    public void InitSlots(DataItemArmy a, DataItemArmy b)
    {
        if (UnitSlots == null) return;
        unitA = a;
        unitB = b;

        foreach (var slot in UnitSlots)
        {
            slot.army = slot.left ? a : b;

            slot.gameObject.SetActive(slot.army != null);

            if (slot.isActiveAndEnabled)
            {
                DataItemUnit unit = slot.position < 0 ? slot.army.formation.transport : slot.army.formation.Formation[slot.position];

                if (unit != null)
                {
                    var token = GenerateToken(unit);
                    token.parent = this;
                    token.AttachToSlot(slot, true);
                }
            }
        }
    }
    #region Token Pool
    public GameObject TokenPrefab;
    public ObjectPool TokenPool;

    public DragDropToken GenerateToken(DataItemUnit u)
    {
        GameObject parent = TokenPool.PoolItem(TokenPrefab);
        DragDropToken token = parent.GetComponent<DragDropToken>();
        token.parent = this;
        token.FromUnit(u, true);
        return token;
    }
    #endregion
    #region Hero Init
    public void ApplyChanges()
    {
        foreach (var slot in UnitSlots)
        {
            slot.army?.formation.SetTroopInPosition(slot.position, slot.attachedToken != null ? slot.attachedToken.tokenUnit : null);
        }

        unitA?.Revise();
        unitB?.Revise();
    }
    public void Clear()
    {
        foreach (DragDropSlot token in UnitSlots)
            token.DeleteToken();
    }
    #endregion
}