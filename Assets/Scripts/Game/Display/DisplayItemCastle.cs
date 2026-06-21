using UnityEngine;

public class DisplayItemCastle: DisplayItemObject<DataItemCastle>
{
    public DataItemCastle assignedCastle;
    public override void AssignObject(DataItemCastle ob)
    {
        base.AssignObject(ob);
        assignedCastle = ob;
        DrawFresh();
    }
    public override void DrawFresh()
    {
        objectSprites = new();
        var topMostTile = assignedCastle.castleTiles[0];
        foreach (var tile in assignedCastle.castleTiles)
        {
            if (tile.gridPos.x < topMostTile.gridPos.x && tile.gridPos.y < topMostTile.gridPos.y)
                topMostTile = tile;
            var prefab = GameManager.main.displayPool.PoolItem(SpritePrefab);
            prefab.transform.position = tile.GetWorldPosition();
            objectSprites.Add(prefab.GetComponent<SpriteRenderer>());
        }
        DrawAgain();
    }
    public override void DrawAgain()
    {
        foreach (var sprite in objectSprites)
        {
            sprite.gameObject.SetActive(true);
            sprite.sprite = assignedCastle.citySprite;
        }
    }
}
