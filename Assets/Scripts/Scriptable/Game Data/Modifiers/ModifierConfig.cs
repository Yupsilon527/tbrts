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
        public float apCost, rpCost, spCost;
        public AbilityDefines.Event listener;
        public AbilityDefines.Condition condition;
        public AttackEffectSO[] effects;

        public  ModifierDefines.ModifierAction Translate()
        {
            var tEffects = effects.Select(x=> x.Translate());
            return (ReactionTable table) =>
             {
                 if (!MeetsCondition(table.caster)) return;

                 if (table.caster.actions.ActionPoint.GetValue() < apCost
                || table.caster.actions.ReactionPoints.GetValue() < rpCost
                || table.caster.actions.SupplyPoints.GetValue() < spCost)
                     return;

                 table.caster.actions.ActionPoint.ChargeValue(apCost);
                 table.caster.actions.ReactionPoints.ChargeValue(rpCost);
                 table.caster.actions.SupplyPoints.ChargeValue(spCost);

                 CastTable castTable = new(table.caster, table.target);
                 foreach (var e in tEffects)
                     e.Activate(castTable);

             };
        }
        public bool MeetsCondition(DataItemUnit unit)
        {
            switch (condition)
            {
                case AbilityDefines.Condition.Damaged:
                    return unit.health.GetPercentage() < 1;
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
                modifier.functions.Add(evt.listener, evt.Translate()) ;
            }
            return modifier;
        }
        return null;
    }
}

