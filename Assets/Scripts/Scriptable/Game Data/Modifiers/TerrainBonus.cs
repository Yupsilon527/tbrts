using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Conditional Innate", menuName = "Abilities/Effects/Modifiers/Conditional Innate (Terrain)")]

public class TerrainBonus : ModifierPassive
{
    public TerrainDefines.Elevation requiredElevation;

    public override TagData Translate()
    {
        Dictionary<AbilityDefines.Event, ModifierDefines.ModifierAction> actions = new();
        foreach (var evt in listeners)
        {
            var effects = evt.effects.Select(e => e.Translate());
            actions.Add(evt.listener, evt.Translate());
        }
        actions.Add(AbilityDefines.Event.OnMoveTile, (ReactionTable table) => { table.caster.modifiers.SetModifierActive(table.modifier, table.caster.troop.IsInTerrain(requiredElevation)); });

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