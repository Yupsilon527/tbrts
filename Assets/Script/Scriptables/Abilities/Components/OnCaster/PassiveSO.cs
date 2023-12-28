using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Passive Ability", menuName = "Abilities/Passive/Passive Ability")]
public class PassiveSO : AbilitySO
{
    public ModifierSO Modifier;
    public override List<AbilitySO.AbilityListener> TranslateFunctions()
    {
        var value = base.TranslateFunctions();

        AbilityDefines.AbilityFunction Refresh =
                (CastTable castData) =>
                {
                    if (Modifier == null)
                    {
                        Debug.LogError("Modifier on " + name + " is null");
                        return;
                    }
                    if (castData == null || castData.caster == null || castData.ability == null)
                    {
                        Debug.LogError("Invalid castdata on " + name);
                    }
                    Modifier.Activate(castData, 0);
                };

        value.Add(new AbilityListener(AbilityDefines.Event.OnCreated, Refresh));
        value.Add(new AbilityListener(AbilityDefines.Event.OnUpgrade, Refresh));
        return value;
    }
}