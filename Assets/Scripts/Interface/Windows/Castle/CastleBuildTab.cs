using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CastleBuildTab : MonoBehaviour
{
    public UnitContainerDescriptipn unitInfo;
    public ProductionButton[] prodContainers;

    public Button productionButton;

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
        selection = item;
        unitInfo.ForData(selection);
        productionButton.enabled = item.GetAvailableState(InterfaceManager.main.castleWindow.assignedCastle) == ProductionData.AvailableState.available;
    }
    void ClearSelection()
    {
        selection = null;
        unitInfo.Clear();
        productionButton.enabled = false;
    }
    void ClearProduction()
    {
        foreach (var container in prodContainers)
        {
            container.gameObject.SetActive(false);
        }
    }
    public void ProduceSelectedUnit()
    {
        if (selection!=null)
        {
            InterfaceManager.main.castleWindow.QueueProduction(selection, selection is not UnitData);
        }
    }
}
