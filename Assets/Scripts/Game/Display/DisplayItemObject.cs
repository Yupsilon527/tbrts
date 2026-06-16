using System.Collections.Generic;
using UnityEngine;

public class DisplayItemObject<tDataItem> : Initializable where tDataItem : DataItemObject
{
    public tDataItem assignedObject;
    public GameObject SpritePrefab;
    public List<SpriteRenderer> objectSprites, bannerSprites;
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
    public virtual void OnPlayerOwnerChange()
    {

    }
}
