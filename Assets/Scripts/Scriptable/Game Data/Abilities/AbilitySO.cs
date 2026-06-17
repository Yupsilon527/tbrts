using System;
using UnityEngine;

public class ActionSO : ScriptableObject
{
    public AttackEffectSO[] effects;
}


public class ActionData : BaseData
{
    public ApplyEffects[] effects;
}
