using System.Collections.Generic;
using UnityEngine;

public class PropertyIha : PropertyAttribute
{
    public AbilityData[] grantedAbilities;

    public PropertyIha(FunctionalData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite, data.uibehavior, (int)data.priority, data.states, data.properties,data.grantedAbilities,data.functions)
    {
    }
    public PropertyIha(InheritAbilityData data, DataItemUnit caster, DataItemUnit parent = null) : this(data.InternalName, caster, parent, data.sprite, data.uibehavior, (int)data.priority, data.states, data.properties,data.grantedAbilities)
    {
    }
    public PropertyIha(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, int p = 0, ModifierDefines.StateData[] sa = null, ModifierDefines.PropertyData[] pr = null, AbilityData[] grantedAbilities = null,  HashSet<AbilityFunction>  functions=null) : base(internalName, caster, parent, sprite, uibehavior, p, sa, pr)
    {
        this.grantedAbilities = grantedAbilities;
        foreach (var func in functions)
        {
            AddFunction(func);
        }
    }
}
