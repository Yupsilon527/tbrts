using System.Collections.Generic;

public abstract class PropertyAbility : PropertyAction
{
    public float procStrength = 1;
    public int uses = 0;

    public DataItemUnit[] GetValidTargets(CastTable table)
    {
        return new[] { table.caster };
    }
    public ApplyEffects[] GetAbilityEffects() { return null; }
    public virtual bool CanBeCast(CombatDefines.AttackPhase phase)
    {
        return HasResourcesToCast();
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
    #region Events
    public void FireEvent(AbilityDefines.Event fct, DataItemUnit target)
    {
        if (!HasEvent(fct)) return;


        FireEvent(fct, new CastTable(parent, UnityEngine.Vector2Int.zero, this)) ;
    }
    public void FireEvent(AbilityDefines.Event fct)
    {
        FireEvent(fct, new CastTable(caster, source:this));
    }
    public void FireEvent(AbilityDefines.Event fct, AttackTable table)
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
    public List<AbilityDefines.AbilityListener> AbilityFunctions;
    void AbilityEvent(AbilityDefines.Event fct, EventTable table)
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
    public abstract DataItemUnit[] GetMainTargets(CastTable table);
    public abstract DataItemUnit[] GetSideTargets(CastTable table);
}
