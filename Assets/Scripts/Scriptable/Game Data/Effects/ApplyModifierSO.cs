
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Apply Modifier", menuName = "Abilities/Effects/Apply Modifier")]
public class ApplyModifierSO : ApplyPhaseSO
{
    public ModifierSO appliedModifier;
    public ModifierParameterAlteration[] alteredParameters;

    public override void ActivateOnCaster(AttackTable table, RollTable roll)
    {
        if (targeting == AbilityDefines.TargetType.nobody) return;
        GiveTarget(table.attacker, table.attacker, roll);

    }
    public override void ActivateOnTargets(AttackTable table, RollTable roll)
    {
        if (targeting == AbilityDefines.TargetType.nobody) return;
        GiveTarget(table.attacker, table.target, roll);
    }

    void GiveTarget(Combatant caster, Combatant target, RollTable table)
    {
        var modifier = target.modifiers.New(appliedModifier, caster, alert: true);
        foreach (var parameter in modifier.parameters)
        {
            foreach (var alteration in alteredParameters)
            {
                if (alteration.parameter == parameter.Key)
                {
                    modifier.parameters[alteration.parameter] = AttackDefines.GetScaleStrength(caster, target, alteration.scaleoff, parameter.Value, alteration.scaleMode, alteration.scaleRate, alteration.scaleDamage, table);
                }
            }
        }
    }
    public override string[] GetTranslationStrings()
    {
        List<string> strings = new List<string>();
        strings.AddRange(appliedModifier.GetTranslationStrings());
        strings.Add("apply_modifier_self");
        strings.Add("apply_modifier_target");
        strings.Add("apply_modifier_both");
        return strings.ToArray();
    }
    public override string GetShortDescription()
    {
        string applyString = targeting == AbilityDefines.TargetType.caster ? "apply_modifier_self" : targeting == AbilityDefines.TargetType.targets ? "apply_modifier_target" : "apply_modifier_both";
        if (appliedModifier.duration == 0 || appliedModifier.expireType == ModifierDefines.ExpireType.permanent)
            return LanguageController.main.Translate(applyString).Replace("%modifier%", LanguageController.main.TranslateName(appliedModifier.name));
        return LanguageController.main.Translate(applyString).Replace("%modifier%", LanguageController.main.TranslateName(appliedModifier.name)).Replace("%duration%", appliedModifier.duration.ToString());
    }
    public override string GetLongDescription(Combatant owner, Combatant target)
    {
        string value = base.GetLongDescription(owner, target);
        return value.Replace("%effect%", appliedModifier.GetModifierDescription(0, 0, 1)).Replace("%modifier%", LanguageController.main.TranslateName(appliedModifier.name));
    }
    [Serializable]
    public class ModifierParameterAlteration
    {
        public string parameter;
        public AttackDefines.ScaleType scaleMode = AttackDefines.ScaleType.Nothing;
        public AttackDefines.ScaleMode scaleoff = AttackDefines.ScaleMode.caster;
        public AttackDefines.ScaleRate scaleRate = AttackDefines.ScaleRate.additive;
        public float scaleDamage = 0;

    }
    private void OnValidate()
    {
        if (alteredParameters == null || alteredParameters.Length == 0)
        {
            if (appliedModifier is BuiltInModifier bim)
            {
                alteredParameters = new ModifierParameterAlteration[bim.vars.Length];

                for (int i = 0; i < bim.vars.Length; i++)
                {
                    alteredParameters[i] = new ModifierParameterAlteration()
                    {
                        parameter = bim.vars[i].name,
                    };
                }
            }
        }
    }
}
