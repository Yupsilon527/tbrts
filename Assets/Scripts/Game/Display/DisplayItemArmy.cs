using UnityEngine;

public class DisplayItemArmy : DisplayItemObject<DataItemArmy>
{
    public DataItemArmy assignedArmy;
    public GameObject formation;
    public SpriteRenderer transport;
    public override void AssignObject(DataItemArmy ob)
    {
        base.AssignObject(ob);
        assignedArmy = ob;
        OnPlayerOwnerChange();
        OnPositionChange(ob.gridPos, DisplayPositionChange.instant);
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
                    objectSprites[i].gameObject.SetActive(true);
                    objectSprites[i].sprite = assignedArmy.formation.Formation[i].data.GetSprite(CharacterSO.SpriteFrame.idle);
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
