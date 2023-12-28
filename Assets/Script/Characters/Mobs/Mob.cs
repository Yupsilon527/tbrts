using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public MobDefines.Archetype archetype = MobDefines.Archetype.nothing;
//Movement
    public OrdersComponent orders;
    public MovementComponent movement;
    public PathfinderComponent pathfinder;
    public AnimationComponent animations;
    public RoomTransitionComponent transition;

    //Combat
    public MobStatsComponent stats;
    public DamageableComponent damageable;
    public CombatantComponent combatant;
    public AttitudeComponent attitude;

    public AbilityComponent abilities;
    public ModifierController modifiers;
    [Header("Lifebar")]
    public HealthBarController healthBar;
    protected virtual void Awake()
    {
        if (orders == null)
            orders = GetComponent<OrdersComponent>();
        if (movement == null)
            movement = GetComponent<MovementComponent>();
        if (pathfinder == null)
            pathfinder = GetComponent<PathfinderComponent>();
        if (animations == null)
            animations = GetComponent<AnimationComponent>();

        if (attitude == null)
            attitude = GetComponent<AttitudeComponent>();
        if (combatant == null)
            combatant = GetComponent<CombatantComponent>();
        if (stats == null)
            stats = GetComponent<MobStatsComponent>();
        if (damageable == null)
            damageable = GetComponent<DamageableComponent>();
        if (abilities == null)
            abilities = GetComponent<AbilityComponent>();
        if (modifiers == null)
            modifiers = GetComponent<ModifierController>();
        if (transition == null)
            transition = GetComponent<RoomTransitionComponent>();
    }
    public virtual void Spawn()
    {
        FireEventOnSelf(AbilityDefines.Event.OnCreated);
        combatant?.EndCombat();
    }
    [Header("Death Animation")]
    public float DeathTime = 1;
    public GameObject deathEffectPrefab;
    public virtual void Die()
    {
        if (deathEffectPrefab!=null)
            SpecialEffectPool.main.EffectFromPrefab(deathEffectPrefab,transform.position);
        if (pathfinder != null)
        {
            pathfinder.ClearNode();
        }
        animations.PlayAnimation("Death");
            if (combatant != null)
            combatant.EndCombat();
        if (movement != null)
            movement.Stop();
        if (abilities != null)
            abilities.StopCasting();
        Deselect();
        Cancel();
    }
    public virtual void Despawn()
    {
        transition?.ExitCurrentRoom();
        foreach (var effect in GetComponentsInChildren<ProjectileEffectController>())
                {
            effect.Die();
        }
    }
    public virtual void Respawn()
    {
        gameObject.SetActive(true);
        damageable.Health.SetValue(1);
        modifiers.New(new PropertyModifier(DefaultModifiers.GhostModifier, null), 0);
        if (animations != null)
            animations.PlayAnimation("Idle");
    }
    public void Cancel()
    {
        foreach (IOnIntrerupt act in GetComponents<IOnIntrerupt>())
        {
            act.OnIntrerupt();
        }
    }
    bool Selected = false;
    public bool IsSelected()
    {
        return Selected;
    }
    public SpriteRenderer SelectionCircle;  //TODO separate component?
    public void Select()
    {
        Selected = true;
        SelectionCircle.color = Color.white;
        foreach (ISelectable sel in GetComponents<ISelectable>())
        {
            sel.OnSelected();
        }
    }
    public void Deselect()
    {
        Selected = false;
        SelectionCircle.color = Color.clear;
        foreach (ISelectable sel in GetComponents<ISelectable>())
        {
            sel.OnDeselected();
        }
    }
    #region Alignment
    public enum Alignment
    {
        enemy,
        playerowned,
        allied
    }
    public Alignment GetAlignment(Mob other)
    {
        if (IsPlayerControlled() == other.IsPlayerControlled())
        {
            return Alignment.playerowned;
        }
        else return Alignment.enemy;
    }
    bool playerOwned = false;
    public  void SetAlignment(bool value)
    {
        playerOwned  = value;
    }
    public  bool IsPlayerControlled()
    {
        return playerOwned;
    }
    #endregion
    #region events
    public void FireEventOnSelf(AbilityDefines.Event evtData)
    {
        HandleEvent(evtData, new Mob[0]);
    }
    public void FireEventOnTarget(AbilityDefines.Event evtData, Mob target)
    {
        HandleEvent(evtData, new Mob[] { target });
    }

    internal void OnRoomChange()
    {
        foreach (IRoomTransition ion in GetComponents<IRoomTransition>())
        {
            ion.OnChangeRoom();
        }
    }

    public virtual void HandleEvent(AbilityDefines.Event evt, Mob[] targets)
    {
        foreach (IEntityEvent ion in GetComponents<IEntityEvent>())
        {
            ion.EventReaction(evt, targets);
        }
    }

    #endregion
    #region Cans

    public bool CanRespond()
    {
        if (damageable != null && !damageable.isAlive()) return false;
        //if (modifiers != null && modifiers.GetState(ModifierDefines.modStates.cannot_move)) return false;
        return true;
    }
    public bool CanMove()
    {
        if (modifiers != null && modifiers.GetState(ModifierDefines.modStates.cannot_move)) return false;
        return true;
    }
    public bool CanCast()
    {
        if (modifiers != null && modifiers.GetState(ModifierDefines.modStates.cannot_move)) return false;
        return true;
    }
    public bool CanAttack()
    {
        if (modifiers != null && modifiers.GetState(ModifierDefines.modStates.cannot_move)) return false;
        return true;
    }
    public bool IsInvulnerable()
    {
        if (modifiers != null && modifiers.GetState(ModifierDefines.modStates.invulnerable)) return true;
        if (damageable != null && !damageable.isAlive()) return true;
        return false;
    }
    #endregion
    #region Size
    public enum EntitySize
    {
        small,
        medium,
        large
    }
    public EntitySize GetEntitySize()
    {
        return EntitySize.small;
    }
    #endregion
    public bool IsInanimate()
    {
        return false;
    }
}
