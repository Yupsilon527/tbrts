using UnityEngine;

public class CombatantContainer : UnitContainer
{
    DataItemUnit owner;
    public HealthBar actionFill;
    public GameObject hpParent, actionParent;

    public override void ForUnit(DataItemUnit unit)
    {
        owner = unit;
        base.ForUnit(unit);
        UpdateLifebar();
        owner.damageable.Health.OnValueChanged.AddListener(UpdateLifebar);

        if (hpParent != null)
        hpParent.SetActive(unit.IsAlive());
        if (actionParent != null)
            actionParent.SetActive(false);
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
            hpFill.AssignResource(owner.damageable.Health);
        if (hpParent != null)
            hpParent.SetActive(owner.damageable.IsAlive());
    }
}
