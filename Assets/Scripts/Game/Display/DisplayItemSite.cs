using UnityEngine;

public class DisplayItemSite : DisplayItemObject<DataItemSite>
{
    public DataItemSite assignedSite;
    public override void AssignObject(DataItemSite ob)
    {
        base.AssignObject(ob);
        assignedSite = ob;
        DrawFresh();
    }
    public override void DrawFresh()
    {
            objectSprites = new();
            foreach (var tile in assignedSite.occupiedTiles)
            {
                var prefab = GameManager.main.displayPool.PoolItem(SpritePrefab);
                prefab.transform.SetParent(transform);
                prefab.transform.position = tile.GetCenterPosition();
                objectSprites.Add(prefab.GetComponent<SpriteRenderer>());
            }
        base.DrawFresh();
    }
    public override void DrawAgain()
    {
        foreach (var s in objectSprites)
        {
            s.sprite = assignedSite.ruinSprite;
            s.color = assignedSite.IsVisited() ? Color.gray: Color.white;
        }
        base.DrawAgain();
    }
}
