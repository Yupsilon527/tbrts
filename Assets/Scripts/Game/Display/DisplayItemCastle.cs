using UnityEngine;

public class DisplayItemCastle : DisplayItemObject
{
    public DataItemCastle assignedCastle;
    public void AssignCastle(DataItemCastle castle)
    {
        objectSprites = new();
        foreach (var tile in castle.castleTiles)
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
