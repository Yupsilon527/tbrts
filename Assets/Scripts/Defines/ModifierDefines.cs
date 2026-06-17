using System;

public static class ModifierDefines
{
    public enum VisibleState
    {
        hidden,
        tooltip_only,
        always_visible
    }
    public delegate void ModifierAction(PropertyAttribute self, DataItemUnit attacker);
    public enum State
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

        priority_melee_target = 14,
        priority_range_target = 15,

        total = 16
    };

    public static bool IsPropertyMultiplicative(Property prop)
    {
        return prop == Property.attack_bonus_percent
        || prop == Property.mana_bonus
        || prop == Property.health_bonus_percent
        || prop == Property.barrier_bonus_percent
        || prop == Property.armor_bonus_percent
        || prop == Property.resistance_bonus_percent
        || prop == Property.block_bonus_percent
        || prop == Property.armor_bonus_percent
        || prop == Property.resistance_bonus_percent
        || prop == Property.block_bonus_percent

        || prop == Property.offense_incoming_from_bonus
        || prop == Property.defense_incoming_from_bonus
        || prop == Property.attack_incoming_from_bonus
        || prop == Property.magic_incoming_from_bonus
        || prop == Property.ctrl_incoming_from_bonus
        || prop == Property.health_incoming_from_bonus
        || prop == Property.barrier_incoming_from_bonus
        || prop == Property.armor_incoming_from_bonus
        || prop == Property.resistance_incoming_from_bonus
        || prop == Property.block_incoming_from_bonus
        || prop == Property.speed_incoming_from_bonus
        || prop == Property.luck_incoming_from_bonus

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


    || prop == Property.incoming_blunt_damage
    || prop == Property.incoming_pierce_damage
    || prop == Property.incoming_fire_damage
    || prop == Property.incoming_cold_damage
    || prop == Property.incoming_electric_damage
    || prop == Property.incoming_radiation_damage
    || prop == Property.incoming_magic_damage
    || prop == Property.incoming_psychic_damage
    || prop == Property.incoming_cosmic_damage
    || prop == Property.incoming_acid_damage
    || prop == Property.incoming_mass_damage
    || prop == Property.incoming_heal

    || prop == Property.outgoing_pure_damage
    || prop == Property.outgoing_blunt_damage
    || prop == Property.outgoing_pierce_damage
    || prop == Property.outgoing_fire_damage
    || prop == Property.outgoing_cold_damage
    || prop == Property.outgoing_electric_damage
    || prop == Property.outgoing_radiation_damage
    || prop == Property.outgoing_magic_damage
    || prop == Property.outgoing_psychic_damage
    || prop == Property.outgoing_cosmic_damage
    || prop == Property.outgoing_acid_damage
    || prop == Property.outgoing_mass_damage
    || prop == Property.outgoing_heal

    || prop == Property.enemies_health
    || prop == Property.enemies_speed
    || prop == Property.enemies_damage
    || prop == Property.curse_enemy_chance
    || prop == Property.gold_gain_percent
    || prop == Property.gold_gain_interest
    || prop == Property.experience_gain_percent
    || prop == Property.bricks_gain_percent
    || prop == Property.resource_gain_percent
    || prop == Property.item_price


        || prop == Property.special_bonus_percent;
    }
    public enum Property
    {
        //combat
        bonus_combat = 0,
        bonus_offense = 1,
        bonus_defense = 2,

        //attack
        attack_bonus = 3,
        attack_bonus_percent = 4,
        special_bonus = 5,
        special_bonus_percent = 6,
        action_bonus = 7,
        mana_bonus = 8,

        //health
        health_bonus = 9,
        health_bonus_percent = 10,
        barrier_bonus = 11,
        barrier_bonus_percent = 12,

        //armor
        armor_bonus = 19,
        armor_bonus_percent = 20,

        resistance_bonus = 21,
        resistance_bonus_percent = 22,

        block_bonus = 23,
        block_bonus_percent = 24,

        //misc
        speed_bonus = 25,
        luck_bonus = 26,

        //bonuses incoming
        offense_incoming_from_bonus = 27,
        defense_incoming_from_bonus = 28,

        attack_incoming_from_bonus = 29,
        magic_incoming_from_bonus = 30,
        ctrl_incoming_from_bonus = 31,

        health_incoming_from_bonus = 32,
        barrier_incoming_from_bonus = 33,

        armor_incoming_from_bonus = 34,
        resistance_incoming_from_bonus = 35,
        block_incoming_from_bonus = 36,

        speed_incoming_from_bonus = 37,
        luck_incoming_from_bonus = 38,

        //chances
        dodge_chance = 39,
        block_chance = 40,
        proc_chance = 41,

        //hidden healing stats
        vampirism_constant = 44,
        vampirism_percent = 45,

        health_regen_bonus = 46,
        health_regen_percentage = 47,
        health_regen_percentage_total = 48,

        //damage mult
        incoming_damage = 50,
        outgoing_damage = 51,

        incoming_barrier = 42,
        incoming_healing = 43,

        shielding_bonus_flat = 52,
        outgoing_shielding = 53,

        incoming_phys_damage = 54,
        outgoing_phys_damage = 55,

        incoming_elem_damage = 56,
        outgoing_elem_damage = 57,

        incoming_blunt_damage = 58,
        incoming_pierce_damage = 59,
        incoming_fire_damage = 61,
        incoming_cold_damage = 62,
        incoming_electric_damage = 63,
        incoming_radiation_damage = 64,
        incoming_magic_damage = 65,
        incoming_psychic_damage = 66,
        incoming_cosmic_damage = 67,
        incoming_acid_damage = 68,
        incoming_mass_damage = 69,
        incoming_heal = 70,

        outgoing_pure_damage = 71,
        outgoing_blunt_damage = 72,
        outgoing_pierce_damage = 73,
        outgoing_fire_damage = 75,
        outgoing_cold_damage = 76,
        outgoing_electric_damage = 77,
        outgoing_radiation_damage = 78,
        outgoing_magic_damage = 79,
        outgoing_psychic_damage = 80,
        outgoing_cosmic_damage = 81,
        outgoing_acid_damage = 82,
        outgoing_mass_damage = 83,
        outgoing_heal = 84,


        //difficulty modifiers
        enemies_health = 87,
        enemies_speed = 88,
        enemies_damage = 89,

        curse_enemy_chance = 90,
        curse_items_chance = 91,

        //resources
        gold_gain_percent = 93,
        gold_gain_interest = 94,

        experience_gain_percent = 95,
        bricks_gain_percent = 96,
        resource_gain_percent = 97,

        item_price = 98,
        shop_resets = 99,
        item_level = 103,
        item_rarity = 104,
        
        total = 105,
        ability_cast_range = 106,
        ability_aoe_range = 107,
    }


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
        time = AbilityDefines.Event.Step,
        stacks = AbilityDefines.Event.OnStacksChange,
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