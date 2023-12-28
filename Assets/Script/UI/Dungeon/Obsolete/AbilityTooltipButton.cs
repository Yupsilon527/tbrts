using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityTooltipButton : TooltipButton
{
    #region Assigned Ability
    protected PropertyAbility assignedAbility;

    public void TieToAbility(PropertyAbility ability)
    {
        if (ability == null )
            hierarchyParent.gameObject.SetActive(false);
        else
        {
            hierarchyParent.gameObject.SetActive(true);
            assignedAbility = ability;
            UpdateInterface();
        }
    }
    public virtual void UpdateInterface()
    {
        if (assignedAbility != null)
        {
            buttonComponent.interactable = assignedAbility.AbilityBehavior != AbilityDefines.Behavior.passive;
        }
        UpdateAbilityVisual();
    }

    #endregion
    #region Components
    public Button buttonComponent;

    protected override void Awake()
    {
        if (hierarchyParent == null)
            hierarchyParent = transform;
        base.Awake();

        if (buttonComponent == null)
            buttonComponent = GetComponent<Button>();
        if (buttonComponent == null)
            buttonComponent = GetComponentInChildren<Button>();
        //if (BtnIcon == null)
         //   BtnIcon = buttonComponent.transform.Find("Icon")?.GetComponent<Image>();

        InitFunctions();

    }
    protected virtual void InitFunctions()
    {
        buttonComponent.onClick.AddListener( () =>
        {
            DungeonInterfaceController.main.mainHero.abilities.CastAbility(assignedAbility);
        });
    }
    #endregion
    #region Update Visual
    public virtual void UpdateAbilityVisual()
    {
        if (assignedAbility!= null && assignedAbility != null)
        {
            ChangeIcon(assignedAbility.original.GetIconSprite()); 
            if (buttonComponent!=null)
            buttonComponent.interactable = assignedAbility.IsFullyCastable();
        }
        else
        {
            ChangeIcon(null);
            if (buttonComponent != null)
                buttonComponent.interactable = false;
        }
        }
    #endregion
}
