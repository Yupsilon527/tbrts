
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public abstract class InterfaceParent : MonoBehaviour
{
    public Hero mainHero;
    protected virtual void OnEnable()
    {
        DeselectHero(null);
    }
    public virtual void SelectHero(Mob player)
    {
        if (player is Hero hero)
        {
            mainHero = hero;
            gameObject.SetActive(player != null);
        }
        if (player != null)
        {
            OnSelectionChanged();
        }
    }
    public virtual void DeselectHero(Mob player)
    {
        SelectHero(null);
    }
    public virtual void ClearSelection()
    {
        SelectHero(null);
    }
    public virtual void OnSelectionChanged()
    {
        OnStuffChanged();
    }
    public virtual void OnStuffChanged()
    {

    }
    public static bool CheckMouseOverUI(Vector2 pointerPos)
    {
        PointerEventData m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = pointerPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData, results);
        return (results.Count > 0);
    }
}
