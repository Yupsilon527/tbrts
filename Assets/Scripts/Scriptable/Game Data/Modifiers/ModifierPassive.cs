using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Innate Modifier", menuName = "Abilities/Effects/Modifiers/Innate Modifier")]

public class ModifierPassive : AlterationSO
{
    public InnateData.AuraType immateType;
    public ModifierConfig.ConfigEvent[] listeners;

    public override TagData Translate()
    {
        Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> actions = new();
        foreach (var evt in listeners)
        {
            var effects = evt.effects.Select(e => e.Translate());
            actions.Add(evt.listener, evt.Translate());
        }

        return new InnateData(
            InternalName,
            sprite,
            immateType,
            uibehavior,
            priority,
            flag,
            properties,
            states,
            actions
            );
    }
}