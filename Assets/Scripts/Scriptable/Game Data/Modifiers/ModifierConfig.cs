using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Config Modifier", menuName = "Abilities/Effects/Modifiers/Config Modifier")]
public class ModifierConfig : ModifierSO
{
    public ConfigEvent[] Events;
    public class ConfigEvent
    {
        public AbilityDefines.Event condition;
        public AttackEffectSO[] effects;
    }
    public override TagData Translate()
    {

        if ( base.Translate() is ModifierData modifier)
        {
            foreach (var evt in Events)
            {
                var effects = evt.effects.Select(e => e.Translate());
                modifier.functions.Add(evt.condition, (PropertyAttribute self, DataItemUnit attacker) =>
                {
                    CastTable castTable = new(self.parent,attacker.troop.formation.GetPositionForUnit(attacker),null);
                    foreach (var e in effects)
                        e.Activate(castTable);

                });
            }
            return modifier;
        }
        return null;
    }
}

