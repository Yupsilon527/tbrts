using System.Collections.Generic;
using UnityEngine;

public class DisplayItemObject<tDataItem> : Initializable where tDataItem : DataItemObject
{
    public tDataItem assignedObject;
    public GameObject SpritePrefab;
    public List<SpriteRenderer> objectSprites, bannerSprites;
    public DisplayBanner banner;
    public virtual void AssignObject(tDataItem ob)
    {
        assignedObject = ob;
    }
    public virtual void DrawFresh()
    {

    }
    public virtual void DrawAgain()
    {

    }
    public virtual void OnGraphicsChange()
    {

    }
    public virtual void OnPlayerOwnerChange()
    {
        banner?.ChangePlayer(assignedObject.GetPlayerOwner());
    }
    public virtual void OnSelectionChange()
    {

    }
}
