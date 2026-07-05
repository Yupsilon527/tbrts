using UnityEngine.UI;

public class ProductionButton : UnitContainer
{
    public ProductionData produced;
    public Button button;
    public System.Action onClick;
    public ResourceValueIndicator[] abilityCosts;

    public override void ForData(ProductionData data)
    {
        produced = data;
        base.ForData(data);
        ShowCosts(data.GetCostForPlayer(GameManager.main.playerManager.currentPlayer));
    }
    public override void ForTable(ProductionTable data)
    {
        base.ForTable(data);
        ShowCosts(produced.costs);
    }
    public override void ForUnit(DataItemUnit unit)
    {
        base.ForUnit(unit);
        produced = unit.data;
        ShowCosts(unit.data.GetCostForPlayer(GameManager.main.playerManager.currentPlayer));
    }
    public override void Clear()
    {
        base.Clear();
        foreach (var cost  in abilityCosts)
        {
            cost.gameObject.SetActive(false);
        }
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
