using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceValueIndicator : Initializable
{
    public Image resourceIcon;
    public TextMeshProUGUI resourceValue;
    public void UpdateValue(EconomyDefines.EconomyResource resource, float value)
    {
        UpdateValue(resource, $"{value}");
    }
    public void UpdateValue(EconomyDefines.EconomyResource resource, string value)
    {
        if (resourceIcon != null)
        {
            Inspect("Load Image " + resource);
         //   resourceIcon.sprite = EconomyDefines.LoadResourceSprite(resource);
        }
        if (resourceValue != null)
        {
            Inspect("Set Value " + value);
            resourceValue.text = value;
        }
    }
    public void UpdateValue(ResourceCost cost)
    {
        UpdateValue(cost.resource, cost.value);
    }
    public void UpdateValue(EconomyDefines.EconomyResource resource, Resource res)
    {
        UpdateValue(resource, res.GetValue());
    }
}
