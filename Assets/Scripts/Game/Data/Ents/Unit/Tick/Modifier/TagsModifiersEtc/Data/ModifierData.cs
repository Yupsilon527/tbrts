
using System.Collections.Generic;
using UnityEngine;

public class ModifierData : FunctionalData
{
    public ModifierDefines.StackType behavior = ModifierDefines.StackType.Unique;
    public int duration = 1;
    public int stacks = 1;
    public int thinker = 1;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.ticks;
    public ModifierDefines.ExpireType destroyEvent = ModifierDefines.ExpireType.ticks;

    public ModifierData(string internalName, Sprite sprite, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, AbilityData[] grantedAbilities = null,  HashSet<AbilityFunction>  funcs = null, ModifierDefines.StackType behavior = ModifierDefines.StackType.IncreaseStacks, int duration = 1, ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.permanent, ModifierDefines.ExpireType destroyEvent = ModifierDefines.ExpireType.permanent, int thinker = 0, int stacks = 1) : base(internalName, sprite, uibehavior, priority, flag, properties, states, grantedAbilities, funcs)
    {
        this.behavior = behavior;
        this.duration = duration;
        this.stacks = stacks;
        this.thinker = thinker;
        this.expireType = expireType;
        this.destroyEvent = destroyEvent;
    }
}
