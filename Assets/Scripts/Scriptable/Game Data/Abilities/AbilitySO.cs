using System;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Ability")]
public class AbilitySO : ScriptableObject
{
    public CombatantAbilityTable data;
}