
using System.Collections.Generic;
using UnityEngine;

namespace VikingParty
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Abilities/Effects/Modifier")]
    public class ModifierConfig : ModifierSO
    {
        public ModifierDefines.PropertyData[] properties = new ModifierDefines.PropertyData[0];
        public ModifierDefines.StateData[] states = new ModifierDefines.StateData[0];
        public override PropertyModifier Translate(Combatant caster, Combatant target, int stacks = 0)
        {
            var modifierData = new ModifierData(name, priority, flag, uibehavior, expireType, behavior, activation, states, properties, null);
            modifierData.sprite = sprite;
            var mod = new PropertyModifier(modifierData,caster,target, duration, expireType, destroyCondition, behavior);
            mod.SetStackCount(stacks);
            return mod;
        }

        public override string GetModifierDescription(int lvlc, int lvln, float levelScale)
        {
            string desc = "";

            if (lvlc == lvln)
            {
                desc += ModifierDefines.GetPropertyTable(properties, lvlc, levelScale);
                if (!string.IsNullOrEmpty(desc)) desc += "<br>";
                desc += GetPropertiesDescription();
            }
            else
            {
                foreach (var prop in properties)
                {
                    if (!string.IsNullOrEmpty(desc)) desc += "<br>";

                    desc += $"{prop.ValueToString(lvln + lvlc, true, levelScale)} {LanguageController.main.Translate("prop_" + prop.Property.ToString())}";
                    if (prop.IncreasePerLevel != 0)
                        desc += $" ({prop.ValueToString(lvln - lvlc, false, levelScale)})";

                }
            }
            return desc;
        }
        public override string GetPropertiesDescription()
        {
            return  ModifierDefines.GetStateTable(states);
        }
        public override float GetProperty(ModifierDefines.Property property)
        {
            foreach (var p in properties)
            {
                if (p.Property == property)
                    return p.value;
            }
            return 0;
        }
        public override bool GetState(ModifierDefines.State state)
        {
            foreach (var s in states)
            {
                if (s.State == state)
                    return true;
            }
            return false;
        }
        public override string[] GetTranslationStrings()
        {
            List<string> total = new();
            total.AddRange(base.GetTranslationStrings());
            foreach (var prop in properties)
            {
                total.Add("prop_" + prop.Property.ToString());
            }
            foreach (var state in states)
            {
                total.Add("state_" + state.State.ToString());
            }
                return total.ToArray();
        }
    }
}