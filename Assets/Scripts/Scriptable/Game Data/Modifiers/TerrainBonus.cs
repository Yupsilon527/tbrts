using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Conditional Innate", menuName = "Abilities/Effects/Modifiers/Conditional Innate (Terrain)")]

public class TerrainBonus : ModifierPassive
{
    public TerrainDefines.Elevation requiredElevation;

    public override TagData Translate()
    {
         HashSet<AbilityFunction>  actions = new();
        foreach (var evt in listeners)
        {
            var effects = evt.effects.Select(e => e.Translate());
            actions.Add(new (evt.listeners, evt.Translate()));
        }
        actions.Add(new (AbilityDefines.Event.OnMoveTile, (ReactionTable table) => { table.caster.modifiers.SetModifierActive(table.modifier, table.caster.troop.IsInTerrain(requiredElevation)); table.caster.modifiers.RefreshModifier(table.modifier); },false));

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