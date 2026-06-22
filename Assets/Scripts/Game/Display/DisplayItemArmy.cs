using UnityEngine;

public class DisplayItemArmy : DisplayItemObject<DataItemArmy>
{
    public DataItemArmy assignedArmy;
    public SpriteRenderer transport;
    public override void AssignObject(DataItemArmy ob)
{
    base.AssignObject(ob);
    assignedArmy = ob;
        OnPlayerOwnerChange();
        OnPositionChange(ob.gridPos, DisplayPositionChange.instant);
        OnGraphicsChange();
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
            for (int i = 0; i< objectSprites.Count; i++)
            {
                if (i< assignedArmy.formation.Formation.Length && assignedArmy.formation.Formation[i] != null)
                {
                    objectSprites[i].gameObject.SetActive(true);
                    objectSprites[i].sprite = assignedArmy.formation.Formation[i].data.icon;
                }
                else
                {
                    objectSprites[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
