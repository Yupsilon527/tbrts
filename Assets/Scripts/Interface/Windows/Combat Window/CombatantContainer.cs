using UnityEngine;
using UnityEngine.UI;

public class CombatantContainer : UnitContainer
{
    DataItemUnit owner;
    public Image hpFill, actionFill;
    public GameObject hpParent, actionParent;

    public override void ForUnit(DataItemUnit unit)
    {
        owner = unit;
        base.ForUnit(unit);
        UpdateLifebar();
        owner.damageable.Health.OnValueChanged.AddListener( UpdateLifebar);

        hpParent?.SetActive(unit.IsAlive());
        actionParent?.SetActive(false);
    }
    public override void Clear()
    {
        if (owner != null)
        {
            owner.damageable.Health.OnValueChanged.RemoveListener(UpdateLifebar);
        }
        base.Clear();
    }
    void UpdateLifebar()
    {
        if (hpFill != null)
            hpFill.fillAmount = owner.damageable.Health.GetPercentage();
    }
}
