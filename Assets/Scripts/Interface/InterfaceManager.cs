using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager : WindowManager
{
    public static InterfaceManager main;

    public InfoWindow infoWindow;
    public ArmyInfoWindow armyWindow;
    public ArmyMergeWindow transferWindow;

    public CastleProductionWindow castleWindow;
    public CastleInfoWindow castleInfoWindow;
    public CastleRazeWindow castleRazeWindow;

    public CommandMenu commandMenu;
    public MovementIndicator moveIndicator;
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
    public void OpenCastleWindow(DataItemCastle castle, bool production)
    {
        castleWindow.Open();
        castleWindow.AssignPlayer(castle.GetPlayerOwner());
        castleWindow.SetCastle(castle);
        castleWindow.OpenTab(production ? 1 : 0);
    }
}
