using UnityEngine;

public class DisplayItemArmy : DisplayItemObject<DataItemBanner>
{
    public DataItemBanner assignedArmy;
    public GameObject formation;
    public SpriteRenderer transport;
    public override void AssignObject(DataItemBanner ob)
    {
        base.AssignObject(ob);
        assignedArmy = ob;
        OnPlayerOwnerChange();
        OnPositionChange(ob.GetCoords(), DisplayPositionChange.instant);
        OnGraphicsChange();
        OnSelectionChange();
    }
    public override void OnPositionChange(Vector3 pos, DisplayPositionChange change)
    {
        base.OnPositionChange(pos, change);
        OnPathChange();
    }
    public override void OnSelectionChange()
    {
        OnPathChange();
        base.OnSelectionChange();
        InterfaceManager.main.moveIndicator.ClearList();
        InterfaceManager.main.moveIndicator.DoOrderDisplay(assignedArmy);
    }
    public override void DrawAgain()
    {
        if (assignedArmy.formation.transport != null)
        {
            transport.gameObject.SetActive(true);
            transport.sprite = assignedArmy.formation.transport.data.icon;
            foreach (var sprite in objectSprites)
            {
                sprite.gameObject.SetActive(false);
            }
        }
        else
        {
            transport.gameObject.SetActive(false);
            for (int i = 0; i < objectSprites.Count; i++)
            {
                if (i < assignedArmy.formation.Formation.Length && assignedArmy.formation.Formation[i] != null)
                {
                    var unit = assignedArmy.formation.Formation[i];
                    objectSprites[i].gameObject.SetActive(true);
                    objectSprites[i].sprite = unit.data.GetSprite(unit.IsAlive() ?  CharacterSO.SpriteFrame.idle : CharacterSO.SpriteFrame.dead);
                }
                else
                {
                    objectSprites[i].gameObject.SetActive(false);
                }
            }
        }
    }
    public override void OnPathChange()
    {
        if (assignedArmy.IsSelected())
        {
            InterfaceManager.main.moveIndicator.ClearList();
            InterfaceManager.main.moveIndicator.DoOrderDisplay(assignedArmy);
        }
    }
}
