using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Built In Modifier", menuName = "Abilities/Effects/Built In Modifier")]
public class BuiltInModifier : AbilityEffect
{
    [Header("Duration")]
    public float duration = 1;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.time;
    #region Type
    public enum BuiltInModifierType
    {
        Stun = 0,
        Mark = 1,
        Berserk = 2,
        Minicrit = 3,
    }
    public BuiltInModifierType BuiltinType;
    public override void ActivateOnTargets(CastTable table, Mob[] targets, float animdelay)
    {
        foreach (Mob target in targets)
        {
            target.modifiers.New(GetBuiltinModifier(table.ability), 0);
        }
    }
    PropertyModifier GetBuiltinModifier(PropertyAbility source)
    {
        PropertyModifier newModifier = null;
        switch (BuiltinType)
        {
            case BuiltInModifierType.Stun:
                newModifier = new PropertyModifier(DefaultModifiers.StunModifier, source, duration, expireType, behavior);
                break;
            case BuiltInModifierType.Berserk:
                newModifier = new PropertyModifier(DefaultModifiers.BerserkerModifier, source, duration, expireType, behavior);
                break;
            case BuiltInModifierType.Minicrit:
                newModifier = new PropertyModifier(DefaultModifiers.MiniCritModifier, source, duration, expireType, behavior);
                break;
            case BuiltInModifierType.Mark:
                newModifier = new PropertyModifier(DefaultModifiers.MarkModifier, source, duration, expireType, behavior);
                break;

        }
        if (newModifier!=null)
        {
            foreach (ModifierParameter parain in vars)
            {
                newModifier.SetParameter(parain.name, parain.value);
            }
        }
        return newModifier;
    }
    #endregion
    #region Builtin ability variables
    [System.Serializable]
    public class ModifierParameter
    {
        public string name;
        public float value;

        public ModifierParameter(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }
    public ModifierParameter[] vars;
    public float GetVarValue(string name, float def = 1)
    {
        foreach (ModifierParameter abv in vars)
        {
            if (abv.name == name)
            {
                return abv.value;
            }
        }
        return def;
    }
    private void OnValidate()
    {
        if (vars != null) return;
        switch (BuiltinType)
        {
            case BuiltInModifierType.Minicrit:
            case BuiltInModifierType.Berserk:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("crit_strength", 1),

                };
                break;
            default:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("delete_this", 1),
                };
                break;
        }
    }
    #endregion
}
