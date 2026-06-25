using UnityEngine;
using UnityEngine.UI;

public class UnitContainer : MonoBehaviour
{
    public Image unitImage;
    public TMPro.TextMeshProUGUI unitLabel;

    public virtual void ForUnit(DataItemUnit unit) { }
}
