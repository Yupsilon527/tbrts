using System;
using UnityEditor;
using UnityEngine;

public static class ModifierDefines
{
    public enum VisibleState
    {
        hidden,
        tooltip_only,
        always_visible
    }
    public delegate void ModifierAction(PropertyModifier self, Mob attacker);
    public enum modStates
    {
        //hard disables
        cannot_move = 0,
        cannot_attack = 1,
        cannot_cast = 2,
        ai_override = 3,    //take over AI
        command_restricted = 4, //can issue orders, cannot be issued orders by player

        //immunities
        dot_immune = 8,//OBSOLETE immune to damage over time states
        debuff_immune = 9,//negative modifier immunity
        soft_disable_immune = 10,//slow immunity only
        hard_disable_immune = 11,//unstoppable - slow and CC immunity

        //misc
        invulnerable = 12,
        out_of_the_game = 13,

        total = 14
    };

    public enum modProps//???
    {
        //movement
        move_speed = 0,
        move_speed_mult = 1,
        move_speed_override = 2,

        //attack bonus
        base_attack_bonus = 3,
        base_attack_mult = 4,
        bonus_attack_bonus = 5,

        //health
        bonus_health = 6,
        base_health_mult = 7,

        incoming_damage = 8,
        outgoing_damage = 9,
        incoming_healing = 10,
        outgoing_healing = 11,
        outgoing_shielding = 12,

        inc_damage_life = 13,
        inc_damage_shield = 14,

        incoming_melee = 15,
        incoming_range = 16,
        incoming_area = 17,
        outgoing_melee = 18,
        outgoing_range = 19,
        outgoing_area = 20,

        ability_cast_range = 21,
        ability_aoe_range = 22,

        //debuff_duration_amp = 10,

        laif_regen_constant = 23,           //regen N points every second
        laif_regen_multiplier = 24,         //increases constant regen by % percentage
        laif_regen_percentage = 25,          //regen percentage of total life
        laif_regen_fraction = 26,           //regen % of current healt? REQUIRES REWORK

        bonus_armor_constant = 27,
        bonus_armor_percentage = 28,

        bonus_resist_constant = 29,
        bonus_resist_percentage = 30,

        base_magic_bonus = 31,
        base_magic_mult = 32,
        bonus_magic_bonus = 33,

        total = 34,
    };

    public enum Behavior    //TODO part of the modifier
    {
        Multiple = 0,
        Replace = 1,
        Unique = 2,
        Stacking = 3,
        Duration = 4
    }

    public enum Flag
    {
        Nothing = 0,
        Undispellable = 0,
        //positive
        Buff = 1,
        Regeneration = 2,
        //negative
        Debuff = -1,
        DamageOverTime = -2,
        SoftDisable = -3,
        HardDisable = -4,
    }
    public enum Priority
    {
        low = 0,
        normal = 1,
        high = 2
    }

    public enum ExpireType
    {
        permanent = AbilityDefines.Event.Nothing,
        time = AbilityDefines.Event.OnThinkerTick,
        stacks = AbilityDefines.Event.OnUpgrade,
        spellcast = AbilityDefines.Event.AbilitySuccess,
        attacks = AbilityDefines.Event.AttackLanded,
    }


    public static float ModifierDurationShort = 1 / 60;
    public static float ModifierDurationMedium = 1 / 10;
    public static float ModifierDurationLong = 1 / 5;
    #region Properties
    [Serializable]
    public class PropertyData
    {
        public ModifierDefines.modProps Property;
        public float value;
        public float IncreasePerLevel;
    }
    #endregion
    #region States
    [Serializable]
    public class StateData
    {
        public ModifierDefines.modStates State;
        public float priority;
        public StateData() { }

        public StateData(ModifierDefines.modStates state, float priority)
        {
            State = state;
            this.priority = priority;
        }
    }
    #endregion
}