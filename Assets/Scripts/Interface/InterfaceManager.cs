using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InterfaceManager : WindowManager
{
    public static InterfaceManager main;

    public PlayerWidget playerWidget;

    public InfoWindow infoWindow;
    public ArmyMergeWindow transferWindow;
    public CombatWindow combatWindow;

    public ArmyInfoWindow armyWindow;
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
    public void AssignPlayer(DataItemPlayer player)
    {
        OpenWindow(infoWindow);
        infoWindow.ShowPlayerTurn(player);
        playerWidget?.AssignPlayer(player);

        PlayerInputController.main.Clear();
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
    public void OpenCastleRazeWindow(DataItemCastle castle, DataItemBanner attacker)
    {
        castleRazeWindow.assignedCastle = castle;
        castleRazeWindow.attacker = attacker;
        if (castleRazeWindow.ValidRaze()) {
            castleRazeWindow.Open();
        }
    }
    public void PreviewCombat(DataItemBanner attacker, DataItemBanner defender)
    {

    }
    public void OpenPrepareCombatWindow(DataItemBanner attacker, DataItemBanner defender)
    {
        combatWindow.Open();
        combatWindow.PresentSides(attacker, defender);
    }
}
