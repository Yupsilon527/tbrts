using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Innate Modifier", menuName = "Abilities/Effects/Modifiers/Innate Modifier")]

public class ModifierPassive : AlterationSO
{
    public InnateData.AuraType innateType;
    public ModifierConfig.ConfigEvent[] listeners;

    public override TagData Translate()
    {
         HashSet<AbilityFunction>  actions = new();
        foreach (var evt in listeners)
        {
            var effects = evt.effects.Select(e => e.Translate());
            actions.Add(new (evt.listeners, evt.Translate()));
        }

        return new InnateData(
            InternalName,
            sprite,
            uibehavior,
            priority,
            innateType,
            flag,
            properties,
            states,
            abilities,
            actions
            );
    }
}