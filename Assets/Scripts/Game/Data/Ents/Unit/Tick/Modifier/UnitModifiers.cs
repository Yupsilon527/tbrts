using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitModifiers : UnitProperties, CombatantTicker
{
    public int lastTick = 0;
    public UnitModifiers(DataItemUnit parent) : base(parent)
    {
    }
    #region List
    protected List<PropertyTag> _modifiers = new List<PropertyTag>();
    public PropertyModifier[] GetModifiers()
    {
        return _modifiers.Select(m => m is PropertyModifier ? m as PropertyModifier : null).ToArray();
    }
    public PropertyInnate[] GetInnates()
    {
        return _modifiers.Select(m => m is PropertyInnate ? m as PropertyInnate : null).ToArray();
    }
    #endregion
    #region Filter
    public bool HasModifier(string Name)
    {
        return FindModifierByName(Name) != null;
    }
    public PropertyTag FindModifierByName(string Name)
    {
        Name = Name.ToLower();
        foreach (var Mod in _modifiers)
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
        var modifiers = GetModifiers();
        for (int iM = 0; iM < modifiers.Length; iM++)
        {
            var mod = modifiers[Reverse ? (modifiers.Length - iM - 1) : iM];
            if (mod.InternalName.ToLower() == Name.ToLower())
            {
                found = mod;
                return true;
            }
        }
        return false;
    }
    public PropertyModifier[] Filter(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Tag, bool includePositives = false, bool includeNegative = false, ModifierDefines.Flag checkFlag = ModifierDefines.Flag.Tag)
    {
        List<PropertyModifier> rest = new List<PropertyModifier>();
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
            {
                if (ModifierName == "" || ModifierName == Mod.InternalName)
                {
                    if ((alignment == ModifierDefines.Flag.Tag || alignment == Mod.flag) || (includeNegative && Mod.IsNegative()) || (includePositives && Mod.IsPositive()) || (checkFlag == ModifierDefines.Flag.Tag && Mod.flag == checkFlag))
                    {
                        rest.Add(Mod);

                    }
                }
            }
        }
        return rest.ToArray();

    }
    #endregion
    #region Create Modifiers
    public bool ApplyNewModifierFromData(TagData tag, DataItemUnit caster = null, int stacks =1, bool refresh = true) {
        if (tag is InnateData innate)
            return ApplyNewModifierFromData(innate, caster);
        if (tag is ModifierData modifier)
            return ApplyNewModifierFromData(modifier, caster, stacks, refresh);
        if (HasModifier(tag.InternalName))
        {
            return false;
        }
        return ApplyNewModifier(new PropertyTag(tag, caster,parent), skipImmunityCheck: true, refresh: refresh);
    }
    public bool ApplyNewModifierFromData(InnateData innate, DataItemUnit caster)
    {
        if (!innate.CanApplyToUnit(caster,parent))
        {
            return false;
        }
        return ApplyNewModifier(new PropertyInnate(innate, caster, parent), refresh: true);
    }
    public bool ApplyNewModifierFromData(ModifierData Modifier, DataItemUnit caster, int atTick, int stacks , out PropertyModifier resultingModifier)
    {
        resultingModifier = null;
        if (Modifier is AlterationData alt && IsImmuneToModifier(alt)) { return false; }

        if (Modifier.behavior == ModifierDefines.StackType.Unique && HasModifier(Modifier.InternalName))
        {
            return false;
        }

        resultingModifier = new PropertyModifier(Modifier,caster,parent);

        return ApplyNewModifier(resultingModifier, atTick + Modifier.duration, stacks,refresh: true);
    }
    public bool ApplyNewModifier(PropertyTag status, int atTick=0,int stacks = 1, bool skipImmunityCheck = false, bool refresh = true)
    {
        if (!skipImmunityCheck && IsImmuneToModifier(status)) { return false; }
        status.parent = parent;
        /*if (Modifier.alignment == ModifierDefines.Alignment.Debuff)
        {
            Modifier.duration *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.debuff_duration_amp);
        }*/
        if (status is PropertyModifier modifier) {
            modifier.duration = atTick;
            modifier.stacks = stacks;
        switch (modifier.behavior)
        {
            case ModifierDefines.StackType.Replace: //Replace 
                if (TryFindModifierByName(status.InternalName, false, out PropertyModifier found))
                    Remove(found);
                break;
            case ModifierDefines.StackType.Unique: //Unique 
                if (HasModifier(status.InternalName))
                {
                    return false;
                }
                break;
            case ModifierDefines.StackType.IncreaseStacks:
                if (TryFindModifierByName(status.InternalName, false, out PropertyModifier original))
                {
                    if (status is PropertyModifier data)
                    {
                        original.SetStackCount(original.GetStackCount() + data.GetStackCount());
                        if (original.properties.Count > 0)
                            RefreshProperties();
                    }
                    return false;
                }
                break;
            case ModifierDefines.StackType.ExtendDuration:
                if (TryFindModifierByName(status.InternalName, false, out PropertyModifier first))
                {
                    if (status is PropertyModifier data)
                    {
                        first.SetCooldown(first.duration + data.duration);
                    }
                    return false;
                }
                break;

        }
        }
        else if (HasModifier(status.InternalName))
        {
            return false;
        }
        Debug.Log("[Modifiers] Add new modifier " + status.InternalName);
        OnAddModifier(status);
        if (refresh) Refresh();
        return true;
    }
    void OnAddModifier(PropertyTag tag)
    {
        _modifiers.Add(tag);
        if (tag is PropertyModifier mod)
        {
            mod.ExecuteFunction(AbilityDefines.Event.OnCreated);
            foreach (PropertyModifier Mod in _modifiers)
            {
                if (IsImmuneToModifier(Mod))
                {
                    Mod.Die(false);
                }
            }
        }
        RefreshModifier(tag);
    }
    #endregion
    #region Enable/Disable
    public void SetModifierActive(PropertyTag tag, bool value)
    {
        if (tag is PropertyAttribute mod)
        {
            mod.active = value;
            RefreshModifier(tag);
        }
    }
    #endregion
    #region Refresh
    public void RefreshModifier(PropertyTag tag)
    {
        if (tag is PropertyAttribute alt)
        {
            if (alt.states.Count > 0) RefreshStates();
            if (alt.properties.Count > 0) RefreshProperties();
        }
        if (tag is PropertyIha iha)
        {
            if (iha.grantedAbilities.Length > 0) RefreshAbilities();
        }
    }
    public override void Refresh(bool force = false)
    {
        if (force || statRefresh || propRefresh || abilRefresh)
        {
            if (force || statRefresh) states = new int[(int)ModifierDefines.State.total];
            if (force || propRefresh) properties = new float[(int)ModifierDefines.Property.total];
            if (force || abilRefresh) parent.innates.ClearTempAbilities();
            foreach (PropertyTag tag in _modifiers)
            {
                if (tag is PropertyAttribute at) { 
                if (!at.dead && at.active && !at.IsExpired())
                {
                    if (force || statRefresh)
                        UpdateModifierStates(at);
                    if (force || propRefresh)
                        UpdateModifierProperties(at);
                    if (force || abilRefresh)
                        UpdateModifierAbilities(at);
                }
                }
                if (tag is PropertyModifier mod) { 
                if (mod.expireType == ModifierDefines.ExpireType.ticks || mod.HasThinker)
                {
                    HasUpdates = true;
                }
                }
            }
            if (propRefresh)
            {
                parent.stats.Recalculate();
            }
        }
        propRefresh = false;
        statRefresh = false;
        abilRefresh = false;
    }
    void UpdateModifierStates(PropertyAttribute Mod)
    {
        foreach (ModifierDefines.State state in Mod.states)
        {
            UpdateState(state, Mod.priority);
        }
    }
    void UpdateModifierProperties(PropertyAttribute Mod)
    {
        foreach (KeyValuePair<ModifierDefines.Property, float> prop in Mod.properties)
        {
            UpdateProperty(prop.Key, prop.Value);
        }
    }
    void UpdateModifierAbilities(PropertyAttribute Mod)
    {
        if (Mod is PropertyIha iha) { 
        foreach (AbilityData prop in iha.grantedAbilities)
        {
            parent.innates.AddAbility(prop.abilityID,prop.abilityLevel,UnitDefines.UpgradeCondition.temp);
        }
        }
    }
    #endregion
    #region Increment Decrement
    public void IncrementModifier(TagData tag, int levels)
    {
        if (FindModifierByName(tag.InternalName) is PropertyModifier modifier)
        {
            modifier.IncrementStackCount(levels);
        }else
        {
            ApplyNewModifierFromData(tag);
        }
    }
    public void DecrementModifier(TagData tag, int levels)
    {
        if (HasModifier(tag.InternalName) )
        {
            foreach (var modifier in Filter(tag.InternalName))
            {
                int stacks = Mathf.Max(levels,modifier.stacks);
                modifier.DecrementStackCount(stacks);
                levels -= stacks;
            }
        }
        else
        {
            ApplyNewModifierFromData(tag);
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
        foreach (PropertyModifier mod in GetModifiers())
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
    public void Remove(PropertyTag Mod)
    {
        Remove(Mod, false);
    }

    public void Remove(PropertyTag Mod, bool expire, bool refresh = true)
    {
        Remove(new PropertyTag[] { Mod }, expire, refresh);
    }
    public void DestroyFilteredModifiers(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Tag, bool includePositives = false, bool includeNegative = false, bool refresh = true)
    {
        Remove(Filter(ModifierName, alignment, includePositives, includeNegative), false, refresh);
    }
    public void Remove(PropertyTag[] Mods, bool expire = true, bool refresh = true)
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
    #region Resistance And Defense
    public bool IsImmuneToModifier(TagData mod)
    {
        return IsImmuneToModifier(mod.GetFlag());
    }
    public bool IsImmuneToModifier(PropertyTag mod)
    {
        return IsImmuneToModifier(mod.GetFlag());
    }
    public bool IsImmuneToModifier(ModifierDefines.Flag flag)
    {
        if (flag == ModifierDefines.Flag.Tag)
        {
            return false;
        }

        return ((GetState(ModifierDefines.State.debuff_immune) && flag < ModifierDefines.Flag.Tag) ||
            (flag == ModifierDefines.Flag.HardDisable && GetState(ModifierDefines.State.hard_disable_immune)) ||
            (flag == ModifierDefines.Flag.DamageOverTime && GetState(ModifierDefines.State.dot_immune)) ||
        (flag == ModifierDefines.Flag.SoftDisable && GetState(ModifierDefines.State.soft_disable_immune)));
    }

    public override void TriggerFuncs(AbilityDefines.Event evt, DataItemUnit t)
    {
        foreach (var tag in _modifiers)
        {
            if (tag is PropertyAttribute Mod)
            {
                if (!Mod.dead && !Mod.IsExpired())
                {
                    Mod.ExecuteEvent(evt, parent);
                }
            }
        }
            Refresh();
    }
    #endregion
    #region Timely Update
    public void Tick(int steps)
    {
        bool executed = false;
        if (HasUpdates)
        {
            int tickDelta = steps - lastTick;
            foreach (PropertyModifier Mod in GetModifiers())
            {
                if (!Mod.dead)
                {
                    executed = executed | Mod.RefreshCooldown(tickDelta);
                }
            }
        }
        lastTick = steps;
    }
    public int GetNextTick()
    {
        int ticks = int.MaxValue;
        foreach (var modifier in GetModifiers())
        {
            ticks = Mathf.Min(ticks, modifier.duration, modifier.thinkInterval);
        }
        return ticks;
    }

    #endregion
}
