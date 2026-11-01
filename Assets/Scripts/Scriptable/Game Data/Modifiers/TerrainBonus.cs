using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Conditional Innate", menuName = "Abilities/Effects/Modifiers/Conditional Innate (Terrain)")]

public class TerrainBonus : ModifierPassive
{
    public TerrainDefines.Elevation requiredElevation;

    public override TagData Translate()
    {
         HashSet<AbilityReaction>  reactions = new();
        foreach (var evt in listeners)
        {
            var effects = evt.effects.Select(e => e.Translate());
            reactions.Add(new (evt.listeners, evt.target, evt.timing, evt.Translate()));
        }

        HashSet<AbilityFunction> actions = new();
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
            abilities.Select((a) => new AbilityData(a.abilityID.ToString(), a.abilityLevel)).ToArray(),
            actions,
            reactions
            );
    }
}