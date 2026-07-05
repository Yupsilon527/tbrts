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
        if (assignedCastle.occupiedTiles.Count > 0)
        {
            objectSprites = new();
            var topMostTile = assignedCastle.occupiedTiles[0];
            foreach (var tile in assignedCastle.occupiedTiles)
            {
                if (tile.gridPos.x < topMostTile.gridPos.x && tile.gridPos.y < topMostTile.gridPos.y)
                    topMostTile = tile;
                var prefab = GameManager.main.displayPool.PoolItem(SpritePrefab);
                prefab.transform.SetParent(transform);
                prefab.transform.position = tile.GetCenterPosition();
                objectSprites.Add(prefab.GetComponent<SpriteRenderer>());
            }
            if (banner!=null)
            banner.transform.position = topMostTile.GetCenterPosition();
        }
        OnPlayerOwnerChange();
        DrawAgain();
    }
    public override void DrawAgain()
    {
        var citySprite = assignedCastle.citySprite == null ? assignedCastle.GetPlayerOwner().faction.castleTexture : assignedCastle.citySprite;
        foreach (var sprite in objectSprites)
        {
            sprite.gameObject.SetActive(true);
            sprite.sprite = citySprite;
        }
    }
}
