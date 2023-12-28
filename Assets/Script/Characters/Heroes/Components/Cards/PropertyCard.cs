using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCard : PropertyAbility
{
    public PropertyCard(Mob owner, AbilitySO Data) : base(owner, Data)
    {
    }
    public override bool CanBeCast()
    {
        return GetCastBehavior() != AbilityDefines.Behavior.passive;
    }
}
