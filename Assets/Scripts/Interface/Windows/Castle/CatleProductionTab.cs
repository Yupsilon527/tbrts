using UnityEngine;

public class CatleProductionTab : MonoBehaviour
{
    public DataItemCastle castle;
    public ProgressBarElement productionProgress;
    public ProductionButton[] production;

    private void OnEnable()
    {
        Revise();
    }
    void Revise()
    {
        if (castle == null) return;

        float progress = 0;
        if (castle.production.productionQueue.Count > 0 )
        {
            float cp = castle.production.iProductionTime;
            float np = castle.production.GetFirstItemLaborCost();

            progress = cp / np;
            float turns = (np - cp) / castle.income.baseIncome[(int)EconomyDefines.IncomeResource.Labor];

            productionProgress.value.text = $"{Mathf.Round(progress*100)}% ({Mathf.Ceil(turns)}) Turns)";
        }
        else
        {
            productionProgress.value.text = "Nothing...";
        }

        productionProgress.UpdateProgress(progress);

        for (int i = 0; i< production.Length; i++)
        {
            if (i < BuildingDefines.iCastleProductionMax)
            {
                production[i].gameObject.SetActive(true);
                if (i < castle.production.productionQueue.Count && castle.production.productionQueue[i] is ProductionTable table)
                {
                    production[i].ForTable(table);
                }
                else
                {
                    production[i].Clear();
                }
            }
            else
            {
                production[i].gameObject.SetActive(false);
            }
        }
    }
}
