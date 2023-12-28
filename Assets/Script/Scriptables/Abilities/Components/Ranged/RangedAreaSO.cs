
using UnityEngine;
[CreateAssetMenu(fileName = "Area Around Caster", menuName = "Abilities/Misc/Area Around Caster")]
public class RangedAreaSO : ActiveAbilitySO
{
    public int AreaOfEffect = 0;

    public override AbilityDefines.CheckHitMobs CheckTargetMobs()
    {
        return AbilityDefines.check_circle_around_self;
    }
    public override AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.self;
    }
    public override float GetMaxRange()
    {
        return AreaOfEffect;
    }
    public override float GetAoERange()
    {
        return AreaOfEffect;
    }
}
