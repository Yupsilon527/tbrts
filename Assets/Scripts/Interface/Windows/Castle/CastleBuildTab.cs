using UnityEngine;

public class CastleBuildTab : MonoBehaviour
{
    public UnitContainerDescriptipn unitInfo;
    public ProductionButton[] prodContainers;

    ProductionData selection;
    public void ShowProduction(ProductionData[] production)
    {
        ClearSelection();
        ClearProduction();
        for (int i = 0; i < prodContainers.Length; i++)
        {
            if (i < production.Length)
            {
                var item = production[i];
                prodContainers[i].ForData(production[i]);
                prodContainers[i].gameObject.SetActive(true);
                prodContainers[i].onClick = () => { Select(item); };
            }
            else
            {
                prodContainers[i].gameObject.SetActive(false);
            }
        }
    }
    void Select(ProductionData item)
    {
        unitInfo.ForData(item);
    }
    void ClearSelection()
    {
        unitInfo.Clear();
    }
    void ClearProduction()
    {
        foreach (var container in prodContainers)
        {
            container.gameObject.SetActive(false);
        }
    }
}
