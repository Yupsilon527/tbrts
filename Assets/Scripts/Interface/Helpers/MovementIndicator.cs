using System.Collections.Generic;
using UnityEngine;

public class MovementIndicator : MonoBehaviour
{
    public GameObject elementPrefab;
    List<MovementIndicatorTile> entries = new();
    public void DoOrderDisplay(DataItemArmy army)
    {
        var movement = army.movement.GetMyMovement();
        int movePoints = army.movement.GetMyMovementDistance();
        int mov = 0;
        foreach (var order in army.orders.orders)
        {
            if (order.path != null)
            {
                foreach (var tile in order.path.walkpath)
                {
                    mov += tile.GetMoveCost(movement);

                    float turns = Mathf.Floor((mov - army.movement.movementLeft) / movePoints);

                    string moveLabel = "" + mov;

                    if (turns > 0)
                    {
                        moveLabel += " (" + turns + ")";
                    }
                    if (order.path.walkpath.IndexOf(tile) == order.path.walkpath.Count - 1)
                    {
                        moveLabel = order.OrderID + "<br>" + moveLabel;
                    }
                    LabelIndicator(PoolButton(), tile.gridPos, moveLabel);
                }
            }
            if (order.OrderID == Order.ID.Rest)
            {
                continue;
            }
        }
    }
    MovementIndicatorTile PoolButton()
    {
        foreach (var div in entries)
        {
            if (div != null && !div.gameObject.activeSelf)
            {
                div.gameObject.SetActive(true);
                div.transform.SetAsLastSibling();
                return div;
            }
        }

        GameObject d = Instantiate(elementPrefab, transform);
        if (d.TryGetComponent(out MovementIndicatorTile tind))
        {
            entries.Add(tind);
            d.SetActive(true);
            d.transform.SetAsLastSibling();
            return tind;
        }
        return null;
    }
    void LabelIndicator(MovementIndicatorTile indicator, Vector2Int tile, string label)
    {
        indicator.transform.position = SidewaysMap.main.TranslateEntityPosition(tile);
        indicator.textMesh.text = label;
    }
    public void ClearList()
    {
        foreach (MovementIndicatorTile listle in entries)
        { listle.gameObject.SetActive(false); }
    }
}
