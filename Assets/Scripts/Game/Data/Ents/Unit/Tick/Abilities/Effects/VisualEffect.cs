using System.Drawing;
using System.Linq;
using UnityEngine;

public abstract class VisualEffect : ApplyEffects
{
    public float delay;
    public float duration;
    public  CombatantContainer GetContainerForUnit(DataItemUnit unit)
    {
        return InterfaceManager.main.combatWindow.containers.FirstOrDefault(c => c.assignedUnit == unit);
    }
}
