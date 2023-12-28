using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionButton : MonoBehaviour
{
    public Image heroBackground;
    public Image heroPortrait;
    public Button heroButton;
    #region AssignedHero
    Hero myHero;
    public void AssignHero(Hero nHero)
    {
        myHero = nHero;
        ChangeInfo();
    }
    void ChangeInfo()
    {
        if (myHero !=null)
        {
            if (heroPortrait != null)
            {
                heroPortrait.sprite = myHero.classComponent.AssignedClass.heroPortrait;
            }
            UpdateState();
        }
    }
    public void UpdateState()
    {
        if (myHero!=null && heroButton != null )
        {
            heroButton.interactable = myHero.damageable.isAlive();
            heroBackground.color = heroButton.interactable ? myHero.classComponent.AssignedClass.heroColor : Color.gray;
        }
    }
    #endregion
    public void OnButtonClicked()
    {
        if (myHero != null)
        {
            if (myHero.IsSelected())
            {
                CameraController.main.MovePosition(myHero.transform.position);
            }
            else
            {
                PlayerController.main.party.SelectSingle(myHero);
            }
        }
    }
}
