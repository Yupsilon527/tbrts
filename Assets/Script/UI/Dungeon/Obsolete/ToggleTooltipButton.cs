using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTooltipButton : AbilityTooltipButton
{
    protected override void InitFunctions()
    {
        buttonComponent.onClick.AddListener(() =>
        {
            if (assignedAbility != null && assignedAbility.caster != null && assignedAbility.caster is Hero mainHero)
            {
                mainHero.CycleAttack();
                TieToAbility(mainHero.GetSelectedAttack());
            }
        });
    }
    public override void UpdateAbilityVisual()
    {
        if (assignedAbility != null && assignedAbility != null)
        {
            ChangeIcon(assignedAbility.original.GetIconSprite());
        }
        else
        {
            ChangeIcon(null);
        }
    }
}
