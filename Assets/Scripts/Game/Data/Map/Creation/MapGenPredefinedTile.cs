using UnityEngine;

public class MapGenPredefinedTile : MonoBehaviour
{
    public MapGenEditor mge;
    public Vector2Int gridPos;
    public ObjectDataSO objectData;

    public char elevation = '-';
    public int variation = 0;

    private void OnValidate()
    {
        mge = GetComponentInParent<MapGenEditor>();
        gridPos = SidewaysMap.TranslateWorldPosition(transform.position);
        name = "Segment x" + gridPos.x + "_y" + gridPos.y + " " + elevation+variation;
        UpdateSprite();
    }
    void UpdateSprite()
    {
        if (mge == null) return;
        GetComponent<SpriteRenderer>().color = Color.magenta;
        foreach (var item in mge.biomeData.Elevations)
        {
            if (item.CharID == elevation)
            {
                var sprite = item.GetSprite(0, variation);
                if (sprite != null)
                {
                    GetComponent<SpriteRenderer>().sprite = sprite;
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
}
