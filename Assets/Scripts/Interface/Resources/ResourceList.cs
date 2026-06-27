using UnityEngine;

public class ResourceList : MonoBehaviour
{
    public ResourceValueIndicator[] resourceIndicators = new ResourceValueIndicator[(int)EconomyDefines.EconomyResource.Total];

    public void DisplayCosts(ResourceCost[] resources)
    {
        for (int i = 0; i< resourceIndicators.Length; i++)
        {
            resourceIndicators[i].gameObject.SetActive(i < resources.Length);
            if (resourceIndicators[i].gameObject.activeSelf)
            {
                resourceIndicators[i].UpdateValue(resources[i]);
            }
        }
    }
    public void Clear()
    {
        for (int i = 0; i < resourceIndicators.Length; i++)
        {
            resourceIndicators[i].gameObject.SetActive(false);
        }
    }
}
