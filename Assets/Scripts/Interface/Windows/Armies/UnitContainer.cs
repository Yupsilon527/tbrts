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
        unitImage.sprite = data.icon;
        unitLabel.text = data.InternalName;
    }
    public virtual void ForData(ProductionTable data)
    {
    }
    public virtual void Clear()
    {

    }
}
