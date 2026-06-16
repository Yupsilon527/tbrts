using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PropertyAttribute : PropertyBase
{
    public string InternalName = "ERROR";
    public Sprite sprite;
    public bool active = true;
    public int priority;
    public ModifierDefines.Behavior behavior;

    //states, props
    public List<ModifierDefines.modStates> states = new List<ModifierDefines.modStates>();
    public Dictionary<ModifierDefines.Properties, float> properties = new Dictionary<ModifierDefines.Properties, float>();

    // Thinker
    public bool HasThinker = false;
    public int thinker = 0;
    public int lastTick = 0;

    //Parameters
    public Dictionary<string, float> parameters = new Dictionary<string, float>();

    public PropertyAttribute(string Name = "UNDEFINED",
         ModifierDefines.Behavior bh = ModifierDefines.Behavior.Unique,
        ModifierDefines.StateData[] sa = null,
        ModifierDefines.PropertyData[] pr = null)
    {
        InternalName = Name;
        behavior = bh;

        SetStates(sa);
        SetProps(pr);
    }

    #region States
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

    public void SetStates(ModifierDefines.StateData[] states)
    {
        if (states == null) return;
        for (int iS = 0; iS < states.Length; iS++)
        {
            SetState(states[iS].State, true);
        }
    }
    #endregion
    #region Properties
    public void SetProperty(ModifierDefines.Properties prop, float value)
    {
        SetPropertyRaw(prop, ModifierDefines.IsPropertyMultiplicative(prop) ? (value / 100) : value);
}
public void SetPropertyRaw(ModifierDefines.Properties prop, float value)
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
    public float GetProperty(ModifierDefines.Properties property)
    {
        if (!properties.ContainsKey(property))
            return 0;
        return properties[property];
    }

    public void SetProps(ModifierDefines.PropertyData[] properties)
    {
        if (properties == null) return;
        for (int iS = 0; iS < properties.Length; iS++)
        {
            SetProperty(properties[iS].Property,properties[iS].value);
        }
    }
    #endregion
    public bool dead = false;
    public virtual void Die(bool expire)
    {
        if (!dead)
        {
            dead = true;
        }
    }
    protected void CheckExpiration()
    {
        if (IsExpired())
        {
            Die(true);
        }
    }
    public virtual bool IsExpired()
    {
        return false;
    }
    #region Thinker
    public void StartThinker(int interval)
    {
        HasThinker = true;
        actionInterval = Mathf.Max(1, interval);
        thinker = 0;
    }
   public virtual void Think()
    {

    }
    public override bool ForwardTime(int cooldown)
    {
       bool executed = base.ForwardTime(cooldown);
        CheckExpiration();

        if (HasThinker) thinker -= cooldown;
        executed = executed || thinker < 0;
        while (thinker < 0) 
            Think();
        return executed;
    }
    #endregion
    #region Parameters
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


}
