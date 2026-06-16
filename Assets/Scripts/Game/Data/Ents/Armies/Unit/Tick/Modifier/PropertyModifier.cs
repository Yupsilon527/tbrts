using System.Collections.Generic;
using UnityEngine;

public class PropertyThinker : PropertyAttribute
{
    public Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> functions = new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>();
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

}
public class PropertyModifier : PropertyThinker
{
    //expiration
    public ModifierDefines.ExpireType expireType;
    public int tickDuration = 1;
    public int stacks = 1;

    public ModifierDefines.Flag flag;
    public ModifierDefines.VisibleState uibehavior;
    public PropertyModifier(string Name,
      ModifierDefines.Flag a,
        ModifierDefines.VisibleState v,
      ModifierDefines.ExpireType expire,
       ModifierDefines.Behavior bh,
      Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> fs) : this(Name, a, v, expire, bh, new ModifierDefines.StateData[0], new ModifierDefines.PropertyData[0], fs)
    {

    }
    public PropertyModifier(string Name = "UNDEFINED",
        ModifierDefines.Flag a = ModifierDefines.Flag.Undispellable,
        ModifierDefines.VisibleState v = ModifierDefines.VisibleState.tooltip_only,
        ModifierDefines.ExpireType expire = ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior bh = ModifierDefines.Behavior.Unique,
        ModifierDefines.StateData[] sa = null,
        ModifierDefines.PropertyData[] pr = null,
        Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> fs = null)
    {
        InternalName = Name;
        flag = a;
        uibehavior = v;
        expireType = expire;
        behavior = bh;

        SetStates(sa);
        SetProps(pr);
        functions = fs;
    }
    public PropertyModifier(ModifierSO scriptable) : this(scriptable.name, scriptable.flag, scriptable.uibehavior, scriptable.expireType, scriptable.behavior, scriptable.states, scriptable.properties, null)
    {
        sprite = scriptable.sprite;
    }
    /*  #region Ownership
      public Combatant GetOwner()
      {
          return parent;
      }
      public Combatant GetCaster()
      {
          return propertyAbility.parent;
      }
      #endregion */
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
    #region Duration, expire
    public override void Think()
    {
        ExecuteFunction(AbilityDefines.Event.OnThink);
    }
    public override bool IsExpired()
    {
        return (expireType == ModifierDefines.ExpireType.time && expiration < 0) || (expireType == ModifierDefines.ExpireType.stacks && stacks <= 0) || tickDuration < 0;
    }
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
    public override bool ForwardTime(int cooldown)
    {
        if (expireType == ModifierDefines.ExpireType.time)
            return base.ForwardTime(cooldown);

        if (HasThinker)
        {
            thinker -= cooldown;
            while (thinker < 0)
            {
                Think();
                return true;
            }
        }
        return false;
    }
    public override void ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        base.ExecuteEvent(act, target);
        if (expireType != ModifierDefines.ExpireType.time && (int)act == (int)expireType)
        {
            expiration--;
            CheckExpiration();
        }
    }
    #endregion
    #region Stacks
    public int GetStackCount()
    {
        return stacks;
    }
    public void SetStackCount(int value)
    {
        stacks = value;
        ExecuteFunction(AbilityDefines.Event.OnStacksChange);
    }
    public void IncrementStackCount()
    {
        SetStackCount(stacks + 1);
    }
    public void DecrementStackCount()
    {
        SetStackCount(stacks - 1);
    }
    #endregion
    #region Display
    public Sprite GetModifierIcon()
    {
        if (sprite != null)
        {
            return sprite;
        }
        return null;
    }
    public bool IsTooltipVisible()
    {
        return uibehavior >= ModifierDefines.VisibleState.tooltip_only;
    }
    public bool IsOverheadVisible()
    {
        return uibehavior >= ModifierDefines.VisibleState.always_visible;
    }
    #endregion
}
