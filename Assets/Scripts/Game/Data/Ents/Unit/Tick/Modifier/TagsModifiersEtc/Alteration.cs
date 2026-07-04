
using System.Collections.Generic;
using UnityEngine;

public class AlterationData : TagData
{
    public ModifierDefines.Flag flag;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];

    public AlterationData(string internalName, Sprite sprite,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null) : base(internalName, sprite, uibehavior)
    {
        this.priority = priority;
        this.flag = flag;
        this.properties = properties;
        this.states = states;
    }

    public float GetProperty(ModifierDefines.Property property)
    {
        foreach (var p in properties)
        {
            if (p.Property == property)
                return p.value;
        }
        return 0;
    }
    public bool GetState(ModifierDefines.State state)
    {
        foreach (var s in states)
        {
            if (s.State == state)
                return true;
        }
        return false;
    }
    public override ModifierDefines.Flag GetFlag()
    {
        return flag;
    }
}

public class PropertyAttribute : PropertyTag
{
    public bool active = true;
    public int priority;

    //states, props
    public List<ModifierDefines.State> states = new List<ModifierDefines.State>();
    public Dictionary<ModifierDefines.Property, float> properties = new Dictionary<ModifierDefines.Property, float>();

    //Parameters
    public Dictionary<string, float> parameters = new Dictionary<string, float>();
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions = new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>();

    public PropertyAttribute(AlterationData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite,  data.uibehavior,(int) data.priority,data.states,data.properties)
    {
    }
    public PropertyAttribute(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden,
       
        int p = 0,
        ModifierDefines.StateData[] sa = null,
        ModifierDefines.PropertyData[] pr = null) : base(internalName, caster, parent, sprite, uibehavior)
    {
        priority = p;
        SetStates(sa);
        SetProps(pr);

    }

    #region Functions

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
    public virtual void ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        if (functions.TryGetValue(act, out ModifierDefines.ModifierAction func))
            func.Invoke(this, target);
    }

    #endregion

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

    public void SetProps(ModifierDefines.PropertyData[] properties)
    {
        if (properties == null) return;
        for (int iS = 0; iS < properties.Length; iS++)
        {
            SetProperty(properties[iS].Property, properties[iS].value);
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