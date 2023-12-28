using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Single Melee", menuName = "Abilities/Melee/Single")]
public class MeleeSingleSO : ActiveAbilitySO
{
    public override AbilityDefines.Behavior GetAbilityBehavior()
    {
        return AbilityDefines.Behavior.target;
    }

    public override AbilityDefines.CheckHitMobs CheckTargetMobs()
    
    {
        return AbilityDefines.check_circle_around_target;
    }
}
