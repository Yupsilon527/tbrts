using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelectionController : MonoBehaviour
{
    public HeroSelectionButton[] heroPortraits;

    public void InitalizeHeroes()
    {
        int iHero = 0;
        for (int iB = 0; iB < heroPortraits.Length; iB++)
        {
            if (iHero < PlayerController.main.party.heroes.Count)
            {
                if (PlayerController.main.party.heroes[iHero] is Hero hero)
                {
                    heroPortraits[iB].gameObject.SetActive(true);
                    heroPortraits[iB].AssignHero(hero);
                }
                iHero++;
            }
            else
            {
                heroPortraits[iB].gameObject.SetActive(false);
            }
        }
    }
    public void UpdateButtonStates ()
    {
        foreach (var portrait in heroPortraits)
        {
            portrait.UpdateState();
        }
    }
}
