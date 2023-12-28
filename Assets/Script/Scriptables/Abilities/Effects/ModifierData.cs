using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierData 
{
    public string name;
    public Sprite sprite;
    public int duration = 1;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;

    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
    #region Constructors
    public ModifierData() { }
    public ModifierData(string Name, ModifierDefines.Flag a)
    {
        name = Name;
        flag = a;
        expireType = ModifierDefines.ExpireType.permanent;
    }
    public ModifierData(string Name, ModifierDefines.Flag a, ModifierDefines.ExpireType expire)
    {
        name = Name;
        flag = a;
        expireType = expire;
    }
    public ModifierData(string Name,
      ModifierDefines.Flag a,
        ModifierDefines.VisibleState v,
      ModifierDefines.ExpireType expire,
       ModifierDefines.Behavior bh,
      Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> fs) : this(Name, a,v, expire, bh, new ModifierDefines.StateData[0], fs)
    {

    }
    public ModifierData(string Name,
      ModifierDefines.Flag a,
        ModifierDefines.VisibleState v,
      ModifierDefines.ExpireType expire,
       ModifierDefines.Behavior bh,
      ModifierDefines.StateData[] sa) : this(Name, a,v, expire, bh, sa, new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>())
    {

    }

    
    public ModifierData(string Name = "UNDEFINED", 
        ModifierDefines.Flag a = ModifierDefines.Flag.Undispellable,
        ModifierDefines.VisibleState v = ModifierDefines.VisibleState.tooltip_only,
        ModifierDefines.ExpireType expire = ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior bh = ModifierDefines.Behavior.Unique,
        ModifierDefines.StateData[] sa = null,
        Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> fs = null)
    {
        name = Name;
        flag = a;
        uibehavior = v;
        expireType = expire;
        behavior = bh;

        if (sa == null)
            states = new ModifierDefines.StateData[0];
        else
            states = sa;

        if (fs == null)
            functions = new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>();
        else
            functions = fs;
    }
    public ModifierData(ModifierSO scriptable) : this(scriptable.name,scriptable.flag,scriptable.uibehavior,scriptable.expireType, scriptable.behavior, scriptable.states, null)
    {
        sprite = scriptable.sprite;
    }
    #endregion
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions;
    #region Alignment

    public bool IsPositive()
    {
        return flag > ModifierDefines.Flag.Undispellable;
    }
    public bool IsNegative()
    {
        return flag < ModifierDefines.Flag.Undispellable;
    }
    #endregion
    #region Visibility
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    #endregion
}
