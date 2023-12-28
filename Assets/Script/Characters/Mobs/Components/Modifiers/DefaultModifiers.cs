using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DefaultModifiers
{
    public static ModifierData MarkModifier = new ModifierData(
        "Mark",
        ModifierDefines.Flag.Debuff,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Replace
        );
    public static ModifierData BloodMarkModifier = new ModifierData(
        "BloodMark",
        ModifierDefines.Flag.Debuff,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Replace,
         new ModifierDefines.StateData[] { new ModifierDefines.StateData(ModifierDefines.modStates.soft_disable_immune, 1) },
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {
             {
                 AbilityDefines.Event.OnTakeDirectDamage,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        /*DamageTable dmt = self.parent.damageable?.lastDamage;
                        if (dmt != null && dmt.IsDirectDamage())
                        {
                            //calc original damage
                            float damage = dmt.real_damage * self.GetParameter("percent_damage");

                            //find all targets with this modifier
                            List<Mob> targets = new List<Mob>();
                            foreach (Mob ent in EntityController.main.entities)
                            {
                                if ((ent.modifiers?.HasModifier(self.ModifierName)) == true)
                                {
                                    targets.Add(ent);
                                }
                            }
                            //damage all the targets
                            foreach (Mob tgt in targets)
                            {
                                tgt.damageable.RecieveDamage(self.parent, self.parentAbility,  damage, AttackDefines.AttackType.CompositeDamage, AttackDefines.DamageFlag.Indirect, 0);
                            }

                        }*/
                    }
             }
         }
        );
    public static ModifierData SlowModifier = new ModifierData(
        "Slow",
        ModifierDefines.Flag.Debuff,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Replace,

         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnCreated,
                    (PropertyModifier self, Mob attacker) =>
                    {
                            self.SetProperty(ModifierDefines.modProps.move_speed_mult, self.GetParameter("slow_strength"));
                    }
             }
         }
        );
    /*public static ModifierData PoisonModifier = new ModifierData(
        "Poison",
        ModifierDefines.Flag.Debuff,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.stacks,
         ModifierDefines.Behavior.Stacking,
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnTurnEnd,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        self.parent.damageable.DealDamage(
                            new DamageTable(
                                self.GetCaster(),
                                self.GetOwner(),
                                self.GetStackCount(),
                                 AttackDefines.DamageType.Magical,
                                  AttackDefines.DamageFlag.Indirect
                                ),
                            0
                            );
                        self.DecrementStackCount();

                    }
             }
         }
        );
    public static ModifierData BurnModifier = new ModifierData(
        "Burn",
        ModifierDefines.Flag.Debuff,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Replace,
         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnTurnEnd,
                    (PropertyModifier self, Mob attacker) =>
                    {
                        self.parent.damageable.DealDamage(
                            new DamageTable(
                                self.GetCaster(),
                                self.GetOwner(),
                                 self.GetParameter("burn_damage"),
                                 AttackDefines.DamageType.Magical,
                                  AttackDefines.DamageFlag.Indirect
                                ),
                            0
                            );

                    }
             }
         }
        );*/
    public static ModifierData MiniCritModifier = new ModifierData(
        "MiniCrit",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.tooltip_only,
         ModifierDefines.ExpireType.attacks,
         ModifierDefines.Behavior.Duration,

         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnCreated,
                    (PropertyModifier self, Mob attacker) =>
                    {
                            self.SetProperty(ModifierDefines.modProps.base_attack_mult, self.GetParameter("crit_strength"));
                    }
             }
         }
        );
    public static ModifierData BerserkerModifier = new ModifierData(
        "Berserker",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.tooltip_only,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Duration,

         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnCreated,
                    (PropertyModifier self, Mob attacker) =>
                    {
                            self.SetProperty(ModifierDefines.modProps.base_attack_mult, self.GetParameter("crit_strength"));
                    }
             }
         }
        );
    #region Mechanical Modifiers
    public static ModifierData GhostModifier = new ModifierData(
        "Ghost",
        ModifierDefines.Flag.Undispellable,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Replace,
         new ModifierDefines.StateData[] { 
             new ModifierDefines.StateData(ModifierDefines.modStates.cannot_attack, 1),
             new ModifierDefines.StateData(ModifierDefines.modStates.cannot_cast, 1),
             new ModifierDefines.StateData(ModifierDefines.modStates.invulnerable, 1),
         },

         new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
         {

             {
                 AbilityDefines.Event.OnCreated,
                    (PropertyModifier self, Mob attacker) =>
                    {
                            self.SetProperty(ModifierDefines.modProps.laif_regen_percentage, 1);
                        self.StartThinker(1f);//TODO define
                    }
             },
             {
                 AbilityDefines.Event.OnThinkerTick,
                    (PropertyModifier self, Mob attacker) =>
                    {
                            if (self.GetOwner().damageable.Health.GetPercentage()>=1)
                        {
                            self.Die(true);
                        }
                    }
             }
         }
        );
    public static ModifierData StunModifier = new ModifierData(
        "Ghost",
        ModifierDefines.Flag.HardDisable,
         ModifierDefines.VisibleState.always_visible,
         ModifierDefines.ExpireType.time,
         ModifierDefines.Behavior.Multiple,
         new ModifierDefines.StateData[] {
             new ModifierDefines.StateData(ModifierDefines.modStates.cannot_attack, 1),
             new ModifierDefines.StateData(ModifierDefines.modStates.cannot_cast, 1),
             new ModifierDefines.StateData(ModifierDefines.modStates.cannot_move, 1),
         }
        );
    #endregion
}
