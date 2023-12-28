using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingAssistant : MovementIndicator
{
    public GameObject rangeIndicator;
    public SpriteRenderer areaIndicator;

    public static CastingAssistant main;
    private void Awake()
    {
        main = this;
        gameObject.SetActive(false);
    }
    protected override void Update()
    {
        UpdateSpellIndicator();
        base.Update();
        UpdateTargetMobIndicator();
    }

    #region AssignedSpell
    PropertyAbility assignedSpell;
    public void AssignSpell(PropertyAbility spell)
    {
        assignedSpell = spell;
        SetOwner(spell.caster);
        ConfigSpellComponents();
    }
    void ConfigSpellComponents()
    {
        if (assignedSpell != null)
        {
            switch (assignedSpell.original.GetAbilityBehavior())
            {
                case AbilityDefines.Behavior.self:
                    if (lineRenderer != null)
                    {
                        lineRenderer.gameObject.SetActive(false);
                    }
                    if (rangeIndicator != null)
                    {
                        rangeIndicator.gameObject.SetActive(false);
                    }
                    if (areaIndicator != null)
                    {
                        areaIndicator.gameObject.SetActive(true);
                    }
                    break;
                case AbilityDefines.Behavior.point:
                case AbilityDefines.Behavior.target:
                    if (lineRenderer != null)
                    {
                        lineRenderer.gameObject.SetActive(true);
                    }
                    if (rangeIndicator != null)
                    {
                        rangeIndicator.gameObject.SetActive(true);
                    }
                    if (areaIndicator != null)
                    {
                        areaIndicator.gameObject.SetActive(true);
                    }
                    break;
            }
            destinationIndicator.gameObject.SetActive(false);
            ConfigRangeIndicator();
        }
    }
    void ConfigRangeIndicator()
    {
        float area = assignedSpell.GetAreaRange(false);
        areaIndicator.gameObject.SetActive(area > 0);
       
            areaIndicator.transform.localScale = Vector3.one * area * 2;
        

        rangeIndicator.transform.localScale = Vector3.one * assignedSpell.GetMaxRange(false) * 2;
        
    }
    void UpdateSpellIndicator()
    {
        if (assignedSpell == null) return;
        rangeIndicator.transform.position = origin;
        switch (assignedSpell.original.GetAbilityBehavior())
        {
            case AbilityDefines.Behavior.self:
                areaIndicator.transform.position = origin;
                break;
            case AbilityDefines.Behavior.point:
                areaIndicator.transform.position = destination;
                break;
            case AbilityDefines.Behavior.target:
                areaIndicator.transform.position = destination;
                if (destinationIndicator != null)
                {
                    destinationIndicator.gameObject.SetActive(targetMob != null);
                }
                if (areaIndicator != null)
                {
                    areaIndicator.gameObject.SetActive(targetMob != null);
                }
                if (lineRenderer != null)
                {
                    lineRenderer.gameObject.SetActive(targetMob != null);
                }
                break;
        }
    }
    Mob targetMob = null;
    void UpdateTargetMobIndicator()
    {
        if (destinationIndicator != null)
        {
            destinationIndicator.gameObject.SetActive(targetMob != null);
            if (targetMob != null) {
                destinationIndicator.transform.position = targetMob.transform.position;
                ChangeColor(assignedSpell.IsValidTarget(targetMob) ? Color.white: Color.red);
                    }
            else
            {
                if (assignedSpell.RequiresUnitTarget())
                    ChangeColor(Color.red);
                else
                    ChangeColor(assignedSpell.CanCastOnPoint(destination) ?  Color.white : Color.red);//TODO define colors
            }
        }
    }
    public void AdjustDestination(Vector2 pos)
    {
        destination = CameraController.main.ScreenToWorldPoint(pos);
        targetMob = CameraController.main.MobFromWorldPointForAbility(destination, assignedSpell);
    }
    #endregion
    public override void ChangeColor(Color c)
    {
        base.ChangeColor(c);
        areaIndicator.color = c;
    }
}
