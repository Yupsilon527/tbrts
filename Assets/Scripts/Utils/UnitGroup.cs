
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using UnityEngine;
using System.Linq;

public class UnitGroup<TMob> : Collection<TMob> where TMob : Mob
{
    // --- Events ---

    /// <summary>Fired after a Mob is added. Arg: the mob that was added.</summary>
    public event Action<TMob> OnUnitAdded;

    /// <summary>Fired after a Mob is removed. Arg: the mob that was removed.</summary>
    public event Action<TMob> OnUnitRemoved;

    /// <summary>Fired after any structural change (add, remove, clear, set).</summary>
    public event Action OnGroupContentsChange;

    // -------------------------------------------------------
    // Collection<T> override points — all mutations flow here

    protected override void InsertItem(int index, TMob item)
    {
        if (!Contains(item))
        {
            base.InsertItem(index, item);
            item.RegisterGroup(this as UnitGroup<Mob>);
            OnUnitAdded?.Invoke(item);
            OnGroupContentsChange?.Invoke();
        }
    }

    protected override void RemoveItem(int index)
    {
        TMob removed = this[index];
        base.RemoveItem(index);
        removed.UnregisterGroup(this as UnitGroup<Mob>);
        OnUnitRemoved?.Invoke(removed);
        OnGroupContentsChange?.Invoke();
    }

    protected override void SetItem(int index, TMob item)
    {
        TMob previous = this[index];
        base.SetItem(index, item);
        previous.UnregisterGroup(this as UnitGroup<Mob>);
        item.RegisterGroup(this as UnitGroup<Mob>);
        OnUnitRemoved?.Invoke(previous);
        OnUnitAdded?.Invoke(item);
        OnGroupContentsChange?.Invoke();
    }

    protected override void ClearItems()
    {
        var snapshot = new List<TMob>(this);
        base.ClearItems();
        foreach (var mob in snapshot)
        {
            mob.UnregisterGroup(this as UnitGroup<Mob>);
            OnUnitRemoved?.Invoke(mob);
        }
        OnGroupContentsChange?.Invoke();
    }

    // -------------------------------------------------------
    // Convenience helpers

    /// <summary>Add multiple units at once; fires one OnGroupContentsChange at the end.</summary>
    public void AddRange(IEnumerable<TMob> mobs)
    {
        foreach (var mob in mobs)
        {
            base.InsertItem(Count, mob);
            mob.RegisterGroup(this as UnitGroup<Mob>);
            OnUnitAdded?.Invoke(mob);
        }
        OnContentsUpdate();
    }

    /// <summary>Remove all mobs matching a predicate.</summary>
    public int RemoveWhere(Func<TMob, bool> predicate)
    {
        var toRemove = new List<TMob>();
        foreach (var mob in this)
            if (predicate(mob)) toRemove.Add(mob);

        foreach (var mob in toRemove)
        {
            int idx = IndexOf(mob);
            if (idx >= 0)
            {
                base.RemoveItem(idx);
                mob.UnregisterGroup(this as UnitGroup<Mob>);
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

    public TMob[] Filter(DataItemPlayer player, Mob.Alignment alignment)
    {
        return this
            .Where(m => m != null && player.GetAlignment(m.GetPlayerOwner()) == alignment)
            .ToArray();
    }


    public TMob[] FindInCircle(Vector2 center, float radius,
                               DataItemPlayer player, Mob.Alignment alignment)
    {
        float sqrRadius = radius * radius;
        return Filter(player, alignment)
            .Where(m => ((Vector2)m.transform.position - center).sqrMagnitude <= sqrRadius)
            .ToArray();
    }

    public TMob FindNearestInCircle(Vector2 center, float radius,
                                    DataItemPlayer player, Mob.Alignment alignment)
    {
        float sqrRadius = radius * radius;
        TMob nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var mob in Filter(player, alignment))
        {
            float sqrDist = ((Vector2)mob.transform.position - center).sqrMagnitude;
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
    public TMob[] FindInRect(Rect rect, DataItemPlayer player, Mob.Alignment alignment)
    {
        return Filter(player, alignment)
            .Where(m => rect.Contains(m.transform.position))
            .ToArray();
    }
}