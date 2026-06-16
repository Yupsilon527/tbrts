using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatantAbilities : UnitComponent, CombatantTicker
{
    public int lastTick = 0;

    public ResourceInt Ap;
    public ResourceInt Mp;

    public HashSet<PropertyWeapon> _actions = new();
    public HashSet<EffectCounter> _effects = new();
    public class EffectCounter
    {
        public ApplyEffects appliedEffect;
        public int counter = 1;

        public EffectCounter(ApplyEffects effect)
        {
            appliedEffect = effect;
        }
    }
    public CombatantAbilities(DataItemUnit parent) : base(parent)
    {
    }
    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        if (act == AbilityDefines.Event.OnRefresh
            || act == AbilityDefines.Event.OnSpawn)
        {
            ClearAbilities();
            FromCombatantData();
        }
        else if (act ==  AbilityDefines.Event.CombatBegin)
        {
            lastTick = 0;
            Ap.SetPercentage(1);
            Mp.SetPercentage(1);
        }
        base.TriggerFuncs(act);
    }
    public void ClearAbilities()
    {
        _actions.Clear();
        _effects.Clear();
    }
    void AddAbility(PropertyAbility ability)
    {
        _actions.Add(ability);
        ability.SetCooldown(Mathf.CeilToInt(parent.stats.realStats.SpeedCoefficient * ability.actionDelay));
    }
    public void FromCombatantData()
    {
        foreach (var ability in parent.scriptable.abilities)
        {
            if (ability != null) AddAbility(ability.data);
        }
    }
    public void AddAbility(CombatantAbilityTable data)
    {
        RegisterEvent(data.abilityEvent);
        foreach (var a in data.attacks)
            _effects.Add(new EffectCounter(a));
        foreach (var m in data.modifiers)
            _effects.Add(new EffectCounter(m));
        foreach (var p in data.procs)
            _effects.Add(new EffectCounter(p));
    }
    public void RegisterEvent(AbilityData abilityEvent)
    {
        var action = new PropertyAbility(abilityEvent);
        AddAbility(action);
    }
    public bool Trigger(DataItemUnit mainTarget, CombatDefines.AttackPhase phase, int ticks)
    {
        var abilities = _actions.Where(a => a.HasResourcesToCast() && a.attackPhase == phase);

        foreach (var action in abilities)
        {
            if (action.ForwardTime(ticks))
            {
                while (action.expiration <= 0)
                {
                    int currentTick = Combat.main.currentTick + action.expiration;
                    //.main.Inspect($"Combatant {parent.scriptable.name} performs action {action.action} at turn {currentTick}");
                    foreach (var target in action.GetValidTargets(parent, mainTarget))
                    {
                        Action(target, action.action, currentTick, action.procStrength);
                    }
                    if (action.mpCost > 0)
                    {
                        action.SetCooldown(-1);
                        return true;
                    }
                    else
                    {
                        action.ExtendCooldown(parent.stats.realStats.SpeedCoefficient);
                    }
                }
                return true;
            }
        }
        return ticks == 0;
    }
    public bool CastAbility(PropertyAbility ability, Gem gem = null)
    {
        if (!ability.RequiresGemTarget())
        {
            Combat.main.GetCurrentPlayer().abilities.CastAbilityNoTarget(ability);
            Combat.main.PostPlayerTurn();
            return true;
        }
        else if (gem != null && ability.IsValidGemTarget(gem))
        {
            Combat.main.GetCurrentPlayer().abilities.CastAbilityOnGem(ability, gem);
            Combat.main.PostPlayerTurn();
            return true;
        }
        return false;
    }
    void CastAbilityNoTarget(PropertyAbility ability)
    {
        Combat.main.Actionbegin(new CastTable(parent, parent.GetAttackTarget(), ability, Combat.main.currentTurn));
        ability.CastFromTable(CastTable.main);
        Combat.main.ActionConclude();
    }
    void CastAbilityOnGem(PropertyAbility ability, Gem gem)
    {
        Combat.main.Actionbegin(new CastTable(parent, parent.GetAttackTarget(), ability, Combat.main.currentTurn, targetGem: gem));
        ability.CastFromTable(CastTable.main);
        Combat.main.ActionConclude();
    }
    public bool Tick(int steps)
    {
        int tickDelta = steps - lastTick;
        lastTick = steps;
        return Trigger(parent.GetAttackTarget(), CombatDefines.Events.Ticks, tickDelta);
    }
    public int GetNextTick(int steps)
    {
        int ticks = int.MaxValue;
        foreach (var action in _actions)
        {
            ticks = Mathf.Min(ticks, steps + action.expiration);
        }
        return ticks;
    }

    public virtual string OutputTable()
    {
        string output = "<br><b>Actions</b><br>";
        foreach (var action in _actions)
        {
            output += action.ToString() + "<br>";
        }
        output += "<br><b>Effects</b><br>";
        foreach (var effect in _effects)
        {
            output += effect.appliedEffect.GetDescription() + "<br>";
        }
        return output;
    }
}
