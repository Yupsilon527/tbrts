using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager : WindowManager
{
    public static InterfaceManager main;

    public InfoWindow infoWindow;
    public CastleInfoWindow castleWindow;
    public ArmyInfoWindow armyWindow;

    public CommandMenu commandMenu;
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
