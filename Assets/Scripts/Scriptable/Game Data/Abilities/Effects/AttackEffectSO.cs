using UnityEngine;

public abstract class AttackEffectSO : ScriptableObject
{
    public float applyChance = 1;
    public CombatDefines.ChanceMult chance;
    public CombatDefines.TargetType targeting;

    public abstract ApplyEffects Translate();
}