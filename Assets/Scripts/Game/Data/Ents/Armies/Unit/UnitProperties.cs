using UnityEngine;


public class UnitProperties : UnitComponent
{
    public int[] states = new int[(int)ModifierDefines.modStates.total];
    public float[] properties = new float[(int)ModifierDefines.Properties.total];

    protected bool propRefresh = false;
    protected bool statRefresh = false;
    protected bool HasUpdates = false;

    public override void TriggerFuncs(AbilityDefines.Event act)
    {
        base.TriggerFuncs(act);
        if (act == AbilityDefines.Event.CombatBegin)
        {
            Refresh(true);
        }
    }
    public UnitProperties(DataItemUnit parent) : base(parent)
    {
    }
    #region States
    public bool GetState(ModifierDefines.modStates State)
    {
        return states[(int)State] > 0;
    }
    protected void UpdateState(ModifierDefines.modStates State, int value)
    {
        if ((int)State >= 0 && (int)State < (int)ModifierDefines.modStates.total)
            return;

        //if (Mathf.Abs(states[(int)State]) < Mathf.Abs(value))
        {
            states[(int)State] += value;
        }
    }
    #endregion
    #region Properties
    public float GetPropertyAdditive(ModifierDefines.Properties p)
    {
        if (p < 0 || (int)p >= properties.Length)
            return 0;
        return properties[(int)p];
    }
    public float GetPropertyMultiplicative(ModifierDefines.Properties p)
    {
        if (p < 0 || (int)p >= properties.Length)
            return 1;
        return 1 + properties[(int)p];
    }
    protected void UpdateProperty(ModifierDefines.Properties Property, float value)
    {
        if ((int)Property < 0 || (int)Property >= (int)ModifierDefines.Properties.total)
            return;
        RefreshProperties();
        if (ModifierDefines.IsPropertyMultiplicative(Property))
            properties[(int)Property] = (1f + properties[(int)Property]) * (value - 1);
        else
            properties[(int)Property] += value;
    }
    #endregion
    #region Refresh
    public void RefreshProperties()
    {
        propRefresh = true;
    }
    public void RefreshStates()
    {
        statRefresh = true;
    }
    public virtual void Refresh(bool force = false)
    {
        if (force || statRefresh || propRefresh)
        {
            if (force || statRefresh) states = new int[(int)ModifierDefines.modStates.total];
            if (force || propRefresh) properties = new float[(int)ModifierDefines.Properties.total];
        }
        propRefresh = false;
        statRefresh = false;
    }
    #endregion
}
