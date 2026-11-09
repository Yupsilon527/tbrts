using UnityEngine;

public class ColorEffect : VisualEffect
{
    public Color color;

    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        if (SanityCheck() && GetContainerForUnit(table.target) is CombatantContainer combatant)
        {
            combatant.FlashColor(color, delay, duration);
            InterfaceManager.main.combatWindow.animations.animations.Add(new CountdownAction(delay + duration));
        }
    }
}
