using UnityEngine;

public class DisplayItemCastle : DisplayItemObject<DataItemCastle>
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
        if (assignedCastle.castleTiles.Count > 0)
        {
            objectSprites = new();
            var topMostTile = assignedCastle.castleTiles[0];
            foreach (var tile in assignedCastle.castleTiles)
            {
                if (tile.gridPos.x < topMostTile.gridPos.x && tile.gridPos.y < topMostTile.gridPos.y)
                    topMostTile = tile;
                var prefab = GameManager.main.displayPool.PoolItem(SpritePrefab,transform);
                prefab.transform.position = tile.GetCenterPosition();
                objectSprites.Add(prefab.GetComponent<SpriteRenderer>());
            }
            if (banner!=null)
            banner.transform.position = topMostTile.GetCenterPosition();
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
