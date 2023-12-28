using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Builtin Passive Ability", menuName = "Abilities/Passive/Builtin Passive Ability")]

public class ItemPassiveSO : AbilitySO
{

   /* public static ModifierData FirstStrike = new ModifierData(
        "FirstStrike",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.hidden,
         ModifierDefines.ExpireType.permanent,
         ModifierDefines.Behavior.Stacking,
         new ModifierDefines.StateData[] {

         },
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {
             {
                 AbilityDefines.Event.OnTurnStart,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        Mob caster = self.parent;

                        if (caster == null) return;
                PropertyModifier critM = new PropertyModifier(DefaultModifiers.MiniCritModifier, self.parentAbility, 1);
                critM.SetParameter("crit_strength",self.GetParameter("crit_strength") * self.GetStackCount());

                    }
             }
         }
        );
    public static ModifierData Bloodlust = new ModifierData(
        "Bloodlust",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.hidden,
         ModifierDefines.ExpireType.permanent,
         ModifierDefines.Behavior.Stacking,
         new ModifierDefines.StateData[] {

         },
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {
             {
                 AbilityDefines.Event.OnScoreKill,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        Mob caster = self.parent;

                        if (caster == null) return;
                        PropertyModifier rageM = new PropertyModifier(DefaultModifiers.BerserkerModifier, self.parentAbility, Mathf.FloorToInt(self.GetParameter("modifier_duration")));
                        rageM.SetParameter("crit_strength",self.GetParameter("crit_strength") * self.GetStackCount());
                    }
             }
         }
        );
    public static ModifierData Cannibalism = new ModifierData(
        "Cannibalism",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.hidden,
         ModifierDefines.ExpireType.permanent,
         ModifierDefines.Behavior.Stacking,
         new ModifierDefines.StateData[] {

         },
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {
             {
                 AbilityDefines.Event.OnScoreKill,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        Mob caster = self.parent;

                        if (caster == null) return;
                       caster.damageable.RecieveDamage(self.parent, self.parentAbility,  self.GetParameter("heal_value") * self.GetStackCount(), AttackDefines.AttackType.Heal, AttackDefines.DamageFlag.Indirect, 0);
                      }
             }
         }
        );


    public static ModifierData RestHeal = new ModifierData(
        "RestHeal",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.hidden,
         ModifierDefines.ExpireType.permanent,
         ModifierDefines.Behavior.Stacking,
         new ModifierDefines.StateData[] {

         },
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {
             {
                 AbilityDefines.Event.OnTurnEnd,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        Mob caster = self.parent;

                        if (caster == null) return;

                        if (caster.damageable == null || caster.damageable.lastDamage== null|| caster.damageable.lastDamage.time >= Time.time + self.GetParameter("interval"))
                            return;

                        caster.damageable.RecieveDamage(self.parent, self.parentAbility,  self.GetParameter("heal_value") * self.GetStackCount(), AttackDefines.AttackType.Heal, AttackDefines.DamageFlag.Indirect, 0);
                    }
             }
         }
        );
    public override Dictionary<AbilityDefines.Event, AbilityDefines.AbilityFunction> TranslateFunctions()
    {
        return new Dictionary<AbilityDefines.Event, AbilityDefines.AbilityFunction> {

            {
                AbilityDefines.Event.OnCreated,
                (CastTable castData) =>
                {
                    if (castData == null ||castData.caster == null|| castData.ability == null)
                    {
                        Debug.LogError("Invalid castdata on "+name);
                    }

                PropertyModifier markM = new PropertyModifier(GetModifierData(), castData.ability, Mathf.CeilToInt(GetVarValue("turns", 3)));
            foreach (BuiltInModifier.ModifierParameter parain in vars)
            {
                markM.SetParameter(parain.name, parain.value);
            }
                }
            },
            {
                AbilityDefines.Event.OnLevelChange,
                (CastTable castData) =>
                {
                    if (castData == null ||castData.caster == null|| castData.ability == null)
                    {
                        Debug.LogError("Invalid castdata on "+name);
                    }
                PropertyModifier markM = new PropertyModifier(GetModifierData(), castData.ability, Mathf.CeilToInt(GetVarValue("turns", 3)));
            foreach (BuiltInModifier.ModifierParameter parain in vars)
            {
                markM.SetParameter(parain.name, parain.value);
            }
                }
            }
        };
    }
    public enum DefaultItemModifier
    {
        ItemFirstStrike = 0,
        ItemBloodlust = 1,
        ItemCannibalism = 2,
        ItemRestHeal = 3,
    }
    public DefaultItemModifier Modifier;
    public ModifierData GetModifierData()
    {
        switch (Modifier)
        {
            case DefaultItemModifier.ItemFirstStrike:
                return FirstStrike;
            case DefaultItemModifier.ItemBloodlust:
                return Bloodlust;
            case DefaultItemModifier.ItemCannibalism:
                return Cannibalism;
            case DefaultItemModifier.ItemRestHeal:
                return RestHeal;
            default:
                return null;

        }
    }
    public BuiltInModifier.ModifierParameter[] vars;
    public float GetVarValue(string name, float def = 1)
    {
        foreach (BuiltInModifier.ModifierParameter abv in vars)
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
        if (vars != null && vars.Length > 0) return;
        switch (Modifier)
        {
            case DefaultItemModifier.ItemFirstStrike:
            case DefaultItemModifier.ItemBloodlust:
                vars = new BuiltInModifier.ModifierParameter[]
                {
                    new BuiltInModifier.ModifierParameter("percent_damage", 1),

                };
                break;
            case DefaultItemModifier.ItemRestHeal:
            case DefaultItemModifier.ItemCannibalism:
                vars = new BuiltInModifier.ModifierParameter[]
                {
                    new BuiltInModifier.ModifierParameter("heal_value", 4),

                };
                break;
            default:
                vars = new BuiltInModifier.ModifierParameter[]
                {
                    new BuiltInModifier.ModifierParameter("turns", 1),
                };
                break;
        }
    }*/
}
