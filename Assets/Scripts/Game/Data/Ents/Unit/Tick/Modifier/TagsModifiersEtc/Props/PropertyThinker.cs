using System.Collections.Generic;
using UnityEngine;

public class PropertyThinker : PropertyIha, ITimerAction
{

    // Thinker
    public bool HasThinker = false;
    public bool executed = false;
    public int lastThink = 0;
    public int thinkInterval = 0;

    public PropertyThinker(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null, ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden, int p = 0, int thinkInterval = 0, ModifierDefines.StateData[] sa = null, ModifierDefines.PropertyData[] pr = null, AbilityData[] grantedAbilities = null,  HashSet<AbilityFunction>  functions = null) : base(internalName, caster, parent, sprite, uibehavior, p, sa, pr, grantedAbilities,functions)
    {
        this.thinkInterval = thinkInterval;
        HasThinker = thinkInterval > 0;
    }

    #region Thinker
    public void StartThinker(int interval)
    {
        HasThinker = true;
        thinkInterval = Mathf.Max(1, interval);
        lastThink = 0;
    }
    public virtual void Think()
    {
        lastThink += thinkInterval;
    }
    #endregion

    public void ExtendCooldown(float cdr = 1)
    {
        Delay((int)(thinkInterval * cdr));
    }
    public void SetCooldown(int cooldown)
    {
        lastThink = cooldown;
    }
    public virtual bool RefreshCooldown(int cooldown)
    {
        if (HasThinker) lastThink -= cooldown;
        executed = executed || lastThink < 0;
        while (lastThink < 0)
            Think();
        return executed;
    }
    public virtual void Delay(int cooldown)
    {
        lastThink += cooldown;
    }
    public virtual void Reset()
    {
        lastThink = 0;
        executed = false;
    }
}
