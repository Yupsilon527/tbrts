using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    Resource HealthResource;
    Resource ShieldResource;

    public TextMeshProUGUI HealthValue;
    public Transform HealthFill;
    public Transform ArmorFill;

    public void AssignDamageable(DamageableComponent dmg)
    {
        AssignResource(dmg.Health, dmg.Shield);
    }
    public void AssignResource(Resource health, Resource shield)
    {
        HealthResource = health;
        HealthResource.OnValueChanged.AddListener(() =>
        {
            OnValueChange();
        });
        ShieldResource = shield;
        ShieldResource.OnValueChanged.AddListener(() =>
        {
            OnValueChange();
        });
        OnValueChange();
    }
    private void OnEnable()
    {
        OnValueChange();
    }
    public void OnValueChange()
    {
        if (HealthResource == null)
            return;

        float hValue = HealthResource.GetValue();
        float sValue = ShieldResource.GetValue();
        float hTotal = HealthResource.GetLimit(false);

        HealthFill.localScale = new Vector3(hValue / (sValue + hTotal), 1, 1);
        ArmorFill.localScale = new Vector3((hValue + sValue) / (sValue + hTotal), 1, 1);
        if (HealthValue != null)
            HealthValue.text = (hValue + sValue) + "/" + hTotal;

        /*if (ArmorInfo != null)
            if (ShieldResource.GetValue() > 0)
            {
                ArmorInfo.gameObject.SetActive(true);
                ArmorInfo.text = ShieldResource.GetValue().ToString();
                Armor.SetActive(true);
            }
            else
            {
                ArmorInfo.gameObject.SetActive(false);
                Armor.SetActive(false);
            }*/
    }
}
