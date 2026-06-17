using UnityEngine;

namespace VikingParty
{
    public abstract class AttackEffectSO : ScriptableObject, ILocChecker
    {
        public AbilityDefines.Condition condition = AbilityDefines.Condition.Always;
        public AbilityDefines.TargetType targeting = AbilityDefines.TargetType.caster_and_target;
        public virtual AttackDefines.AttackPhase GetAttackPhase()
        {
            return targeting == AbilityDefines.TargetType.caster ? AttackDefines.AttackPhase.Prep : AttackDefines.AttackPhase.Attack;
        }
        public virtual void Activate(AttackTable table, RollTable roll)
        {
            if (MeetsCondition(table))
            {
                if (targeting == AbilityDefines.TargetType.caster || targeting == AbilityDefines.TargetType.caster_and_target)
                {
                    ActivateOnCaster(table, roll);
                }
                if (targeting == AbilityDefines.TargetType.targets || targeting == AbilityDefines.TargetType.caster_and_target)
                {
                    ActivateOnTargets(table, roll);
                }
            }
        }
       protected virtual bool MeetsCondition(AttackTable table)
        {
            switch (condition)
            {
                case AbilityDefines.Condition.Contact:
                    return !table.WasBlocked;
                case AbilityDefines.Condition.Blocked:
                    return table.WasBlocked;
                case AbilityDefines.Condition.Armored:
                    return table.target.damageable.ConstantArmor.GetValue() + table.target.damageable.PersistentArmor.GetValue() > 0;
                case AbilityDefines.Condition.Metal:
                    return table.WasBlocked || table.target.damageable.ConstantArmor.GetValue() + table.target.damageable.PersistentArmor.GetValue() > 0;
                case AbilityDefines.Condition.Flesh:
                    return table.WasHealthDamage;
                default:
                    return true;
            }
        }
        public abstract void ActivateOnTargets(AttackTable table, RollTable roll);
        public abstract void ActivateOnCaster(AttackTable table, RollTable roll);
        public virtual string GetLongDescription(Combatant owner, Combatant target)
        {
            switch (condition)
            {
                case AbilityDefines.Condition.Contact:
                    return LanguageController.main.Translate("condition_unblocked");
                case AbilityDefines.Condition.Blocked:
                    return LanguageController.main.Translate("condition_blocked");
                case AbilityDefines.Condition.Armored:
                    return LanguageController.main.Translate("condition_armored");
                case AbilityDefines.Condition.Metal:
                    return LanguageController.main.Translate("condition_metal");
                case AbilityDefines.Condition.Flesh:
                    return LanguageController.main.Translate("condition_flesh");
                default:
                    return "%effect%";
            }
        }

        public virtual string[] GetTranslationStrings()
        {
            switch (condition)
            {
                case AbilityDefines.Condition.Contact:
                    return new string[]{ "condition_unblocked"};
                case AbilityDefines.Condition.Blocked:
                    return new string[]{  "condition_blocked"};
                case AbilityDefines.Condition.Armored:
                    return new string[]{ "condition_armored"};
                case AbilityDefines.Condition.Metal:
                    return new string[]{ "condition_metal" };
                case AbilityDefines.Condition.Flesh:
                    return new string[]{ "condition_flesh" };
                default:
                    return new string[]{ "%effect%"};
            }
        }
        public virtual string GetShortDescription() { return ""; }
        public virtual float GetDamageValue( RollTable table)
        {
            return 0;
        }
        public virtual float GetHealValue(RollTable table)
        {
            return 0;
        }
        public virtual float GetBlockValue( RollTable table)
        {
            return 0;
        }
    }
    public abstract class ApplyPhaseSO : AttackEffectSO
    {
        public AttackDefines.AttackPhase phase = AttackDefines.AttackPhase.Attack;

        public override AttackDefines.AttackPhase GetAttackPhase()
        {
            return phase;
        }
    }
}