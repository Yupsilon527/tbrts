
using UnityEngine;

[CreateAssetMenu(fileName = "Built In Modifier", menuName = "Abilities/Effects/Built In Modifier")]
public class BuiltInModifier : ApplyEffects
{
    public enum BuiltInModifierType
    {
        GoldIncome = 0,
        HealPostCombat = 1,

    }
    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        table.target.modifiers.ApplyNewModifier(Translate(), table.tick, true, false);

    }
    #region Params
    [Header("Parameters")]
    public BuiltInModifierType BuiltinType;
    public PropertyModifier Translate(int stacks = 0)
    {
        PropertyModifier nModifier = null;
        switch (BuiltinType)
        {
            case BuiltInModifierType.HealPostCombat:
                nModifier = new (DefaultModifiers.HealPostCombat);
                break;

        }
        if (nModifier != null)
        {
            foreach (ModifierParameter parain in vars)
            {
                nModifier.SetParameter(parain.name, parain.value);
            }
        }
        nModifier.SetStackCount(stacks);
        return nModifier;
    }
    void InitParams()
    {
        switch (BuiltinType)
        {
            case BuiltInModifierType.GoldIncome:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("gold_income", 10),
                };
                break;
            case BuiltInModifierType.HealPostCombat:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("post_combat_heal", 5),
                };
                break;
        }

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
        if (vars == null || vars.Length == 0) InitParams();
    }
    #endregion
}
