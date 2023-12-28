using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Effects/Modifier")]
public class ModifierSO : AbilityEffect
{
    public int duration = 1;
    public Sprite sprite;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    public ModifierDefines.Priority priority = ModifierDefines.Priority.low;

    public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
    public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
   
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        foreach (Mob target in targets)
        {
            target.modifiers.New(Translate(), table.ability, 0);
        }
    }
    ModifierData Translate()
    {
        return new ModifierData(this);
    }
    #region Visibility
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    #endregion
}
