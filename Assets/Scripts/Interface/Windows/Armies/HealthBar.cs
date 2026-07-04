using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI value;
    public void AssignResource(Resource res)
    {
        if (fill!=null)
        fill.fillAmount = res.GetPercentage();
        if (value != null)
            value.text = res.GetValue() + "/" + res.GetLimit();
    }
}
