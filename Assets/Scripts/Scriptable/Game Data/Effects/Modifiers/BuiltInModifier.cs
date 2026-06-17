
using System.Collections.Generic;
using UnityEngine;

namespace VikingParty
{
    [CreateAssetMenu(fileName = "Built In Modifier", menuName = "Abilities/Effects/Built In Modifier")]
    public class BuiltInModifier : ModifierSO
    {
        public enum BuiltInModifierType
        {
            Haste = 0,
            PoisonFeet = 1,
            MidasFeet   = 2,
            RegenFeet   = 3,
            Thiefling   = 4,
            TempAttack = 5,
            Ice = 6,
            Freeze = 7,

        }
        #region Params
        [Header("Parameters")]
        public BuiltInModifierType BuiltinType;
        public override PropertyModifier Translate(Combatant caster, Combatant target, int stacks = 0)
        {
            PropertyModifier nModifier = null;
            switch (BuiltinType)
            {
                case BuiltInModifierType.Haste:
                    nModifier = new PropertyModifier(DefaultModifiers.Haste, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.PoisonFeet:
                    nModifier = new PropertyModifier(CoreDiceModifiers.PoisonWalkerModifier, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.MidasFeet:
                    nModifier = new PropertyModifier(CoreDiceModifiers.GoldWalkerModifier, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.RegenFeet:
                    nModifier = new PropertyModifier(CoreDiceModifiers.HealWalkerModifier, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.Thiefling:
                    nModifier = new PropertyModifier(CoreDiceModifiers.BurglarModifier, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.TempAttack:
                    nModifier = new PropertyModifier(DefaultModifiers.TempAttack, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.Ice:
                    nModifier = new PropertyModifier(DefaultModifiers.IceModifier, caster, target, duration, expireType, destroyCondition, behavior);
                    break;
                case BuiltInModifierType.Freeze:
                    nModifier = new PropertyModifier(DefaultModifiers.FreezeModifier, caster, target, duration, expireType, destroyCondition, behavior);
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
                case BuiltInModifierType.Haste:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("speed", 1),
                    };
                    break;
                case BuiltInModifierType.PoisonFeet:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("damage_percent", 3),
                    };
                    break;
                case BuiltInModifierType.MidasFeet:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("gold_income", 3),
                    };
                    break;
                case BuiltInModifierType.RegenFeet:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("step_healing", 5),
                    };
                    break;
                case BuiltInModifierType.Thiefling:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("steal_percent", 20),
                    };
                    break;
                case BuiltInModifierType.TempAttack:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("attack", 1),
                    };
                    break;
                case BuiltInModifierType.Freeze:
                case BuiltInModifierType.Ice:
                    vars = new ModifierParameter[]
                    {
                    new ModifierParameter("strength", 1),
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
        public override string[] GetTranslationStrings()
        {
            List<string> total = new()
            {
                DefaultModifiers.Haste.name,
                CoreDiceModifiers.PoisonWalkerModifier.name,
                CoreDiceModifiers.GoldWalkerModifier.name,
                CoreDiceModifiers.HealWalkerModifier.name,
                CoreDiceModifiers.BurglarModifier.name,
                DefaultModifiers.TempAttack.name,
                DefaultModifiers.FreezeModifier.name,
                DefaultModifiers.IceModifier.name,
            };
            return total.ToArray();
            }
    }
}