using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityDefines
{
    public enum TargetType
    {
        caster = 0,
        targets = 1,
        caster_and_targets = 2,
        ability_target = 3,
    }
    public enum AbilityType
    {
        none = 0,
        melee=1,
        range = 2,
        snipe = 3,
        heal = 4,
        buff = 5,
    
    }
    public enum Behavior
    {
        passive = 0,
        self = 1,
        point = 2,
        target = 3,
    };

    public enum Flag
    {
        nothing = 0,
        enemies = 1,          //can hit enemies
        allies = 2,           //can hit allies
        selfcast = 4,         //ability can be targeted on self
        hurt = 8,         //ability can only target hurt allies
        basicattack = 16,
        invulnerable = 32,
    };
    public delegate void AbilityFunction(CastTable CastData);
    public delegate Mob[] CheckHitMobs(CastTable CastData);
    public static CheckHitMobs check_self_default = (CastTable CastData) =>
    {
        return new Mob[0];
    };
    public static CheckHitMobs check_circle_around_self = (CastTable CastData) =>
    {
        EnvironmentController currRoom = CastData.caster.transition.CurrentRoom;

        bool playerControlled = CastData.caster.IsPlayerControlled();
        bool targetPlayer = (playerControlled && CastData.ability.CheckFlag(Flag.allies) || !playerControlled && CastData.ability.CheckFlag(Flag.enemies));
        bool targetEnemies = (playerControlled && CastData.ability.CheckFlag(Flag.enemies) || !playerControlled && CastData.ability.CheckFlag(Flag.allies));

        return currRoom.LocalMobs.FindEntitiesInCircle(CastData.origin, CastData.ability.GetAreaRange(false), includePlayer: targetPlayer, includeEnemies: targetEnemies);//TODO filter moblist
    };

    public static CheckHitMobs check_circle_around_point = (CastTable CastData) =>
    {
        EnvironmentController currRoom = CastData.caster.transition.CurrentRoom;

        bool playerControlled = CastData.caster.IsPlayerControlled();
        bool targetPlayer = (playerControlled && CastData.ability.CheckFlag(Flag.allies) || !playerControlled && CastData.ability.CheckFlag(Flag.enemies));
        bool targetEnemies = (playerControlled && CastData.ability.CheckFlag(Flag.enemies) || !playerControlled && CastData.ability.CheckFlag(Flag.allies));

        return currRoom.LocalMobs.FindEntitiesInCircle(CastData.point, CastData.ability.GetAreaRange(false), includePlayer: targetPlayer, includeEnemies: targetEnemies);
    };

    public static CheckHitMobs check_circle_around_target = (CastTable CastData) =>
    {
        EnvironmentController currRoom = CastData.caster.transition.CurrentRoom;

        bool playerControlled = CastData.caster.IsPlayerControlled();
        bool targetPlayer = (playerControlled && CastData.ability.CheckFlag(Flag.allies) || !playerControlled && CastData.ability.CheckFlag(Flag.enemies));
        bool targetEnemies = (playerControlled && CastData.ability.CheckFlag(Flag.enemies) || !playerControlled && CastData.ability.CheckFlag(Flag.allies));

        return currRoom.LocalMobs.FindEntitiesInCircle(CastData.target.transform.position, CastData.ability.GetAreaRange(false), includePlayer: targetPlayer, includeEnemies: targetEnemies);
    };

    public enum Event
    {
        Nothing=-1,

        OnCreated=0,
        OnDestroyed = 1,


        OnAbilityEnabled =2,   //when this ability is enabled
        OnAbilityDisabled=3,  //when this ability is disabled

        AbilityBegin = 4,      //before this ability is cast
        AbilityChannel = 5,      //Each Channel Tick
        AbilityFail =6,     //each time this ability is cast
        AbilitySuccess=7,     //after this ability is cast
        AttackLanded = 27,

        OnTargetedByAbility = 8,
        OnHitByEnemy = 9,


        OnTakeDamage = 10,
        OnTakeDirectDamage = 11,
        OnTakeLifeDamage =12,

        OnHealRecieved=13,
        OnShieldRecieved=14,

        OnShieldBlock=15,
        OnShieldBreak=16,
        OnRecieveHit = 26,

        OnScoreKill =17,
        OnDeath=18,

        OnDestroy=19,
        OnExpired=20,

        OnStatUpdate=21,
        OnUpgrade=22,

        OnTeleported=23,
        OnIntrerrupted = 24,
        OnThinkerTick = 25,



        Total = 28
    }
    public static float MeleeRange = 3;
}
