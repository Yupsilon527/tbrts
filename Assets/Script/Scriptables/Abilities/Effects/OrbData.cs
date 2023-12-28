using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Orb Attack", menuName = "Abilities/Effects/Orb Attack")]
public class OrbData : ModifierData
{
    public AbilityEffect[] AttackDatas;
    public OrbData(OrbSO scriptable) : base(scriptable)
    {
        sprite = scriptable.sprite;
    }
}
