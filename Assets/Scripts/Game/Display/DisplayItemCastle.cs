using UnityEngine;

public class DisplayItemCastle: DisplayItemObject<DataItemCastle>
{
    public DataItemCastle assignedCastle;
    public override void AssignObject(DataItemCastle ob)
    {
        base.AssignObject(ob);
        objectSprites = new();
        foreach (var tile in ob.castleTiles)
        {
            var prefab = GameManager.main.displayPool.PoolItem(SpritePrefab);
            prefab.transform.position = tile.GetWorldPosition();
            objectSprites.Add(prefab.GetComponent<SpriteRenderer>());
        }
        Redraw();
    }
   public override void Redraw()
    {

    }
}
