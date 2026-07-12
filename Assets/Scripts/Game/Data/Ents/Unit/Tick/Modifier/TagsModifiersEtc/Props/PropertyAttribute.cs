
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropertyAttribute : PropertyTag
{
    public bool active = true;
    public int priority;

    //states, props
    public List<ModifierDefines.State> states = new List<ModifierDefines.State>();
    public Dictionary<ModifierDefines.Property, float> properties = new Dictionary<ModifierDefines.Property, float>();

    //Parameters
    public Dictionary<string, float> parameters = new Dictionary<string, float>();
    public HashSet<AbilityFunction> functions = new HashSet<AbilityFunction>();

    public PropertyAttribute(AlterationData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite, data.uibehavior, (int)data.priority, data.states, data.properties)
    {
    }
    public PropertyAttribute(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden,

        int p = 0,
        ModifierDefines.StateData[] sa = null,
        ModifierDefines.PropertyData[] pr = null) : base(internalName, caster, parent, sprite, uibehavior)
    {
        priority = p;
        SetStates(sa);
        SetProps(pr, 1);

    }

    #region Functions

    public void AddFunction(AbilityFunction f)
    {
        if (!f.IsValid())
        {
            return;
        }

        functions.Add(f);

    }


    public bool ExecuteFunction(AbilityDefines.Event act)
    {
        return ExecuteEvent(act, null);
    }
    public virtual bool ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        bool v = false;
        foreach (var f in functions)
        {
            if ((active || !f.onlyWhenActive) && f.events.Contains(act))
            {
                f.action.Invoke(new ReactionTable(Combat.main.currentTick, parent, target, this));
                v = true;
            }
        }
        return v;
    }

    #endregion
    public override void Die(bool expire)
    {
        if (!dead)
        {
            if (expire)
            {
                ExecuteFunction(AbilityDefines.Event.OnExpired);
            }
            ExecuteFunction(AbilityDefines.Event.OnDestroyed);
            parent.modifiers.RefreshModifier(this);
            dead = true;
        }
    }

    #region States
    public void SetState(ModifierDefines.State state, bool value)
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
    public bool GetState(ModifierDefines.State state)
    {
        return states.Contains(state);
    }
    public bool GetState(int state)
    {
        return GetState((ModifierDefines.State)state);
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
    public void SetProperty(ModifierDefines.Property prop, float value)
    {
        SetPropertyRaw(prop, ModifierDefines.IsPropertyMultiplicative(prop) ? (value / 100) : value);
    }
    public void SetPropertyRaw(ModifierDefines.Property prop, float value)
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
    public float GetProperty(ModifierDefines.Property property)
    {
        if (!properties.ContainsKey(property))
            return 0;
        return properties[property];
    }

    public void SetProps(ModifierDefines.PropertyData[] properties, int level)
    {
        if (properties == null) return;
        for (int iS = 0; iS < properties.Length; iS++)
        {
            SetProperty(properties[iS].Property, properties[iS].value + properties[iS].IncreasePerLevel * level);
        }
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