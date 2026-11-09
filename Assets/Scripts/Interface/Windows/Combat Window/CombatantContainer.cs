using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatantContainer : UnitContainer
{
    public DataItemUnit assignedUnit;
    public Image colorOverlay;
    public FlatAnimator animator;
    public HealthBar actionFill;
    public GameObject hpParent, actionParent;

    public override void ForUnit(DataItemUnit unit)
    {
        assignedUnit = unit;
        base.ForUnit(unit);
        animator.character = unit.data.armySprites;
        assignedUnit.combatDisplay ??= this;
        assignedUnit.damageable.Health.OnValueChanged.AddListener(UpdateLifebar);
        Refresh();
        ResetColor();
    }
    public void Refresh()
    {
        if (assignedUnit == null) return;

        animator.ClearAnimation();
        UpdateLifebar();
        if (hpParent != null)
            hpParent.SetActive(assignedUnit.IsAlive());
        if (actionParent != null)
            actionParent.SetActive(false);
    }
    public override void Clear()
    {
        if (assignedUnit != null)
        {
            if (assignedUnit .combatDisplay == this)
                assignedUnit.combatDisplay =null;
            assignedUnit.damageable.Health.OnValueChanged.RemoveListener(UpdateLifebar);
        }
        base.Clear();
    }
    void UpdateLifebar()
    {
        if (hpFill != null)
            hpFill.AssignResource(assignedUnit.damageable.Health);
        if (hpParent != null)
            hpParent.SetActive(assignedUnit.damageable.IsAlive());
    }
    public void FlashColor(Color color, float delay = 0, float duration = 1)
    {
        StartCoroutine(FlashCoroutine(color, delay, duration));
    }
    private IEnumerator FlashCoroutine(Color color, float d = 0, float t = 1)
    {
        yield return new WaitForSeconds(d);

        Color originalColor = colorOverlay.color;

        float fadeIn = t * .3f;
        float fadeOut = t * .7f;

        float elapsed = 0f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.deltaTime;
            colorOverlay.color = Color.Lerp(Color.clear, color, elapsed / fadeIn);
            yield return null;
        }

        colorOverlay.color = color;

        elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            colorOverlay.color = Color.Lerp(color, originalColor, elapsed / fadeOut);
            yield return null;
        }

        colorOverlay.color = originalColor;
    }
    public void ResetColor()
    {
        colorOverlay.color = Color.clear;
    }
}
