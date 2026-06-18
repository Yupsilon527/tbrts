
using System.Collections.Generic;

public static class DefaultModifiers
{
    public static ModifierData HealPostCombat = new("postCombat heal",
        null,
       flag: ModifierDefines.Flag.Buff,
        uibehavior:  ModifierDefines.VisibleState.hidden,
     expire:    ModifierDefines.ExpireType.permanent,
        priority:  ModifierDefines.Priority.normal,
 behavior:          ModifierDefines.StackType.Multiple,
          funcs:  new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
                         {
                         {
                    AbilityDefines.Event.CombatEnd,
                     (PropertyAttribute self, DataItemUnit attacker) =>
                                {

                                    self.parent.damageable.Heal(self.GetParameter("post_combat_heal"));
                     }
                }
            }
    );
   /* public static PropertyModifier GoldIncome = new("gold Income",
        ModifierDefines.Flag.Buff,
         ModifierDefines.VisibleState.hidden,
          ModifierDefines.ExpireType.permanent,
           ModifierDefines.Behavior.Multiple,
            new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
                         {
                         {
                    AbilityDefines.Event.CombatEnd,
                     (PropertyThinker self, DataItemUnit attacker) =>
                                {
                                    Player.main.GiveGold(self.GetParameter("gold_income"),true);
                }
                }
            }
    );
    /*        public static ModifierData GoliathModifier = new ModifierData(
                    "Handicap",
                    ModifierDefines.Priority.high,
                     ModifierDefines.Flag.Undispellable,
                     ModifierDefines.VisibleState.always_visible,
                     ModifierDefines.ExpireType.turns,
                     ModifierDefines.Behavior.Replace,
                         fs: new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
                         {
                         {
                             AbilityDefines.Event.OnCreated,
                                (PropertyModifier self, Combatant attacker) =>
                                {
                                        self.SetProperty(ModifierDefines.Property.incoming_damage, self.GetParameter("incoming_damage"));
                                        self.SetProperty(ModifierDefines.Property.outgoing_damage, self.GetParameter("outgoing_damage"));
                                        self.SetProperty(ModifierDefines.Property.attack_speed, self.GetParameter("speed_penalty"));
                                }
                         }
                         }
                    );
            public static ModifierData Haste = new ModifierData(
                    "Haste",
                    ModifierDefines.Priority.normal,
                    ModifierDefines.Flag.Buff,
                     ModifierDefines.VisibleState.hidden,
                     ModifierDefines.ExpireType.fights,
                     ModifierDefines.Behavior.Multiple,
                         fs: new Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction>()
                         {
                         {
                             AbilityDefines.Event.OnCreated,
                                (PropertyModifier self, Combatant attacker) =>
                                {
                                        self.SetProperty(ModifierDefines.Property.attack_speed, self.GetParameter("speed"));
                                }
                         }
                         }
                    );
            public static ModifierData Madness = new ModifierData(
                    "madness",
                    ModifierDefines.Priority.high,
                    ModifierDefines.Flag.Debuff,
                     ModifierDefines.VisibleState.always_visible,
                     ModifierDefines.ExpireType.turns,
                     ModifierDefines.Behavior.IncreaseDuration,
                   new ModifierDefines.StateData[]{
                  new ModifierDefines.StateData(ModifierDefines.State.madness,1)
                  }
                    );
            /*  public static ModifierData SuperGuardModifier = new ModifierData(
                      "SuperGuard",
                      ModifierDefines.Flag.Undispellable,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNRounds,
                       ModifierDefines.Behavior.Replace,
                           new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                           {
                               {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_low, self.GetParameter("block_low"));
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_mid, self.GetParameter("block_mid"));
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_high, self.GetParameter("block_high"));

                                      }
                               },
                               {
                                   ModifierDefines.Event.OnSuccesfulGuard,
                                      (PropertyModifier self) =>
                                      {
                                              self.parent.animations.PlayAnimation("Block");

                                      }
                               }
                           }
                      );
              public static ModifierData Armor = new ModifierData(
                      "Armor",
                      ModifierDefines.Flag.Undispellable,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNFrames,
                       ModifierDefines.Behavior.Replace,
                       new ModifierDefines.StateData[] { new ModifierDefines.StateData(ModifierDefines.State.hard_disable_immune, 1), new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune, 1) },
                           new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                           {
                               {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_low, self.GetParameter("damage_mitigation"));
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_mid, self.GetParameter("damage_mitigation"));
                                              self.SetProperty(ModifierDefines.Property.incoming_damage_high, self.GetParameter("damage_mitigation"));
                                      }
                               }
                           }
                      );
              public static ModifierData HasteModifier = new ModifierData(
                      "Haste",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,

                           new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                           {

                               {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                              self.SetProperty(ModifierDefines.Property.action_speed, self.GetParameter("speed_total"));

                                              self.SetProperty(ModifierDefines.Property.weak_attack_speed, self.GetParameter("speed_watk"));
                                              self.SetProperty(ModifierDefines.Property.strong_attack_speed, self.GetParameter("speed_satk"));
                                              self.SetProperty(ModifierDefines.Property.movement_speed, self.GetParameter("speed_move"));
                                              self.SetProperty(ModifierDefines.Property.guard_speed, self.GetParameter("speed_guard"));
                                      }
                               }
                           }
                      );
              public static ModifierData StaggerResistModifier = new ModifierData(
                      "Steady",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1)
                  }
                      );
              public static ModifierData HeavyModifier = new ModifierData(
                      "Heavy",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.hard_disable_immune,1)
                  }
                      );
              public static ModifierData UnstoppableModifier = new ModifierData(
                      "Unstoppable",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1),
                      new ModifierDefines.StateData(ModifierDefines.State.hard_disable_immune,1)
                  }
                      );
              public static ModifierData StaggerShieldModifier = new ModifierData(
                      "SteadyShield",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                          if (self.parent.damage.Shield.GetValue() ==0 )
                                          {
                                              self.Die(true);
                                          }
                                      }

                        },
                        {
                                   ModifierDefines.Event.OnShieldBreak,
                                      (PropertyModifier self) =>
                                      {
                                          self.Die(true);
                                      }

                        }

                    }
                      );
              public static ModifierData UnstoppableShieldModifier = new ModifierData(
                      "UnstoppableShield",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1),
                      new ModifierDefines.StateData(ModifierDefines.State.hard_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                          if (self.parent.damage.Shield.GetValue() ==0 )
                                          {
                                              self.Die(true);
                                          }
                                      }

                        },
                        {
                                   ModifierDefines.Event.OnShieldBreak,
                                      (PropertyModifier self) =>
                                      {
                                          self.Die(true);
                                      }

                        }

                    }
                      );
              public static ModifierData HasteShieldModifier = new ModifierData(
                      "HasteShield",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1),
                      new ModifierDefines.StateData(ModifierDefines.State.hard_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                          if (self.parent.damage.Shield.GetValue() ==0 )
                                          {
                                              self.Die(true);
                                          }
                                          else
                                      {
                                              self.SetProperty(ModifierDefines.Property.action_speed, self.GetParameter("speed_total"));

                                              self.SetProperty(ModifierDefines.Property.weak_attack_speed, self.GetParameter("speed_watk"));
                                              self.SetProperty(ModifierDefines.Property.strong_attack_speed, self.GetParameter("speed_satk"));
                                              self.SetProperty(ModifierDefines.Property.movement_speed, self.GetParameter("speed_move"));
                                              self.SetProperty(ModifierDefines.Property.guard_speed, self.GetParameter("speed_guard"));

                               }
                                      }

                        },
                        {
                                   ModifierDefines.Event.OnShieldBreak,
                                      (PropertyModifier self) =>
                                      {
                                          self.Die(true);
                                      }

                        }

                    }
                      );
              public static ModifierData ShieldRegenModifier = new ModifierData(
                      "ShieldRegen",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnSuccesfulGuard,
                                      (PropertyModifier self) =>
                                      {
                                          self.parent.damage.Shield.GiveValue(self.GetParameter("shield_replenish"));
                                      }

                        }

                    }
                      );
              public static ModifierData ShieldVampModifier = new ModifierData(
                      "ShieldVamp",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnLandHit,
                                      (PropertyModifier self) =>
                                      {
                                          self.parent.damage.Shield.GiveValue(self.GetParameter("shield_replenish"));
                                      }

                        }

                    }
                      );


              public static ModifierData Stun = new ModifierData(
                      "Stunned",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNFrames,
                       ModifierDefines.Behavior.Multiple,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.cannot_move,1),
                      new ModifierDefines.StateData(ModifierDefines.State.cannot_attack,1)
                  }
                      );
              public static ModifierData Cripple = new ModifierData(
                      "Crippled",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.cannot_move,1)
                  }
                      );
              public static ModifierData Disarm = new ModifierData(
                      "Disarm",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.cannot_attack,1)
                  }
                      );


              public static ModifierData MightModifier = new ModifierData(
                      "Might",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Multiple,

                           new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                           {

                               {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                              self.SetProperty(ModifierDefines.Property.damage_bonus, self.GetParameter("bonus_damage"));
                                      }
                               }
                           }
                      );
              public static ModifierData CounterRageModifier = new ModifierData(
                      "CounterRage",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                   new ModifierDefines.StateData[]{
                      new ModifierDefines.StateData(ModifierDefines.State.soft_disable_immune,1)
                  },
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnSuccesfulGuard,
                                      (PropertyModifier self) =>
                                      {
                                          self.IncrementStackCount();
                                      }

                        },
                        {
                                   ModifierDefines.Event.OnExpired,
                                      (PropertyModifier self) =>
                                      {
                        var  nModifier = new PropertyModifier(MightModifier, Mathf.CeilToInt(self.GetParameter("might_duration")), ModifierDefines.ExpireType.AfterNAttacks, ModifierDefines.Behavior.Multiple);
                          nModifier.SetParameter("might_strength", self.GetParameter("might_bonus") * self.GetStackCount());
                                      }
                        }

                    }
                      );


              public static ModifierData StrikerRageModifier = new ModifierData(
                      "StrikeRage",
                      ModifierDefines.Flag.Debuff,
                       ModifierDefines.VisibleState.always_visible,
                       ModifierDefines.ExpireType.AfterNTurns,
                       ModifierDefines.Behavior.Duration,
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnLandHit,
                                      (PropertyModifier self) =>
                                      {
                                          self.IncrementStackCount();
                                      }

                        },
                        {
                                   ModifierDefines.Event.OnExpired,
                                      (PropertyModifier self) =>
                                      {
                        var  nModifier = new PropertyModifier(MightModifier, Mathf.CeilToInt(self.GetParameter("might_duration")), ModifierDefines.ExpireType.AfterNAttacks, ModifierDefines.Behavior.Multiple);
                          nModifier.SetParameter("might_strength", self.GetParameter("might_bonus") * self.GetStackCount());
                                      }
                        }

                    }
                      );
              public static ModifierData PoisonMine = new ModifierData(
                      "PoisonMine",
                      ModifierDefines.Flag.Undispellable,
                       ModifierDefines.VisibleState.hidden,
                       ModifierDefines.ExpireType.AfterNRounds,
                       ModifierDefines.Behavior.Multiple,
                    new Dictionary<ModifierDefines.Event, ModifierDefines.ModifierAction>()
                    {
                        {
                                   ModifierDefines.Event.OnCreated,
                                      (PropertyModifier self) =>
                                      {
                                          var movement = self.GetOwner().movement;
                          self.SetParameter("positionx", movement.position);
                      self.SetParameter("positiony", (int)movement.GetStance());
                                      }
                        },
                        {
                                   ModifierDefines.Event.OnTurnBegin,
                                      (PropertyModifier self) =>
                                      {
                                          Fighter opponent = self.GetOwner().GetOpponent();
                                          if (opponent.movement.position == self.GetParameter("positionx") && Mathf.Abs((float)opponent.movement.GetStance() - self.GetParameter("positiony")) < 1)
                                          {
                                              opponent.damage.TakeDamage(self.GetOwner(),self.GetParameter("damage"), AttackDefines.AttackType.Inflict_Poison, AttackDefines.DamageFlag.Unblockable,0);
          self.Die(true); 
                                          }
                                      }

                        }

                    }
                      );*/
}