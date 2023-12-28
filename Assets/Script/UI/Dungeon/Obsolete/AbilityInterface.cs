using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInterface : MonoBehaviour
{
    public ToggleTooltipButton attackButton;
    public AbilityTooltipButton[] abilityButtons;
    public RectTransform rectTransform;
    protected virtual void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (attackButton == null)
            attackButton = GetComponentInChildren<ToggleTooltipButton>();
        if (abilityButtons == null)
            abilityButtons = GetComponentsInChildren<AbilityTooltipButton>();
    }
    public void UpdateAbilityButtons(List<Hero> selectedHeroes)
    {

        foreach (Hero hero in selectedHeroes)
        {
            heroabilites.AddRange(hero.abilities.GetAvailableSpells(true));
        }
        ToggleAutoAttackButton(selectedHeroes.Count == 1);
        if (selectedHeroes.Count == 1)
        {
            attackButton.TieToAbility(selectedHeroes[0].GetSelectedAttack());
        }
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < heroabilites.Count)
            {
                abilityButtons[i].TieToAbility(heroabilites[i]);
                abilityButtons[i].gameObject.SetActive(true);
            }
            else
            {
                abilityButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void ToggleAutoAttackButton(bool enabled)
    {
        if (enabled)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f, rectTransform.anchoredPosition.y);
        }
        else
        {
            RectTransform atb = attackButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f - atb.sizeDelta.x, rectTransform.anchoredPosition.y);

            attackButton.TieToAbility(null);
        }
    }

    List<PropertyAbility> heroabilites = new List<PropertyAbility>();
    public void ClearAbilities()
    {
        heroabilites.Clear();
    }
    public void Refresh()
    {
        if (attackButton.gameObject.activeSelf)
            attackButton.UpdateAbilityVisual();
        foreach (AbilityTooltipButton btn in abilityButtons)
            btn.UpdateAbilityVisual();
    }
}
