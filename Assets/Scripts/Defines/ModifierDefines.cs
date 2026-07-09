using System;

public static class ModifierDefines
{
    public enum VisibleState
    {
        hidden,
        tooltip_only,
        always_visible
    }
    public delegate void ModifierAction(ReactionTable e);
    public enum State
    {
        //hard disables
        cannot_attack = 0,
        cannot_cast = 1,
        cannot_move = 10,

        //immunities
        dot_immune = 2,
        debuff_immune = 3,
        soft_disable_immune = 4,
        hard_disable_immune = 5,

        cannot_miss = 6,

        priority_melee_target = 7,
        priority_range_target = 8,

        true_block = 9,
        cannot_guard = 11,

        total = 12,
    }
    public static bool IsPropertyMultiplicative(Property prop)
    {
        return prop == Property.attack_bonus_percent
        || prop == Property.magic_bonus_percent
        || prop == Property.health_bonus_percent
        || prop == Property.barrier_bonus_percent
        || prop == Property.armor_bonus_percent
        || prop == Property.resistance_bonus_percent
        || prop == Property.block_bonus_percent
        || prop == Property.padding_bonus_percent
        || prop == Property.shield_bonus_percent

        || prop == Property.incoming_damage
        || prop == Property.incoming_barrier
        || prop == Property.incoming_healing

        || prop == Property.incoming_phys_damage
        || prop == Property.incoming_elem_damage
        || prop == Property.outgoing_pure_damage
        || prop == Property.incoming_blunt_damage
        || prop == Property.incoming_pierce_damage
        || prop == Property.incoming_slash_damage
        || prop == Property.incoming_magic_damage

        || prop == Property.outgoing_damage
        || prop == Property.outgoing_shielding
        || prop == Property.outgoing_phys_damage
        || prop == Property.outgoing_elem_damage
        || prop == Property.outgoing_pure_damage
        || prop == Property.outgoing_blunt_damage
        || prop == Property.outgoing_slash_damage
        || prop == Property.outgoing_magic_damage
        || prop == Property.outgoing_heal

        || prop == Property.outgoing_bonus_damage

        || prop == Property.vampirism_percent
        || prop == Property.health_regen_percentage
        || prop == Property.health_regen_percentage_total

        || prop == Property.incoming_damage
        || prop == Property.outgoing_damage
        || prop == Property.shielding_bonus_flat

        || prop == Property.incoming_healing
        || prop == Property.incoming_barrier
        || prop == Property.outgoing_shielding

        || prop == Property.incoming_phys_damage
        || prop == Property.outgoing_phys_damage

        || prop == Property.incoming_elem_damage
        || prop == Property.outgoing_elem_damage

        || prop == Property.critical_damage
        || prop == Property.critical_chance
        || prop == Property.outgoing_bonus_damage




        || prop == Property.magic_bonus_percent;
    }
    public enum Property
    {
        // Combat
        bonus_combat = 0,
        bonus_offense = 1,
        bonus_defense = 2,

        // Attack
        attack_bonus = 3,
        attack_bonus_percent = 4,
        magic_bonus = 5,
        magic_bonus_percent = 6,

        // Resources
        action_bonus = 7,
        mana_bonus = 8,
        supply_bonus = 9,

        // Health
        health_bonus = 10,
        health_bonus_percent = 11,
        barrier_bonus = 12,
        barrier_bonus_percent = 13,

        // Armor / Defenses
        armor_bonus = 14,
        armor_bonus_percent = 15,
        shield_bonus = 16,
        shield_bonus_percent = 17,
        padding_bonus = 18,
        padding_bonus_percent = 19,
        resistance_bonus = 20,
        resistance_bonus_percent = 21,

        // Block
        block_bonus = 22,
        block_bonus_percent = 23,

        // Misc
        speed_bonus = 24,
        luck_bonus = 25,

        // Chances
        dodge_chance = 26,
        block_chance = 27,
        proc_chance = 28,

        // Incoming modifiers
        incoming_damage = 29,
        incoming_barrier = 30,
        incoming_healing = 31,

        incoming_phys_damage = 32,
        incoming_elem_damage = 33,

        incoming_blunt_damage = 34,
        incoming_pierce_damage = 35,
        incoming_slash_damage = 36,
        incoming_magic_damage = 37,
        incoming_heal = 38,
        incoming_bonus_damage = 60,

        // Outgoing modifiers
        outgoing_damage = 39,
        outgoing_shielding = 40,
        outgoing_phys_damage = 41,
        outgoing_elem_damage = 42,
        outgoing_pure_damage = 43,
        outgoing_blunt_damage = 44,
        outgoing_slash_damage = 45,
        outgoing_magic_damage = 46,
        outgoing_heal = 47,
        outgoing_bonus_damage = 48,

        // Shielding
        shielding_bonus_flat = 49,

        // Regen & Vampirism
        health_regen_bonus = 50,
        health_regen_percentage = 51,
        health_regen_percentage_total = 52,
        vampirism_constant = 53,
        vampirism_percent = 54,

        // Abilities
        stagger = 55,
        ability_cast_range = 56,
        ability_aoe_range = 57,

        // Critical
        critical_damage = 58,
        critical_chance = 59,

        // Total count (always last)
        total = 61,
    }

    public enum TroopState
    {
        Nothing = 0,
        Stealth = 1,
        Ward = 2,
        Root = 3,
        Haste = 4,
        Slow = 5,
        Total = 11,
    }
    public enum StackType    //TODO part of the modifier
    {
        Multiple = 0,
        Replace = 1,
        Unique = 2,
        IncreaseStacks = 3,
        ExtendDuration = 4
    }

    public enum Flag
    {
        Tag = 0,
        //positive
        Innate = 1,
        Buff = 2,
        Regeneration = 3,
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
        ticks = AbilityDefines.Event.Time,
        stacks = AbilityDefines.Event.OnStacksChange,
        attacks = AbilityDefines.Event.AfterAttack,
        thisBattle = AbilityDefines.Event.CombatEnd,
        forNFights = AbilityDefines.Event.CombatExit,
        thisAction = AbilityDefines.Event.Action,
        startOfTheNextTurn = AbilityDefines.Event.OnBeginTurn,
        endOfThisTurn = AbilityDefines.Event.OnEndTurn,
    }


    public static float ModifierDurationShort = 1 / 60;
    public static float ModifierDurationMedium = 1 / 10;
    public static float ModifierDurationLong = 1 / 5;
    #region Properties
    [Serializable]
    public class PropertyData
    {
        public Property Property;
        public float value;
        public float IncreasePerLevel;
    }
    #endregion
    #region States
    [Serializable]
    public class StateData
    {
        public State State;
        public float priority;
        public StateData() { }

        public StateData(State state, float priority)
        {
            State = state;
            this.priority = priority;
        }
    }
    #endregion
}