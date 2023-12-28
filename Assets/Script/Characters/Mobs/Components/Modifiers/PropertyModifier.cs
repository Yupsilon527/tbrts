using System.Collections.Generic;
using UnityEngine;

public class PropertyModifier
{
    public string ModifierName = "ERROR";
    public ModifierDefines.ExpireType expireType;

    public float StartTime = 0;
    public bool enabled = true;

    public int priority;

    public ModifierData data;
    public ModifierDefines.Behavior behavior;
    public PropertyAbility parentAbility;
    public List<SpecialEffectController> tiedparticles = new List<SpecialEffectController>();

    public PropertyModifier(ModifierData data, PropertyAbility source = null):this(data,source, data.duration, data.expireType, data.behavior)
    {
    }
    public PropertyModifier(ModifierData data, PropertyAbility source = null, float duration = 1, ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time, ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique)
    {
        this.data = data;
        ModifierName = data.name.ToLower();
        this.expireType = expireType;
        this.behavior = behavior;

        parentAbility = source;
        for (int iS = 0; iS < data.states.Length; iS++)
        {
            SetState(data.states[iS].State, true);
        }
        SetDuration(duration);
        SetStackCount(1);
        lastUpdateTime = Time.time;
        if (data.functions != null)
        {
            foreach (KeyValuePair<AbilityDefines.Event, ModifierDefines.ModifierAction> kvp in data.functions)
            {
                functions.Add(kvp.Key, kvp.Value);
            }
        }
    }
    #region Ownership
    public Mob parent;
    public Mob GetOwner()
    {
        return parent;
    }
    public Mob GetCaster()
    {
        return parentAbility.caster;
    }
    #endregion
    #region Alignment

    public bool IsPositive()
    {
        return data.IsPositive();
    }
    public bool IsNegative()
    {
        return data.IsNegative();
    }
    #endregion
    #region Functions
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions = new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>();

    public void AddFunction(AbilityDefines.Event Name, ModifierDefines.ModifierAction execution)
    {
        if (execution == null)
        {
            return;
        }

        functions.Add(Name, execution);

    }

    public void ExecuteFunction(AbilityDefines.Event act)
    {
        ExecuteEvent(act, null);
    }

    public void ExecuteEvent(AbilityDefines.Event act, Mob target)
    {
        if (functions.TryGetValue(act, out ModifierDefines.ModifierAction func))
            func.Invoke(this, target);
        if (expireType != ModifierDefines.ExpireType.time && (int)act == (int)expireType)
        {
            duration--;
            CheckExpiration();
        }
    }

    #endregion
    #region States
    public List<ModifierDefines.modStates> states = new List<ModifierDefines.modStates>();
    public void SetState(ModifierDefines.modStates state, bool value)
    {
        if (value)
        {
            if (!states.Contains(state))
                states.Add(state);
        }
        else
        {
            if (states.Contains(state))
                states.Remove(state);
        }
    }
    public bool GetState(ModifierDefines.modStates state)
    {
        return states.Contains(state);
    }
    public bool GetState(int state)
    {
        return GetState((ModifierDefines.modStates)state);
    }
    #endregion
    #region Properties
    public Dictionary<ModifierDefines.modProps, float> properties = new Dictionary<ModifierDefines.modProps, float>();
    public void SetProperty(ModifierDefines.modProps prop, float value)
    {
        if (properties.ContainsKey(prop))
        {
            properties[prop] = value;
        }
        else
        {
            properties.Add(prop, value);
        }
    }
    public float GetProperty(ModifierDefines.modProps property)
    {
        if (!properties.ContainsKey(property))
            return 0;
        return properties[property];
    }

    #endregion
    #region Particles
    public virtual void AttachEffect(GameObject effect)
    {
        if (effect == null)
            return;
        if (effect.TryGetComponent(out SpecialEffectController sf))
        {
            tiedparticles.Add(sf);
            sf.AttachToModifier(this);
        }
    }
    public void DetachParticle(SpecialEffectController eff)
    {
        if (tiedparticles.Contains(eff))
        {
            tiedparticles.Remove(eff);
        }
    }
    void DetachAllEffects()
    {
        for (int I = 0; I < tiedparticles.Count; I++)
        {
            if (tiedparticles[0] == null)
                tiedparticles.RemoveAt(0);
            else
                tiedparticles[0].Stop();
            }

        tiedparticles.Clear();
    }
    #endregion

    public bool dead = false;
    public virtual void Die(bool expire)
    {
        if (!dead)
        {
            if (expire)
            {
                ExecuteFunction(AbilityDefines.Event.OnExpired);
            }
            DetachAllEffects();
            ExecuteFunction(AbilityDefines.Event.OnDestroy);
            parent.modifiers.RefreshModifier(this);
            dead = true;
        }
    }
    #region Thinker
    public float ThinkInterval = 0;
    public bool HasThinker = false;
    public void StartThinker(float interval)
    {
        HasThinker = true;
        ThinkInterval = Mathf.Max(.1f,interval);
    }
    #endregion
    #region Duration & Increment
    float duration = 1;
    public float GetTurnDuration()
    {
        return duration;
    }
    public void SetDuration(float value)
    {
        duration = value;
    }
    float lastUpdateTime = 0;
    float lastTickTime = 0;
    public void UpdateDuration(float time)
    {
        lastTickTime += lastUpdateTime - time;
        lastUpdateTime = time;
        CheckExpiration();
        if (!dead && HasThinker)
        {
            while (ThinkInterval > 0 && lastTickTime > ThinkInterval)
            {
                ExecuteFunction(AbilityDefines.Event.OnThinkerTick);
                lastTickTime -= ThinkInterval;
            }
        }
    }
    public float GetNextUpdateTime()
    {
      return  lastTickTime + lastTickTime;
    }
    public virtual bool isExpired()
    {
        return (expireType == ModifierDefines.ExpireType.time && Time.time >= StartTime + duration) || (expireType == ModifierDefines.ExpireType.stacks && stacks <= 0) ||duration <0;
    }
    protected void CheckExpiration()
    {
        if (isExpired())
        {
            Die(true);
        }
    }
    #endregion
    #region Stacks
    public int stacks = 1;
    public int GetStackCount()
    {
        return stacks;
    }
    public void SetStackCount(int value)
    {
        stacks = value;
        UpdateFromLevel();
        ExecuteFunction(AbilityDefines.Event.OnUpgrade);
    }
    public void IncrementStackCount()
    {
        SetStackCount(stacks + 1);
    }
    public void DecrementStackCount()
    {
        SetStackCount(stacks - 1);
    }
    void UpdateFromLevel()
    {
        foreach (ModifierDefines.PropertyData prop in data.properties)
        {
            SetProperty(prop.Property, prop.value + prop.IncreasePerLevel * (stacks - 1));

        }
    }
    #endregion
    #region Parameters
    public Dictionary<string, float> parameters = new Dictionary<string, float>();
    public void SetParameter(string name, float value)
    {
        if (parameters.ContainsKey(name))
        {
            parameters[name] = value;
        }
        else
        {
            parameters.Add(name, value);
        }
    }
    public float GetParameter(string name)
    {
        if (parameters.TryGetValue(name, out var value))
            return value;
        return 0;
    }
    #endregion
    #region Display
    public Sprite GetModifierIcon()
    {
        if (data != null && data.sprite != null)
        {
            return data.sprite;
        }
        if (parentAbility != null && parentAbility.original.sprite != null)
        {
            return parentAbility.original.sprite;
        }
        return null;
    }
    public bool IsTooltipVisible()
    {
        return data.uibehavior >= ModifierDefines.VisibleState.tooltip_only;
    }
    public bool IsOverheadVisible()
    {
        return data.uibehavior >= ModifierDefines.VisibleState.always_visible;
    }
    #endregion
}

