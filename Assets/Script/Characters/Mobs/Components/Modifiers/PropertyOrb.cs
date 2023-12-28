using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyOrb : PropertyModifier
{
    public AbilityEffect[] appliedAttacks;
    public PropertyOrb(OrbData data, PropertyAbility source) : base(data, source)
    {
        appliedAttacks = data.AttackDatas;
    }
}
