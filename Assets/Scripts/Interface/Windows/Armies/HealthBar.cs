using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI value;
    public void AssignResource(Resource res)
    {
        ShowValue(res.GetValue() + "/" + res.GetLimit(), res.GetPercentage());
    }
    public void ShowValue(string text, float percent)
    {
        if (fill != null)
            fill.fillAmount = percent;
        if (value != null)
            value.text = text;
    }
}
