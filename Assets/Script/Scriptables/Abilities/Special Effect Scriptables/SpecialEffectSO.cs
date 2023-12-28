using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "On Point", menuName = "Abilities/Special Effects/On Point")]
public class SpecialEffectSO : ScriptableObject
{
    public enum EmitTime : int
    {
        start = 0,
        contact = 2,
        late = 4,
        end = 6
    }

    public bool ScaleWithCaster = false;
    public EmitTime EmitDelay = EmitTime.contact;
    public GameObject EffectPrefab;

    public virtual void MakeEffect(CastTable castData, float delay)
    {
         SpecialEffectPool.main.EffectFromPrefab(EffectPrefab,  castData.GetPointTarget(false), delay + GetDelay(), Mathf.Max(1, 1 + castData.ability.GetAreaRange(false)));
    }
    public float GetDelay()
    {
        return (float)EmitDelay * .1f;
    }
}
