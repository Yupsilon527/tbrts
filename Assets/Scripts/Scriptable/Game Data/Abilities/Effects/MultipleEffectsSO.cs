
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Multiple", menuName = "Abilities/Effects/Multiple")]
public class MultipleEffectsSO : AttackEffectSO
{
    public AttackEffectSO[] list;
    public override ApplyEffects[] Translate()
    {
        return list.SelectMany(e => e.Translate()).ToArray();
    }
}
