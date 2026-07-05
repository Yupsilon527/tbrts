using UnityEngine;
using UnityEngine.UI;

public class UnitContainer : MonoBehaviour
{
    public Image unitImage;
    public TMPro.TextMeshProUGUI unitLabel;
    public HealthBar hpFill;

    public virtual void ForUnit(DataItemUnit unit)
    {
        if (unitImage != null)
        {
            unitImage.enabled = true;
            unitImage.sprite = unit.data.GetSprite(CharacterSO.SpriteFrame.idle);
        }
        if (unitLabel != null)
            unitLabel.text = unit.InternalName;
        if (hpFill != null)
            hpFill.AssignResource(unit.health);
    }
    public virtual void ForData(ProductionData data)
    {
        if (data == null)
        {
            Clear();
            return;
        }
        if (data is UnitData unit)
        {
            unitImage.sprite = unit.GetSprite(CharacterSO.SpriteFrame.idle);
        }
        else
        {
            unitImage.sprite = data.icon;
        }
        unitImage.enabled = true;
        unitLabel.text = data.InternalName;
    }
    public virtual void ForTable(ProductionTable data)
    {
        if (data == null)
        {
            Clear();
            return;
        }
        ForData(data.production); ;
    }
    public virtual void Clear()
    {
        if (unitImage != null)
            unitImage.enabled = false;
        if (unitLabel != null)
            unitLabel.text = "";
    }
}
