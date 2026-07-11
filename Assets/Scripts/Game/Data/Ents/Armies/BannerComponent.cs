using UnityEngine;

public abstract class BannerComponent 
{
    public DataItemBanner parent;

    protected BannerComponent(DataItemBanner parent)
    {
        this.parent = parent;
    }

    public virtual void OnTurnBegin() { }
}
