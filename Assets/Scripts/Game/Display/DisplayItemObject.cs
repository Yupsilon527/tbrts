using System.Collections.Generic;
using UnityEngine;

public class DisplayItemObject : Initializable
{
    public GameObject SpritePrefab;
    public List<SpriteRenderer> objectSprites, bannerSprites;

    public virtual void Redraw()
    {

    }
}
