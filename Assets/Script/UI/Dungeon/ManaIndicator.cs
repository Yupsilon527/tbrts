

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaIndicator : MonoBehaviour
{
    [Header("Components")]
    public Image crystalIcon;
    public TextMeshProUGUI manaIndicator;
    [Header("Colors")]
    public Color colorCharged;
    public Color colorEmpty;

    public void UpdateValue(Resource res)
    {
        if (manaIndicator!=null)
        manaIndicator.text = $"{res.GetValue()}/{res.GetLimit(false)}";
        if (crystalIcon != null)
            crystalIcon.color = res.GetValue() > 0 ? colorCharged : colorEmpty;
    }

}
