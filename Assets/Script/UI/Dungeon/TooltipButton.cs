using UnityEngine;
using UnityEngine.UI;

public class TooltipButton : MonoBehaviour
{
    public Transform hierarchyParent;
    #region Icon Component
    public Image BtnIcon;
    protected virtual void Awake()
    {
        if (BtnIcon == null)
            BtnIcon = transform.Find("Icon")?.GetComponent<Image>();
    }
    protected void ChangeIcon(Sprite IconSprite)
    {
        if (BtnIcon == null) return;
        BtnIcon.gameObject.SetActive(IconSprite != null);
        BtnIcon.sprite = IconSprite;
    }
    #endregion
}
