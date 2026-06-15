
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using UnityEngine;
using System.Linq;

public class UnitGroup<DataItemType> : Collection<DataItemType> where DataItemType : DataItemObject
{
    // --- Events ---

    /// <summary>Fired after a Mob is added. Arg: the mob that was added.</summary>
    public event Action<DataItemType> OnUnitAdded;

    /// <summary>Fired after a Mob is removed. Arg: the mob that was removed.</summary>
    public event Action<DataItemType> OnUnitRemoved;

    /// <summary>Fired after any structural change (add, remove, clear, set).</summary>
    public event Action OnGroupContentsChange;

    // -------------------------------------------------------
    // Collection<T> override points — all mutations flow here

    protected override void InsertItem(int index, DataItemType item)
    {
        if (!Contains(item))
        {
            base.InsertItem(index, item);
            item.RegisterGroup(this as UnitGroup<DataItemObject>);
            OnUnitAdded?.Invoke(item);
            OnGroupContentsChange?.Invoke();
        }
    }

    protected override void RemoveItem(int index)
    {
        DataItemType removed = this[index];
        base.RemoveItem(index);
        removed.UnregisterGroup(this as UnitGroup<DataItemObject>);
        OnUnitRemoved?.Invoke(removed);
        OnGroupContentsChange?.Invoke();
    }

    protected override void SetItem(int index, DataItemType item)
    {
        DataItemType previous = this[index];
        base.SetItem(index, item);
        previous.UnregisterGroup(this as UnitGroup<DataItemObject>);
        item.RegisterGroup(this as UnitGroup<DataItemObject>);
        OnUnitRemoved?.Invoke(previous);
        OnUnitAdded?.Invoke(item);
        OnGroupContentsChange?.Invoke();
    }

    protected override void ClearItems()
    {
        var snapshot = new List<DataItemType>(this);
        base.ClearItems();
        foreach (var mob in snapshot)
        {
            mob.UnregisterGroup(this as UnitGroup<DataItemObject>);
            OnUnitRemoved?.Invoke(mob);
        }
        OnGroupContentsChange?.Invoke();
    }

    // -------------------------------------------------------
    // Convenience helpers

    /// <summary>Add multiple units at once; fires one OnGroupContentsChange at the end.</summary>
    public void AddRange(IEnumerable<DataItemType> mobs)
    {
        foreach (var mob in mobs)
        {
            base.InsertItem(Count, mob);
            mob.RegisterGroup(this as UnitGroup<DataItemObject>);
            OnUnitAdded?.Invoke(mob);
        }
        OnContentsUpdate();
    }

    /// <summary>Remove all mobs matching a predicate.</summary>
    public int RemoveWhere(Func<DataItemType, bool> predicate)
    {
        var toRemove = new List<DataItemType>();
        foreach (var mob in this)
            if (predicate(mob)) toRemove.Add(mob);

        foreach (var mob in toRemove)
        {
            int idx = IndexOf(mob);
            if (idx >= 0)
            {
                base.RemoveItem(idx);
                mob.UnregisterGroup(this as UnitGroup<DataItemObject>);
                OnUnitRemoved?.Invoke(mob);
            }
        }

        if (toRemove.Count > 0) OnContentsUpdate();

        return toRemove.Count;
    }
    public void OnContentsUpdate()
    {
        OnGroupContentsChange?.Invoke();
    }

    public DataItemType[] Filter(DataItemPlayer player, PlayerDefines.Alignment alignment)
    {
        return this
            .Where(m => m != null && player.GetAlignment(m.GetPlayerOwner()) == alignment)
            .ToArray();
    }


    public DataItemType[] FindInCircle(Vector2Int center, int radius,
                               DataItemPlayer player, PlayerDefines.Alignment alignment)
    {
        float sqrRadius = radius * radius;
        return Filter(player, alignment)
            .Where(m => ((Vector2)m.gridPos - center).sqrMagnitude <= sqrRadius)
            .ToArray();
    }

    public DataItemType FindNearestInCircle(Vector2Int center, int radius,
                                    DataItemPlayer player, PlayerDefines.Alignment alignment)
    {
        float sqrRadius = radius * radius;
        DataItemType nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var mob in Filter(player, alignment))
        {
            float sqrDist = ((Vector2)mob.gridPos - center).sqrMagnitude;
            if (sqrDist <= sqrRadius && sqrDist < nearestSqr)
            {
                nearestSqr = sqrDist;
                nearest = mob;
            }
        }
        return nearest;
    }

    /// <summary>
    /// All live units within an axis-aligned box, filtered by alignment.
    /// </summary>
    public DataItemType[] FindInRect(RectInt rect, DataItemPlayer player, PlayerDefines.Alignment alignment)
    {
        return Filter(player, alignment)
            .Where(m => rect.Contains(m.gridPos))
            .ToArray();
    }
}