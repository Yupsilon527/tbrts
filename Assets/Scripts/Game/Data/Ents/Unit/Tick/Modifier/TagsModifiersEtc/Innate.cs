
using System.Collections.Generic;
using UnityEngine;

public class InnateData : FunctionalData
{
    public enum AuraType
    {
        innate,
        troop,
        aura,
    }
    public AuraType auraType;
    public InnateData(string internalName, Sprite sprite, AuraType aura = AuraType.innate,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Tag, ModifierDefines.PropertyData[] properties = null, ModifierDefines.StateData[] states = null, Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> funcs = null) : base(internalName, sprite,  uibehavior, priority, flag, properties, states, funcs)
    {
        auraType = aura;
    }
    public bool CanApplyToUnit(DataItemUnit caster, DataItemUnit unit)
    {
        if (unit.modifiers.HasModifier(InternalName)) return false;
        switch (auraType)
        {
            case AuraType.innate:
                return unit == caster;
            case AuraType.troop:
                return caster.troop == unit.troop;
        }
        return false;
    }
}
public class PropertyInnate : PropertyAttribute
{
    public InnateData.AuraType auraType;


    public PropertyInnate(InnateData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite,  data.uibehavior, (int)data.priority, data.states, data.properties,data.auraType)
    {
    }
    public PropertyInnate(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, int p = 0, ModifierDefines.StateData[] sa = null, ModifierDefines.PropertyData[] pr = null, InnateData.AuraType aura = InnateData.AuraType.innate) : base(internalName, caster, parent, sprite,uibehavior, p, sa, pr)
    {
        auraType = aura;
    }
    public PropertyInnate Clone(DataItemUnit unit)
    {
        var clone =  new PropertyInnate(InternalName, caster, unit, sprite, uibehavior, priority, aura: auraType);
        clone.states = states;
        clone.properties = properties;
        return clone;
    }

    public override void ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        if (act == AbilityDefines.Event.OnMoveTile || act == AbilityDefines.Event.CombatBegin)
        {
            ReviseAura();
        }
        base.ExecuteEvent(act, target);
    }
    void ReviseAura()
    {
        if (!CanApplyToUnit(parent))
        {
            Die(true);
        }
    }
    public bool CanApplyToUnit(DataItemUnit unit)
    {
        if (unit.modifiers.HasModifier(InternalName)) return false;
        switch (auraType)
        {
            case InnateData.AuraType.innate:
                return unit == caster;
            case InnateData.AuraType.troop:
                return caster.troop == unit.troop;
            case InnateData.AuraType.aura:
                return unit.GetAlignment(caster) == PlayerDefines.Alignment.playerowned && parent.GetDistanceFromObject(unit) > caster.innates.GetAbilityLevel("aura");
        }
        return false;
    }
}
