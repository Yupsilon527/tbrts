

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIndicator : MonoBehaviour
{
    [Header("Components")]
    public Image resIcon;
    public TextMeshProUGUI resIndicator;
    [Header("Colors")]
    public Color colorCharged;
    public Color colorEmpty;

    public void UpdateValue(Resource res)
    {
        if (resIndicator != null)
            resIndicator.text = $"{res.GetValue()}/{res.GetLimit()}";
        if (resIcon != null)
            resIcon.color = res.GetValue() > 0 ? colorCharged : colorEmpty;
    }

}
