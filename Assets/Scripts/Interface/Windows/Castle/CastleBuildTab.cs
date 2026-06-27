using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastleBuildTab : MonoBehaviour
{
    public UnitContainerDescriptipn unitInfo;
    public SelectableUnitContainer[] prodContainers;

    ProductionData selection;
    public void ShowProduction(ProductionData[] production)
    {
        for (int i = 0; i < prodContainers.Length; i++)
        {
            if (i < production.Length)
            {
                prodContainers[i].ForData(production[i]);
                prodContainers[i].gameObject.SetActive(true);
            }
            else
            {
                prodContainers[i].gameObject.SetActive(false);
            }
        }
    }
    void Select()
    {

    }
    void ClearSelectiob()
    {

    }
    void ClearProduction()
    {
        foreach (var container in prodContainers)
        {
            container.gameObject.SetActive(false);
        }
    }
}
