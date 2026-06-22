using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VikingParty;

public class InterfaceManager : WindowManager
{
    public static InterfaceManager main;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
    }
    public bool IsMouseOverUI()
    {
        GraphicRaycaster gr = transform.GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        return results.Count > 0;
    }
}
