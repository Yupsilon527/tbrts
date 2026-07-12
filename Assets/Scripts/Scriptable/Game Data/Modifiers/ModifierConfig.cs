using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Config Modifier", menuName = "Abilities/Effects/Modifiers/Config Modifier")]
public class ModifierConfig : ModifierSO
{
    public ConfigEvent[] listeners;
    [Serializable]
    public class ConfigEvent
    {
        public int apCost, rpCost, spCost;
        public AbilityDefines.Event[] listeners;
        public AbilityDefines.Condition casterCondition;
        public AbilityDefines.Condition targetCondition;
        public AttackEffectSO[] effects;

        public  ModifierDefines.ModifierAction Translate()
        {
            var tEffects = effects.Select(x=> x.Translate());
            return (ReactionTable table) =>
             {
                 if (!MeetsCondition(table.caster, casterCondition)|| !MeetsCondition(table.target, targetCondition)) return;



                 if (table.caster.actions.CanAffordAP(apCost) 
                || table.caster.actions.CanAffordRP(rpCost)
                || table.caster.actions.CanAffordSP(spCost))
                     return;

                 table.caster.actions.ActionPoint.ChargeValue(apCost);
                 table.caster.actions.ReactionPoints.ChargeValue(rpCost);
                 table.caster.actions.SupplyPoints.ChargeValue(spCost);

                 CastTable castTable = new(table.caster, table.target);
                 foreach (var e in tEffects)
                     e.Activate(castTable);

             };
        }
        public bool MeetsCondition(DataItemUnit unit, AbilityDefines.Condition condition)
        {
            switch (condition)
            {
                case AbilityDefines.Condition.Damaged:
                    return unit.damageable.Health.GetPercentage() < 1;
                case AbilityDefines.Condition.Alive:
                    return unit.damageable.IsAlive();
                    default: return true;
            }
        }
    }
    public override TagData Translate()
    {

        if ( base.Translate() is ModifierData modifier)
        {
            foreach (var evt in listeners)
            {
                var effects = evt.effects.Select(e => e.Translate());
                modifier.functions.Add(new (evt.listeners, evt.Translate())) ;
            }
            return modifier;
        }
        return null;
    }
}

