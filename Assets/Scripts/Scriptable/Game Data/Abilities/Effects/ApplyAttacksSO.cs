using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace VikingParty
{

    [CreateAssetMenu(fileName = "Apply Attacks", menuName = "Abilities/Effects/Apply Attacks")]
    public class ApplyAttacksSO : ApplyPhaseSO
    {
        public AttackData[] attacks;
        public override void ActivateOnCaster(AttackTable table, RollTable roll)
        {
            if (targeting == AbilityDefines.TargetType.nobody) return;
            foreach (var attack in attacks)
            {
                table.attacker.damageable?.RecieveAttack(table.attacker, attack.BaseDamage, attack.attack, attack.scaling, roll);
            }
        }
        public override void ActivateOnTargets(AttackTable table, RollTable roll)
        {
            if (targeting == AbilityDefines.TargetType.nobody) return;
            foreach (var attack in attacks)
            {
                table.target.damageable?.RecieveAttack(table.attacker, attack.BaseDamage, attack.attack, attack.scaling, roll);
            }
        }
        public override string GetLongDescription(Combatant owner, Combatant target)
        {
            string value = base.GetLongDescription(owner, target);
            string final = "";

            foreach (var attack in attacks)
            {
                float str = Mathf.Ceil(AttackDefines.GetAttackStrength(owner, target, attack, false,null));

                if (!string.IsNullOrEmpty(final)) final += "<br>";
                if (str > 0)
                {
                    final += value.Replace("%effect%", $"{str} {LanguageController.main.Translate(attack.attack.ToString())}");
                }
                else
                {
                    final += value.Replace("%effect%", LanguageController.main.Translate(attack.attack.ToString()));
                }
            }
            return final;
        }

        public override string[] GetTranslationStrings()
        {
            List<string> total = new();

            total.AddRange(base.GetTranslationStrings());
            foreach (var attack in attacks)
            {
                total.Add(attack.attack.ToString());
            }
            total.RemoveAll(s => s == "%effect%");
            return total.ToArray();
        }
        public override float GetHealValue( RollTable table)
        {
            float str = 0;
            foreach (var attack in attacks)
            {
                switch (attack.attack)
                {
                    case AttackDefines.ActionType.MissingHealthHealing:
                    case AttackDefines.ActionType.CurrentHealthHealing:
                    case AttackDefines.ActionType.TotalHealthHealing:
                    case AttackDefines.ActionType.Heal:
                    case AttackDefines.ActionType.Vampirism:
                        str += Mathf.Ceil(AttackDefines.GetAttackStrength(table.attacker, table.target, attack, false, table));
                        break;
                    default:
                        continue;
                }
            }
            return str;
        }
        public override float GetDamageValue(RollTable table)
        {
            float str = 0;
            foreach (var attack in attacks)
            {
                switch (attack.attack)
                {
                    case AttackDefines.ActionType.DamagePure:
                    case AttackDefines.ActionType.TotalHealthDamage:
                    case AttackDefines.ActionType.MissingHealthDamage:
                    case AttackDefines.ActionType.CurrentHealthDamage:
                    case AttackDefines.ActionType.NormalAttack:
                    case AttackDefines.ActionType.PercentageAttack:
                    case AttackDefines.ActionType.CriticalAttack:
                    case AttackDefines.ActionType.NakedAttack:
                    case AttackDefines.ActionType.UnarmedAttack:
                    case AttackDefines.ActionType.BlockPercentageAttack:
                    case AttackDefines.ActionType.PoisonDamage:
                    case AttackDefines.ActionType.PoisonDamageRandom:
                        str += Mathf.Ceil(AttackDefines.GetAttackStrength(table.attacker, table.target, attack, false, table));
                        break;
                    default:
                        continue;
                }
            }
            return str;
        }
        public override float GetBlockValue(RollTable table)
        {
            float str = 0;
            foreach (var attack in attacks)
            {
                switch (attack.attack)
                {
                    case AttackDefines.ActionType.Block:
                    case AttackDefines.ActionType.BlockPure:
                    case AttackDefines.ActionType.NakedBlock:
                    case AttackDefines.ActionType.UnarmedBlock:
                    case AttackDefines.ActionType.GainArmorRaw:
                    case AttackDefines.ActionType.GainArmorBonus:
                        str += Mathf.Ceil(AttackDefines.GetAttackStrength(table.attacker, table.target, attack, false, table));
                        break;
                    default:
                        continue;
                }
            }
            return str;
        }
    }
}