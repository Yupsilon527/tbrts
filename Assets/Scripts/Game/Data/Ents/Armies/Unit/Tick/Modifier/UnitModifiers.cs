using System.Collections.Generic;
using UnityEngine;

public class CombatantModifiers : UnitProperties, CombatantTicker
{
    public int lastTick = 0;
    public CombatantModifiers(DataItemUnit parent) : base(parent)
    {
    }
    protected List<PropertyModifier> _modifiers = new List<PropertyModifier>();
    public List<PropertyModifier> GetModifiers()
    {
        return _modifiers;
    }

    #region Create Modifiers
    public bool ApplyNewModifierFromData(ModifierData Modifier, int atTick, out PropertyModifier resultingModifier)
    {
        resultingModifier = null;
        if (IsImmuneToModifier(Modifier)) { return false; }

        if (Modifier.behavior == ModifierDefines.Behavior.Unique && HasModifier(Modifier.name))
        {
            return false;
        }

        resultingModifier = new PropertyModifier(Modifier);

        return ApplyNewModifier(resultingModifier, atTick, refresh: true);
    }
    public bool ApplyNewModifier(PropertyModifier Modifier, int atTick, bool skipImmunityCheck = false, bool refresh = true)
    {
        if (!skipImmunityCheck && IsImmuneToModifier(Modifier)) { return false; }
        Modifier.parent = parent;
        /*if (Modifier.alignment == ModifierDefines.Alignment.Debuff)
        {
            Modifier.duration *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.debuff_duration_amp);
        }*/
        switch (Modifier.behavior)
        {
            case ModifierDefines.Behavior.Replace: //Replace 
                if (TryFindModifierByName(Modifier.InternalName, false, out PropertyModifier found))
                    Remove(found);
                break;
            case ModifierDefines.Behavior.Unique: //Unique 
                if (HasModifier(Modifier.InternalName))
                {
                    return false;
                }
                break;
            case ModifierDefines.Behavior.Stacking:
                if (TryFindModifierByName(Modifier.InternalName, false, out PropertyModifier original))
                {
                    original.SetStackCount(original.GetStackCount() + Modifier.GetStackCount());
                    if (original.properties.Count > 0)
                        RefreshProperties();
                    return false;
                }
                break;
            case ModifierDefines.Behavior.Duration:
                if (TryFindModifierByName(Modifier.InternalName, false, out PropertyModifier first))
                {
                    first.SetCooldown(first.expiration + Modifier.expiration);
                    return false;
                }
                break;

        }
        Debug.Log("[Modifiers] Add new modifier " + Modifier.InternalName);
        Modifier.SetStackCount(1);
        OnAddModifier(Modifier);
        if (refresh) Refresh();
        return true;
    }
    void OnAddModifier(PropertyModifier Modifier)
    {
        _modifiers.Add(Modifier);
        Modifier.ExecuteFunction(AbilityDefines.Event.OnCreated);
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (IsImmuneToModifier(Mod))
            {
                Mod.Die(false);
            }
        }
        RefreshModifier(Modifier);
    }
    #endregion
    #region Refresh
    public void RefreshModifier(PropertyModifier Modifier)
    {
        if (Modifier.states.Count > 0) RefreshStates();
        if (Modifier.properties.Count > 0) RefreshProperties();
    }
    public override void Refresh(bool force = false)
    {
        if (force || statRefresh || propRefresh)
        {
            if (force || statRefresh) states = new int[(int)ModifierDefines.State.total];
            if (force || propRefresh) properties = new float[(int)ModifierDefines.Property.total];
            foreach (PropertyModifier mod in _modifiers)
            {
                if (!mod.dead && !mod.IsExpired())
                {
                    if (force || statRefresh)
                        UpdateModifierStates(mod);
                    if (force || propRefresh)
                    {
                        UpdateModifierProperties(mod);
                    }
                }
                if (mod.expireType == ModifierDefines.ExpireType.time || mod.HasThinker)
                {
                    HasUpdates = true;
                }
            }
            if (propRefresh)
            {
                parent.stats.Recalculate();
            }
        }
        propRefresh = false;
        statRefresh = false;
    }
    void UpdateModifierStates(PropertyModifier Mod)
    {
        foreach (ModifierDefines.State state in Mod.states)
        {
            UpdateState(state, Mod.priority);
        }
    }
    void UpdateModifierProperties(PropertyModifier Mod)
    {
        foreach (KeyValuePair<ModifierDefines.Property, float> prop in Mod.properties)
        {
            UpdateProperty(prop.Key, prop.Value);
        }
    }
    #endregion

    #region Handle Modifiers
    public void OnTurnBegin()
    {
        HandleModifiers();
    }
    void HandleModifiers()
    {
        bool refresh = false;
        foreach (PropertyModifier mod in _modifiers)
        {
            if (mod.IsExpired())
            {
                refresh = true;
                Remove(mod, true, false);
            }
        }
        if (refresh) Refresh();
    }
    #endregion
    #region Remove Modifiers
    public void Remove(PropertyModifier Mod)
    {
        Remove(Mod, false);
    }

    public void Remove(PropertyModifier Mod, bool expire, bool refresh = true)
    {
        Remove(new PropertyModifier[] { Mod }, expire, refresh);
    }
    public void DestroyFilteredModifiers(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Undispellable, bool includePositives = false, bool includeNegative = false, bool refresh = true)
    {
        Remove(Filter(ModifierName, alignment, includePositives, includeNegative), false, refresh);
    }
    public void Remove(PropertyModifier[] Mods, bool expire = true, bool refresh = true)
    {
        foreach (PropertyModifier Modifier in Mods)
        {
            if (Modifier == null)
                continue;
            if (Modifier != null)
            {
                Modifier.Die(expire);
            }
            RefreshModifier(Modifier);

        }
        if (refresh) Refresh();

    }
    #endregion
    #region Find By Name
    public bool HasModifier(string Name)
    {
        return FindModifierByName(Name) != null;
    }
    public bool HasModifier(string Name, out PropertyModifier mod)
    {
        mod = FindModifierByName(Name);
        return mod != null;
    }
    public PropertyModifier FindModifierByName(string Name)
    {
        Name = Name.ToLower();
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (Mod.InternalName == Name)
            {
                return Mod;
            }
        }
        return null;
    }
    public bool TryFindModifierByName(string Name, bool Reverse, out PropertyModifier found)
    {
        found = null;
        for (int iM = 0; iM < _modifiers.Count; iM++)
        {
            PropertyModifier mod = _modifiers[Reverse ? (_modifiers.Count - iM - 1) : iM];
            if (mod.InternalName.ToLower() == Name.ToLower())
            {
                found = mod;
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Filter
    public PropertyModifier[] Filter(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Undispellable, bool includePositives = false, bool includeNegative = false, ModifierDefines.Flag checkFlag = ModifierDefines.Flag.Undispellable)
    {
        List<PropertyModifier> rest = new List<PropertyModifier>();
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
            {
                if (ModifierName == "" || ModifierName == Mod.InternalName)
                {
                    if ((alignment == ModifierDefines.Flag.Undispellable || alignment == Mod.flag) || (includeNegative && Mod.IsNegative()) || (includePositives && Mod.IsPositive()) || (checkFlag == ModifierDefines.Flag.Undispellable && Mod.flag == checkFlag))
                    {
                        rest.Add(Mod);

                    }
                }
            }
        }
        return rest.ToArray();

    }
    #endregion

    #region Funcs
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
            {
                Mod.ExecuteEvent(act, parent);
            }
        }
    }
    #endregion
    #region Resistance And Defense
    public bool IsImmuneToModifier(ModifierSO mod)
    {
        return IsImmuneToModifier(mod.flag, mod.flag < ModifierDefines.Flag.Undispellable);
    }
    public bool IsImmuneToModifier(PropertyModifier mod)
    {
        return IsImmuneToModifier(mod.flag, mod.IsNegative());
    }
    public bool IsImmuneToModifier(ModifierDefines.Flag flag, bool negative)
    {
        if (flag == ModifierDefines.Flag.Undispellable)
        {
            return false;
        }

        return ((GetState(ModifierDefines.State.debuff_immune) && negative) ||
            (flag == ModifierDefines.Flag.HardDisable && GetState(ModifierDefines.State.hard_disable_immune)) ||
            (flag == ModifierDefines.Flag.DamageOverTime && GetState(ModifierDefines.State.dot_immune)) ||
        (flag == ModifierDefines.Flag.SoftDisable && GetState(ModifierDefines.State.soft_disable_immune)));
    }

    public void EventReaction(AbilityDefines.Event evt, DataItemUnit[] affectedCritters)
    {
        TriggerFuncs(evt);
        if ((int)evt == (int)ModifierDefines.ExpireType.stacks || (int)evt == (int)ModifierDefines.ExpireType.time)
            Refresh();

    }
    #endregion
    #region Timely Update
    public bool Tick(int steps)
    {
        bool executed = false;
        if (HasUpdates)
        {
            int tickDelta = steps - lastTick;
            foreach (PropertyModifier Mod in _modifiers)
            {
                if (!Mod.dead)
                {
                    executed = executed | Mod.ForwardTime(tickDelta);
                }
            }
        }
        lastTick = steps;
        return executed;
    }
    public int GetNextTick(int steps)
    {
        int ticks = int.MaxValue;
        foreach (var modifier in _modifiers)
        {
            ticks = Mathf.Min(ticks, steps + modifier.expiration, steps + modifier.thinker);
        }
        return ticks;
    }
    #endregion
}
