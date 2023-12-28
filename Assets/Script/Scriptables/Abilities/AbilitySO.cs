using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilitySO : ScriptableObject
{
    public string InternalName = "MISSING";
    public string AbilityDesc = "MISSING";
    public AttackDefines.DamageFlag flags;
    public string GetTooltip(bool description)
    { return name + (description ? "_description" : "_name"); }
    public SpecialEffectSO[] AbilityVisuals = new SpecialEffectSO[0];
    public virtual AbilityDefines.CheckHitMobs CheckTargetMobs()
    {
        return (CastTable CastData) => { return new  Mob[0]; };
    }
    public class AbilityListener
    {
        public AbilityDefines.Event aEvent;
        public AbilityDefines.AbilityFunction aFunction;

        public AbilityListener(AbilityEvent evt)
        {
            aEvent = evt.Event;
            aFunction = (CastTable castData) =>
            {
                foreach (var effect in evt.defaultEffects)
                {
                    effect.Activate(castData, 0);
                }
            };
        }
        public AbilityListener(AbilityDefines.Event aEvent, AbilityDefines.AbilityFunction aFunction)
        {
            this.aEvent = aEvent;
            this.aFunction = aFunction;
        }
    }
    [System.Serializable]
    public class AbilityEvent
    {
        public AbilityDefines.Event Event;
        public AbilityEffect[] defaultEffects = new AbilityEffect[0];
    }
    public virtual List<AbilityListener> TranslateFunctions()
    {
        return new List<AbilityListener>();
    }
    public int ManaCost;
    public float GetBaseCost()
    {
        return Mathf.Max(0, ManaCost);
    }
    public float Cooldown;
    public float GetBaseCooldown()
    {
        return Mathf.Max(0, Cooldown);
    }
    public AbilityDefines.Flag[] AbilityFlags;
    public int GetAbilityFlags()
    {
        int flags = 0;
            foreach (AbilityDefines.Flag flag in AbilityFlags)
                flags |= (int)flag;
        return flags;
    }
    public virtual AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.passive;
    }
    public virtual float GetMinRange()
    {
        return 0;
    }
    public virtual float GetMaxRange()
    {
        return AbilityDefines.MeleeRange;
    }
    public virtual float GetAoERange()
    {
        return 0;
    }
    public virtual float GetCastTime()
    {
        return 1;
    }
    public virtual float GetChannelInterval()
    {
        return -1;
    }
    public Sprite sprite;
    public Sprite GetIconSprite()   //TODO
    { return sprite; }
    public virtual AbilityDefines.AbilityType GetAbilityType()
    {
        return AbilityDefines.AbilityType.none;
    }
}