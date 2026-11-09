public class AnimationEffect : VisualEffect
{
    public SpriteAnimation animation;

    public override void ActivateOnUnit(EventTable table, float strength = 1)
    {
        if (SanityCheck() && GetContainerForUnit(table.target) is CombatantContainer combatant) { 
            combatant.animator.PlayAnimation(animation);
            InterfaceManager.main.combatWindow.animations.animations.Add(new CountdownAction(animation.duration));
    }
    }
}