
using System.Collections.Generic;
using UnityEngine;
public class PropertyInnate : PropertyIha
{
    public InnateData.AuraType auraType;


    public PropertyInnate(InnateData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite,  data.uibehavior, (int)data.priority, data.states, data.properties,data.grantedAbilities,data.functions,data.auraType)
    {
    }
    public PropertyInnate(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, int p = 0, ModifierDefines.StateData[] sa = null, ModifierDefines.PropertyData[] pr = null, AbilityData[] grantedAbilities = null,  HashSet<AbilityFunction>  functions = null,  InnateData.AuraType aura = InnateData.AuraType.innate) : base(internalName, caster, parent, sprite, uibehavior, p, sa, pr, grantedAbilities, functions)
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

    public override bool ExecuteEvent(AbilityDefines.Event act, DataItemUnit target)
    {
        bool _ = base.ExecuteEvent(act, target);
        if (act == AbilityDefines.Event.OnMoveTile || act == AbilityDefines.Event.CombatBegin || act == AbilityDefines.Event.OnSpawn)
        {
            ReviseAura();
        }
        return _;
    }
    void ReviseAura()
    {
        if (IsExpired())
        {
            Die(true);
        }
    }
    public override bool IsExpired()
    {
        return !CanApplyToUnit(parent);
    }
    public bool CanApplyToUnit(DataItemUnit unit)
    {
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
