using System;
using UnityEngine;
using UnityEngine.UI;

public class ProductionButton : UnitContainer
{
    public ProductionData produced;
    public Button button;
    public Action onClick;
    public ResourceValueIndicator[] abilityCosts;

    public void ForProduction()
    {
        ShowCosts(produced.GetCostForPlayer(GameManager.main.playerManager.currentPlayer));
    }
    void ShowCosts(ResourceCost[] costs)
    {

        for (int i = 0; i < abilityCosts.Length; i++)
        {
            if (i < costs.Length)
            {
                abilityCosts[i].UpdateValue(costs[i]);
                abilityCosts[i].gameObject.SetActive(costs[i].value != 0);
            }
            else
            {
                abilityCosts[i].gameObject.SetActive(false);
            }
        }
    }
    public virtual void OnPressed()
    {
        onClick.Invoke();
    }
}
