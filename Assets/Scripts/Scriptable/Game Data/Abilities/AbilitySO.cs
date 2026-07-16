using System;
using UnityEngine;

public abstract class ActionSO : ScriptableObject
{
    public AttackEffectSO[] effects;

    public abstract ActionData Translate();
}


public class ActionData : BaseData
{
    public int abilityFlags;
    public ApplyEffects[] effects;

    public virtual ActionData Clone()
    {
        return MemberwiseClone() as ActionData;
    }
}
