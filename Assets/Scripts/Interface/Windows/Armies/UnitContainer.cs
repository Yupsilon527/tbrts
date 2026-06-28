using UnityEngine;
using UnityEngine.UI;

public class UnitContainer : MonoBehaviour
{
    public Image unitImage;
    public TMPro.TextMeshProUGUI unitLabel;

    public virtual void ForUnit(DataItemUnit unit)
    {
        unitImage.sprite = unit.data.GetSprite(CharacterSO.SpriteFrame.idle) ;
        unitLabel.text = unit.InternalName;
    }
    public virtual void ForData(ProductionData data)
    {
        if (data == null)
        {
            Clear();
            return;
        }
        unitImage.enabled = true; 
        unitImage.sprite = data.icon;
        unitLabel.text = data.InternalName;
    }
    public  void ForData(ProductionTable data)
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
        unitImage.enabled = false;
        unitLabel.text = "";
    }
}
