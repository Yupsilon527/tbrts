using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PropertyAbility : PropertyBase
{
    public float procStrength = 1;
    protected ApplyEffects[] effects;
   
    public PropertyAbility(AbilityData data) :this(data.abilityCooldown, data.abilityCondition, data.abilityTarget,data.abilityEvent, data.abilityStrength)
    {
       
    }
    public DataItemUnit[] GetValidTargets(CastTable table)
    {
        return new[] { table.caster };
    }
    public bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return IsUsable() && HasResourcesToCast();
    }
    public virtual bool HasResourcesToCast()
    {
        return true;
    }
    public virtual void SpendResources()
    {

    }
    public void CastFromTable(CastTable table)
    {
        if (CanBeCast() && (table.targetGem == null || IsValidGemTarget(table.targetGem)))
        {
            var spentMana = caster.mana.GetSelectedGems();
            if (TryCastWithResources(table, original.castEffects[0], spentMana, false))
            {
                for (int i = 1; i < original.castEffects.Length; i++)
                {
                    TryCastWithResources(table, original.castEffects[i], spentMana, true);
                }
            }
            if (table.ability.CheckFlag(AbilityDefines.AbilityFlag.discardUsedGems))
            {
                table.attacker.HandleEvent(AbilityDefines.Event.DiscardGems, table.ability.GetTotalGemCost());
            }
            else
            {
                table.attacker.HandleEvent(AbilityDefines.Event.SpendGems, table.ability.GetTotalGemCost());
            }
            FireCooldown();
        }
    }
    public virtual bool IsUsable()
    {
        return true;
    }
    #region Events
    public void FireEvent(AbilityDefines.Event fct, Mob target)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new CastTable(caster, this, target, new Mob[] { target }));
    }
    public void FireEvent(AbilityDefines.Event fct, Mob[] targets)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new CastTable(caster, this, null, targets));
    }
    public void FireEvent(AbilityDefines.Event fct)
    {
        FireEvent(fct, new CastTable(caster, this));
    }
    public void FireEvent(AbilityDefines.Event fct, CastTable table)
    {
        if (!HasEvent(fct)) return;

        AbilityEvent(fct, table);
    }
    public bool HasEvent(AbilityDefines.Event evt)
    {
        if (AbilityFunctions != null)
            foreach (var fct in AbilityFunctions)
                if (fct.aEvent == evt)
                    return true;
        return false;
    }
    #endregion
    #region Ability Events
    public List<AbilitySO.AbilityListener> AbilityFunctions;
    void AbilityEvent(AbilityDefines.Event fct, CastTable table)
    {
        if (AbilityFunctions != null)
        {
            foreach (var item in AbilityFunctions)
            {
                if (item.aEvent == fct)
                {
                    item.aFunction.Invoke(table);
                }
            }
        }
    }
    #endregion
}
