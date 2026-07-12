using System.Collections.Generic;
using UnityEngine;
public class PropertyModifier : PropertyThinker
{
    public ModifierDefines.Flag flag;
    public ModifierDefines.StackType behavior = ModifierDefines.StackType.Unique;
    //expiration
    public ModifierDefines.ExpireType expireType;
    public ModifierDefines.ExpireType destroyType;
    public int stacks = 1;
    public int duration = 1;

      public PropertyModifier(ModifierData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite, data.behavior, data.uibehavior, (int)data.priority, data.states, data.properties,data.grantedAbilities,data.functions,  data.flag,data.expireType,data.destroyEvent,data.thinker,data.stacks,data.duration)
    { }
    public PropertyModifier(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null, ModifierDefines.StackType behavior = ModifierDefines.StackType.Unique, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, int p = 0,  ModifierDefines.StateData[] sa = null, ModifierDefines.PropertyData[] pr = null, AbilityData[] grantedAbilities = null,  HashSet<AbilityFunction>  functions = null, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.permanent, ModifierDefines.ExpireType destroyType = ModifierDefines.ExpireType.permanent, int thinkInterval = 0, int stacks = 1, int duration = 1) : base(internalName, caster, parent, sprite, uibehavior, p, thinkInterval, sa, pr, grantedAbilities,functions)
    {
        this.flag = flag;
        this.behavior = behavior;
        this.expireType = expireType;
        this.destroyType = destroyType;

        this.stacks = stacks;
        this.thinkInterval = thinkInterval;
        this.duration = duration;

        SetProps(pr, stacks);
    }

    public override bool ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        if (base.ExecuteEvent(act, target))
        {
            if (destroyType != ModifierDefines.ExpireType.ticks && (int)act == (int)destroyType)
            {
                Die(true);
            }
            if (expireType != ModifierDefines.ExpireType.ticks && (int)act == (int)expireType)
            {
                duration--;
                CheckExpiration();
            }
            return true;
        }
        return false;
    }


    public override ModifierDefines.Flag GetFlag()
    {
        return flag;
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
        ExecuteFunction(AbilityDefines.Event.Time);
    }
    public override bool IsExpired()
    {
        return (expireType == ModifierDefines.ExpireType.ticks && duration < Combat.main.currentTick) || (expireType == ModifierDefines.ExpireType.stacks && stacks <= 0) || duration<=0;
    }
    public override bool RefreshCooldown(int cooldown)
    {
        bool executed = base.RefreshCooldown(cooldown);
        CheckExpiration();

        return executed;
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
