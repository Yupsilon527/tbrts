using UnityEngine;
public class PropertyAbility : PropertyBase
{
    public bool active = false;
    public bool oneTime = false;
    public float procStrength = 1;
    public CombatDefines.Events condition;
    public CombatDefines.Targeting target;
    public CombatDefines.Action action;
    public override string ToString()
    {
        return $"{action} every {actionInterval} {condition} ({expiration})";
    }
    public PropertyAbility(AbilityData data) :this(data.abilityCooldown, data.abilityCondition, data.abilityTarget,data.abilityEvent, data.abilityStrength)
    {
       
    }
    public PropertyAbility(int t, CombatDefines.Events c, CombatDefines.Targeting r, CombatDefines.Action a, float p)
    {
        actionInterval = t;
        target = r;
        action = a;
        condition = c;
        procStrength = p;

        if (t == 0 || a == CombatDefines.Action.FirstStrike || a == CombatDefines.Action.AttackOnce)
            oneTime = true;
    }
    public DataItemUnit[] GetValidTargets(DataItemUnit caster, DataItemUnit victim)
    {
        switch (target)
        {
            case CombatDefines.Targeting.attackTarget:
                return new[] { victim };
            case CombatDefines.Targeting.randomEnemy:
               var  targets = caster.IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
                return new[] { targets[Mathf.FloorToInt(Random.value * targets.Length)] };
            case CombatDefines.Targeting.randomSecondaryTarget:
                targets = caster.IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
                if (targets.Length == 2)
                    return new[] { targets[1] };
                else if (targets.Length > 1)
                    return new[] { targets[1 + Mathf.FloorToInt(Random.value * (targets.Length - 1))] };
                return new DataItemUnit[0];
            case CombatDefines.Targeting.allEnemies:
                return caster.IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
            case CombatDefines.Targeting.randomAlly:
                targets = !caster.IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
                return new[] { targets[Mathf.FloorToInt(Random.value * targets.Length)] };
            case CombatDefines.Targeting.allAllies:
                return !caster.IsPlayerOwned() ? Combat.main.GetEnemies(true) : Combat.main.GetHeroes(true);
            default:
                return new[] { caster };
        }
    }
}
