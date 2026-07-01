using System.Collections.Generic;

public class PropertyModifier : PropertyAttribute
{
    //expiration
    public ModifierDefines.ExpireType expireType;
    public int tickDuration = 1;
    public int stacks = 1;
    public int expiration = 1;


    public override void ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        base.ExecuteEvent(act, target);
        if (expireType != ModifierDefines.ExpireType.time && (int)act == (int)expireType)
        {
            expiration--;
            CheckExpiration();
        }
    }


    public ModifierDefines.Flag flag;
    public PropertyModifier(string Name,
      ModifierDefines.Flag a,
        ModifierDefines.VisibleState v,
      ModifierDefines.ExpireType expire,
       ModifierDefines.StackType bh,
      Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> fs) : this(Name, a, v, expire, bh, new ModifierDefines.StateData[0], new ModifierDefines.PropertyData[0], fs)
    {

    }
    public PropertyModifier(string Name = "UNDEFINED",
        ModifierDefines.Flag a = ModifierDefines.Flag.Tag,
        ModifierDefines.VisibleState v = ModifierDefines.VisibleState.tooltip_only,
        ModifierDefines.ExpireType expire = ModifierDefines.ExpireType.time,
         ModifierDefines.StackType bh = ModifierDefines.StackType.Unique,
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
    public PropertyModifier(TagData scriptable) : this(scriptable.InternalName, ModifierDefines.Flag.Tag, scriptable.uibehavior, ModifierDefines.ExpireType.permanent, scriptable.behavior)
    {
        sprite = scriptable.sprite;
    }
    public PropertyModifier(AlterationData scriptable) : this(scriptable.InternalName, scriptable.flag, scriptable.uibehavior, ModifierDefines.ExpireType.permanent, scriptable.behavior, scriptable.states, scriptable.properties)
    {
        sprite = scriptable.sprite;
    }
    public PropertyModifier(ModifierData scriptable) : this(scriptable.InternalName, scriptable.flag, scriptable.uibehavior, scriptable.expireType, scriptable.behavior, scriptable.states, scriptable.properties)
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
        return flag > ModifierDefines.Flag.Tag;
    }
    public bool IsNegative()
    {
        return flag < ModifierDefines.Flag.Tag;
    }
    #endregion
    #region Duration, expire
    public override void Think()
    {
        ExecuteFunction(AbilityDefines.Event.OnThink);
    }
    public override bool IsExpired()
    {
        return (expireType == ModifierDefines.ExpireType.time && tickDuration < 0) || (expireType == ModifierDefines.ExpireType.stacks && stacks <= 0);
    }
    public override bool RefreshCooldown(int cooldown)
    {
        bool executed = base.RefreshCooldown(cooldown);
        CheckExpiration();

        return executed;
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
}
