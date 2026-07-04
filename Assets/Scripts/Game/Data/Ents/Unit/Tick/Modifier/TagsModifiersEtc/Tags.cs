
using UnityEngine;

public class TagData
{
    public string InternalName;
    public Sprite sprite;
    public ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.always_visible;
    //public VisualEffectSO[] visualEffects;

    public TagData(string internalName, Sprite sprite,  ModifierDefines.VisibleState uibehavior)
    {
        InternalName = internalName;
        this.sprite = sprite;
        this.uibehavior = uibehavior;
    }
    public virtual ModifierDefines.Flag GetFlag()
    {
        return ModifierDefines.Flag.Tag;
    }
}

public class PropertyTag
{
    public string InternalName = "MISSING";
    public DataItemUnit caster, parent;
    public Sprite sprite;
    public ModifierDefines.VisibleState uibehavior;

    public PropertyTag(TagData data, DataItemUnit caster, DataItemUnit parent=null) :this(data.InternalName,caster,parent,data.sprite,data.uibehavior)
    {

    }
    public PropertyTag(string internalName, DataItemUnit caster, DataItemUnit parent = null, Sprite sprite = null,  ModifierDefines.VisibleState uibehavior = ModifierDefines.VisibleState.hidden)
    {
        InternalName = internalName;
        this.caster = caster;
        if (parent == null)
        this.parent = caster;
        else
        this.parent = parent;
        this.sprite = sprite;
        this.uibehavior = uibehavior;
    }
    #region Display
    public Sprite GetModifierIcon()
    {
        if (sprite != null)
        {
            return sprite;
        }
        return null;
    }
    public bool IsTooltipVisible()
    {
        return uibehavior >= ModifierDefines.VisibleState.tooltip_only;
    }
    public bool IsOverheadVisible()
    {
        return uibehavior >= ModifierDefines.VisibleState.always_visible;
    }
    #endregion
    public virtual ModifierDefines.Flag GetFlag()
    {
        return ModifierDefines.Flag.Tag;
    }
    public bool dead = false;

    public virtual void Die(bool expire)
    {
        if (!dead)
        {
            dead = true;
        }
    }
    protected void CheckExpiration()
    {
        if (IsExpired())
        {
            Die(true);
        }
    }
    public virtual bool IsExpired()
    {
        return false;
    }
}