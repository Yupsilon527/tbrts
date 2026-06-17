using System.Collections.Generic;
using UnityEngine;

namespace VikingParty
{
    public class ModifierSO : ScriptableObject, ILocChecker
    {
        public Sprite sprite;
        public int duration = 1;
        public ModifierDefines.Flag flag = ModifierDefines.Flag.Undispellable;
        public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
        public ModifierDefines.Activation activation = ModifierDefines.Activation.always;
        public ModifierDefines.ExpireType expireType = ModifierDefines.ExpireType.permanent;
        public ModifierDefines.ExpireType destroyCondition = ModifierDefines.ExpireType.permanent;
        public ModifierDefines.Priority priority = ModifierDefines.Priority.low;
        public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden;

        public VisualEffectSO[] visualEffects;
        public virtual PropertyModifier Translate(Combatant caster, Combatant target, int stacks = 0)
        {
            var mod = new PropertyModifier(new ModifierData(name, priority, flag, uibehavior, expireType, behavior), caster, target, duration, expireType, destroyCondition, behavior);
            mod.SetStackCount(stacks);
            return mod;
        }
        public PropertyModifier Translate(Combatant caster, int stacks = 0)
        {
            return Translate(caster,caster, stacks);
        }
        public virtual string GetModifierDescription(int currentlevel, int comparisonlevel, float levelScale)
        {
            return LanguageController.main.TranslateName(name);
        }
        public virtual string GetPropertiesDescription()
        {
            return "";
        }
        public virtual float GetProperty(ModifierDefines.Property property)
        {
            return 0;
        }
        public virtual bool GetState(ModifierDefines.State state)
        {
            return false;
        }

        public virtual string[] GetTranslationStrings()
        {
            List<string> total = new();
            if (uibehavior > ModifierDefines.VisibleState.hidden)
            total.Add(name);
            return total.ToArray();
        }
    }
}