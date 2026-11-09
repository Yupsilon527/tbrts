using System.Linq;

public abstract class VisualEffect : ApplyEffects
{
    public float delay;
    public float duration;
    public  CombatantContainer GetContainerForUnit(DataItemUnit unit)
    {
        return InterfaceManager.main.combatWindow.containers.FirstOrDefault(c => c.assignedUnit == unit);
    }
    public bool SanityCheck()
    {
        return Combat.main.combatMode == Combat.CombatMode.animated;
    }
}
