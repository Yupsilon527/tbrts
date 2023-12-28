using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierController : MobComponent, IEntityEvent
{
    protected List<PropertyModifier> _modifiers;
    public List<PropertyModifier> GetModifiers()
    {
        return _modifiers;
    }

    protected override void Awake()
    {
        _modifiers = new List<PropertyModifier>();
        base.Awake();
        Refresh();
    }   
    void Update()
    {
        TimelyUpdate();
    }
  /*  public override void OnReset()
    {
        _modifiers.Clear();
        Refresh(true);
        base.OnReset();
    }*/


    #region Create Modifiers
    //public void New(propertyModifier oData, ModifierDefines.Type Type, SpecialEffectSO[] baseEffects, float delay)
    public void New(PropertyModifier modifier,  float delay)
    {
        if (Add(modifier))
        {
            Effect(modifier);
        }
    }
    public PropertyModifier New(ModifierData modifier, PropertyAbility source,  float delay)
    {
        if (Add(modifier, source, out PropertyModifier result))
        {
            Effect(result);
        }
        return result;
    }
    void Effect(PropertyModifier modifier)
    {
        /* TimerController.Create("textEffect", delay, (TimerController.Timer self) => { parent.DamageEffect(oData.ModifierName); return 0; }, TimerController.Importance.unimportant);
         foreach (SpecialEffectSO effectSO in baseEffects)
         {
             if (effectSO == null)
             {
                 Debug.LogWarning("[EffectSO] Failed to create effect for " + oData.ModifierName);
                 continue;
             }
             GameObject effect = effectSO.MakeEffect(acd, 0, 0);
             if (effect != null)
                 oData.AttachEffect(effect);
         }*/
    }
    public bool Add(ModifierData Modifier,PropertyAbility source, out PropertyModifier resultingModifier)
    {
        resultingModifier = null;
        if (IsImmuneToModifier(Modifier)) { return false; }

        if (Modifier.behavior == ModifierDefines.Behavior.Unique && HasModifier(Modifier.name))
        {
            return false;
        }

        resultingModifier = new PropertyModifier(Modifier, source);
        
        return Add(resultingModifier, true); 
    }
    public bool Add(PropertyModifier Modifier, bool skipImmunityCheck = false, bool refresh = true)
    {
        if (!skipImmunityCheck && IsImmuneToModifier(Modifier)) { return false; }
        Modifier.StartTime = Time.time;
        Modifier.parent = parent;
        /*if (Modifier.alignment == ModifierDefines.Alignment.Debuff)
        {
            Modifier.duration *= parent.modifiers.GetPropertyMultiplicative(ModifierDefines.modProps.debuff_duration_amp);
        }*/
        switch (Modifier.behavior)
        {
            case ModifierDefines.Behavior.Replace: //Replace 
                if (TryFindModifierByName(Modifier.ModifierName, false, out PropertyModifier found))
                    Remove(found);
                break;
            case ModifierDefines.Behavior.Unique: //Unique 
                if (HasModifier(Modifier.ModifierName))
                {
                    return false;
                }
                break;
            case ModifierDefines.Behavior.Stacking:
                if (TryFindModifierByName(Modifier.ModifierName, false, out PropertyModifier original))
                {
                    original.SetStackCount(original.GetStackCount() + Modifier.GetStackCount());
                    if (original.properties.Count > 0)
                        RefreshProperties();
                    return false;
                }
                break;
            case ModifierDefines.Behavior.Duration:
                if (TryFindModifierByName(Modifier.ModifierName, false, out PropertyModifier first))
                {
                    first.SetDuration(first.GetTurnDuration() + Modifier.GetTurnDuration());
                    return false;
                }
                break;

        }
        Debug.Log("[Modifiers] Add new modifier " + Modifier.ModifierName);
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
    bool propRefresh = false;
    bool statRefresh = false;
    public void RefreshModifier(PropertyModifier Modifier)
    {
        if (Modifier.states.Count > 0) RefreshStates();
        if (Modifier.properties.Count > 0) RefreshProperties();
    }
    public void RefreshProperties()
    {
        propRefresh = true;
    }
    public void RefreshStates()
    {
        statRefresh = true;
    }
    public void Refresh(bool force = false)
    {
        if (force || statRefresh || propRefresh)
        {
            parent.FireEventOnSelf(AbilityDefines.Event.OnStatUpdate);
            if (force || statRefresh) states = new int[(int)ModifierDefines.modStates.total];
            if (force || propRefresh) properties = new float[(int)ModifierDefines.modProps.total];
            foreach (PropertyModifier mod in _modifiers)
            {
                if (!mod.dead && !mod.isExpired())
                {
                    if (force || statRefresh)
                        UpdateModifierStates(mod);
                    if (force || propRefresh)
                    {
                        UpdateModifierProperties(mod);
                    }
                }
                if (propRefresh)
                {
                    parent.stats.Recalculate();
                }
                if (mod.expireType == ModifierDefines.ExpireType.time || mod.ThinkInterval > 0)
                {
                    HasUpdates = true;
                }
            }
        }
         propRefresh = false;
        statRefresh = false;
    }
    public void StatUpdate(PropertyModifier Modifier)
    {
        if (Modifier.GetProperty(ModifierDefines.modProps.base_health_mult) != 0 || Modifier.GetProperty(ModifierDefines.modProps.bonus_health) != 0)
            parent.stats.UpdateMaxHealth();

     //   if (Modifier.GetProperty(ModifierDefines.modProps.bonus_mana) != 0 || Modifier.GetProperty(ModifierDefines.modProps.base_mana_mult) != 0)
       //     parent.stats.UpdateMana();

        if (Modifier.GetProperty(ModifierDefines.modProps.laif_regen_multiplier) != 0 || Modifier.GetProperty(ModifierDefines.modProps.laif_regen_constant) != 0 || Modifier.GetProperty(ModifierDefines.modProps.laif_regen_percentage) != 0 || Modifier.GetProperty(ModifierDefines.modProps.laif_regen_fraction) != 0)
            parent.stats.UpdateHealthRegen();

       // if (Modifier.GetProperty(ModifierDefines.modProps.mana_regen_percentage) != 0 || Modifier.GetProperty(ModifierDefines.modProps.mana_regen_constant) != 0 || Modifier.GetProperty(ModifierDefines.modProps.mana_regen_percentage_total) != 0 || Modifier.GetProperty(ModifierDefines.modProps.mana_regen_fraction) != 0)
         //   parent.stats.UpdateManaRegen();

        if (Modifier.GetProperty(ModifierDefines.modProps.base_attack_bonus) != 0 || Modifier.GetProperty(ModifierDefines.modProps.base_attack_mult) != 0 || Modifier.GetProperty(ModifierDefines.modProps.bonus_attack_bonus) != 0)
            parent.stats.UpdateAttackDamage();

        if (Modifier.GetProperty(ModifierDefines.modProps.base_magic_bonus) != 0 || Modifier.GetProperty(ModifierDefines.modProps.base_magic_mult) != 0 || Modifier.GetProperty(ModifierDefines.modProps.bonus_magic_bonus) != 0)
            parent.stats.UpdateMagicDamage();

        if (Modifier.GetProperty(ModifierDefines.modProps.bonus_armor_percentage) != 0 || Modifier.GetProperty(ModifierDefines.modProps.bonus_armor_constant) != 0)
            parent.stats.UpdateArmor();

        if (Modifier.GetProperty(ModifierDefines.modProps.bonus_resist_percentage) != 0 || Modifier.GetProperty(ModifierDefines.modProps.bonus_resist_constant) != 0)
            parent.stats.UpdateResistance();

        if (Modifier.GetProperty(ModifierDefines.modProps.move_speed) != 0 || Modifier.GetProperty(ModifierDefines.modProps.move_speed_mult) != 0 || Modifier.GetProperty(ModifierDefines.modProps.move_speed_override) != 0)
            parent.stats.UpdateMoveSpeed();

    }
    #endregion
    #region Handle Modifiers
    public void OnTurnBegin()
    {
        HandleModifiers();
    }
    void HandleModifiers() {
        bool refresh = false;
        foreach (PropertyModifier mod in _modifiers)
        {
            if (mod.isExpired())
            {
                refresh = true;
                Remove(mod,true,false);
            }
        }
        if (refresh) Refresh();
    }
    #endregion
    #region States
    public int[] states = new int[(int)ModifierDefines.modStates.total];

    public bool GetState(ModifierDefines.modStates State)
    {
        return states[(int)State] > 0;
    }
    void UpdateState(ModifierDefines.modStates State, int value)
    {
        if ((int)State >= 0 && (int)State < (int)ModifierDefines.modStates.total)
            return;

        //if (Mathf.Abs(states[(int)State]) < Mathf.Abs(value))
        {
            states[(int)State] += value;
        }
    }
    void UpdateModifierStates(PropertyModifier Mod)
    {
        foreach (ModifierDefines.modStates state in Mod.states)
        {
            UpdateState(state, Mod.priority);
        }
    }

    #endregion
    #region Properties
    public float[] properties = new float[(int)ModifierDefines.modProps.total];

    public float GetPropertyAdditive(ModifierDefines.modProps Property)
    {
        return properties[(int)Property];
    }
    public float GetPropertyMultiplicative(ModifierDefines.modProps Property)
    {
        return 1 + properties[(int)Property];
    }
    void UpdateProperty(ModifierDefines.modProps Property, float value)
    {
        if ((int)Property < 0 || (int)Property >= (int)ModifierDefines.modProps.total)
            return;
        RefreshProperties();
        switch (Property)
        {
            default:                                    //PERCENTAGE
                properties[(int)Property] = (1f + properties[(int)Property]) * (value - 1) ;
                break;
            case ModifierDefines.modProps.base_attack_bonus://ADDITIVE
            case ModifierDefines.modProps.bonus_attack_bonus:
            case ModifierDefines.modProps.bonus_health:    //Rules
            case ModifierDefines.modProps.move_speed:
                properties[(int)Property] += value;
                break;
           /*     case ModifierDefines.modProps.cspeed:       //HIGHEST
                    properties[(int)Property] = Mathf.Max(properties[(int)Property], value);
                    break;*/
        }
    }
    void UpdateModifierProperties(PropertyModifier Mod)
    {
        foreach (KeyValuePair<ModifierDefines.modProps, float> prop in Mod.properties)
        {
            UpdateProperty(prop.Key, prop.Value);
        }
    }
    #endregion
    #region Effects    
    /*public void AttachEffect(SpecialEffectController effect)
    {
        //effect.transform.SetParent((overhead ? spriteRenderers[1] : spriteRenderers[0]).transform);
    }*/
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
            if (Mod.ModifierName == Name)
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
            if (mod.ModifierName.ToLower() == Name.ToLower())
            {
                found = mod;
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Filter
    public PropertyModifier[] Filter(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Nothing,  bool includePositives = false, bool includeNegative = false)
    {
        List<PropertyModifier> rest = new List<PropertyModifier>();
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (!Mod.dead && !Mod.isExpired())
            {
                if (ModifierName == "" || ModifierName == Mod.ModifierName)
                {
                    if ((alignment == ModifierDefines.Flag.Nothing || alignment == Mod.data.flag) || (includeNegative && Mod.IsNegative()) || (includePositives && Mod.IsPositive()))
                    {
                            rest.Add(Mod);
                        
                    }
                }
            }
        }
        return rest.ToArray();

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
    public void DestroyFilteredModifiers(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Nothing,  bool includePositives = false, bool includeNegative = false, bool refresh = true)
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
       if (refresh)  Refresh();
        
    }
    #endregion

    #region Funcs
    public void TriggerFuncs(AbilityDefines.Event act)
    {
        foreach (PropertyModifier Mod in _modifiers)
        {
            if (!Mod.dead && !Mod.isExpired())
            {
                Mod.ExecuteEvent(act, parent);
            }
        }
    }
    #endregion
	#region Timely Update
    float nextUpdateTime = 0;
    bool HasUpdates = false;
    public void TimelyUpdate()
    {
        if (HasUpdates && nextUpdateTime < Time.time)
        {
            nextUpdateTime = Time.time + 1;
            foreach (PropertyModifier Mod in _modifiers)
            {
                if (!Mod.dead)
                {
                        if (Mod.expireType == ModifierDefines.ExpireType.time || Mod.ThinkInterval > 0)
                        {
                            HasUpdates = true;
                        }
                        Mod.UpdateDuration(Time.time);
                        nextUpdateTime = Mathf.Min(nextUpdateTime, Mod.GetNextUpdateTime());
                    
                }
            }
        }
    }
	#endregion
    #region Resistance And Defense
    public bool IsImmuneToModifier(PropertyModifier mod)
    {
        return IsImmuneToModifier(mod.data);
    }
        public bool IsImmuneToModifier(ModifierData mod)
    {
        if (mod.flag == ModifierDefines.Flag.Undispellable)
        {
            return false;
        }

        return ((GetState(ModifierDefines.modStates.debuff_immune) && mod.IsNegative()) ||
            (mod.flag == ModifierDefines.Flag.HardDisable && GetState(ModifierDefines.modStates.hard_disable_immune)) ||
            (mod.flag == ModifierDefines.Flag.DamageOverTime && GetState(ModifierDefines.modStates.dot_immune)) ||
        (mod.flag == ModifierDefines.Flag.SoftDisable && GetState(ModifierDefines.modStates.soft_disable_immune)));
    }

    public void EventReaction(AbilityDefines.Event evt, Mob[] affectedCritters)
    {
        TriggerFuncs(evt);
        if ((int)evt == (int)ModifierDefines.ExpireType.attacks || (int)evt == (int)ModifierDefines.ExpireType.spellcast || (int)evt == (int)ModifierDefines.ExpireType.stacks || (int)evt == (int)ModifierDefines.ExpireType.time  )
            Refresh();
        
    }
    #endregion

    #region Unique Attack Modifiers
    public PropertyOrb[] GetAttackModifiers()
    {
        List<PropertyOrb> orbatks = new List<PropertyOrb>();
        foreach (PropertyModifier mod in _modifiers)
        {
            if (mod is PropertyOrb orb )
            { orbatks.Add(orb); }
        }
        return orbatks.ToArray();
    }
    #endregion
}
