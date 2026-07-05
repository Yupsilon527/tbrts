using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandMenu : PlayerWindow
{
    public Transform parent;
   public RectTransform rectTransform;

    public string[] Names;
    public PlayerMenuAction[] Actions;

    public GameObject playerButtonPrefab;
    public GameObject playerDividerPrefab;
    List<GameObject> entries = new();
    List<GameObject> dividers = new();
    public delegate void PlayerMenuAction();

    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref rectTransform);
    }
    public void OpenAtPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
        Open();
    }
    public void OpenTileCommands(DataItemTile tile)
    {
        var player = GameManager.main.playerManager.GetActivePlayer();
        List<PropertySpell> s = new();
        List<string> e = new();
        List<PlayerMenuAction> a = new();
        var selArmy = GameManager.main.armyManager.mainSelectedArmy;
        if (selArmy != null)
        {
            if (selArmy.movement.CanWalkOnTile(tile))
            {
                e.Add("Move here");
                a.Add(() => {
                    selArmy.orders.ReplaceOrder(new Order(Order.ID.Move, tile.gridPos));
                    selArmy.movement.ResolveMovement();
                });
            }
            if (tile.armyLayer != null)
            {
                if (tile.armyLayer == selArmy)
                {
                    e.Add("Defend");
                    a.Add(() => {
                        selArmy.orders.ReplaceOrder(new Order(Order.ID.Rest, tile.gridPos));
                    });
                }
                else if (tile.armyLayer.GetAlignment(selArmy) == PlayerDefines.Alignment.playerowned)
                {
                    if (selArmy.CanWeMerge(tile.armyLayer))
                    {
                        e.Add("Merge");
                        a.Add(() => {
                            selArmy.Transfer(tile.armyLayer, false);
                        });
                    }
                    else
                    {
                        e.Add("Transfer");
                        a.Add(() => {
                            InterfaceManager.main.OpenWindow(InterfaceManager.main.transferWindow);
                            InterfaceManager.main.transferWindow.AssignPlayer(player);
                            InterfaceManager.main.transferWindow.MergeUnits(selArmy, tile.armyLayer);
                        });
                    }
                }
            }
            else if (selArmy.IsAdjecent(tile))
            {
                e.Add("Split");
                a.Add(() => {
                    var tempArmy = new DataItemArmy(tile.gridPos, player.ID);
                    InterfaceManager.main.OpenWindow(InterfaceManager.main.transferWindow);
                    InterfaceManager.main.transferWindow.AssignPlayer(player);
                    InterfaceManager.main.transferWindow.MergeUnits(selArmy, tile.armyLayer);
                });
            }
            if (tile.buildingLayer != null)
            {
                if (tile.buildingLayer is DataItemCastle castle && selArmy.CanInvadeCastle(castle))
                {
                    e.Add("Raze");
                    a.Add(() => {
                        InterfaceManager.main.OpenCastleRazeWindow(castle,selArmy);
                    });
                }
                //Explore ruin
            }
            if (tile.gridPos == selArmy.GetCoords())
            {
                s.AddRange(selArmy.abilities.GetAllAvaiableSpells());
            }
            else
            {
                s.AddRange(selArmy.abilities.GetAbilitiesCastable(tile));
            }
        }
        if (tile.armyLayer != null && tile.armyLayer.GetAlignment(player) == PlayerDefines.Alignment.playerowned)
        {
                e.Add("Select");
                a.Add(() => {
                        GameManager.main.armyManager.SelectArmy(tile.armyLayer);
                });
        }
        if (s.Count > 0)
        {
            e.Add("-");
            a.Add(null);

            foreach (var spell in s)
            {
                e.Add("Cast " + spell.InternalName);
                a.Add(()=>
                {
                    if (spell.HasResourcesToCast()) { 
                    if (spell.InstantCast())
                    {
                        selArmy.abilities.CastAbilityOnTile(spell, tile);
                    }
                    else
                    {
                        PlayerInputController.main.AssignCastAbility(spell);
                    }
                    }
                });
            }
        }

        LoadEntries(e.ToArray(), a.ToArray());
    }

    public void OpenTileDetails(DataItemTile tile)
    {
        var player = GameManager.main.playerManager.GetActivePlayer();
        List<string> e = new();
        List<PlayerMenuAction> a = new();

        if (tile.buildingLayer is DataItemCastle castle)
        {
            if (castle.GetAlignment(player) == PlayerDefines.Alignment.playerowned)
            {
                e.Add("Castle Production");
                a.Add(() => { InterfaceManager.main.OpenCastleWindow(castle,true); });

            }
            else
            {
                e.Add("Castle Info");
                a.Add(() => { InterfaceManager.main.infoWindow.ShowCastleInfo(castle); });
            }
        }
        if (tile.IsRevealedByPlayer(player, UnitDefines.TileVisibility.visible))
        {
            e.Add("Tile Info");
            a.Add(() => { InterfaceManager.main.infoWindow.ShowTileInfo(tile); });

            if (tile.armyLayer is DataItemArmy army && army.IsVisibleToPlayer(player))
            {
                e.Add("Army Info");
                a.Add(() => { InterfaceManager.main.OpenWindow(InterfaceManager.main.armyWindow); });
            }
        }

        e.Add("<div>");
        a.Add(null);

        e.Add("Next Idle Army");
        a.Add(() => { GameManager.main.armyManager.SelectNextIdleArmy(); });
        e.Add("Next Idle City");
        a.Add(() => { GameManager.main.castleManager.SelectNextIdleCity(); });

        e.Add("<div>");
        a.Add(null);

        e.Add("Zoom On Tile");
        a.Add(() => { CameraController.main.CenterOnTile(tile.gridPos); CameraController.main.Zoom(0, true); });
        if (GameManager.main.armyManager.mainSelectedArmy is DataItemArmy selArmy)
        {
            e.Add("Center on selected army");
            a.Add(() => { CameraController.main.CenterOnTile(selArmy.GetCoords()); });
            e.Add("Move here");
            a.Add(() => {
                selArmy.orders.ReplaceOrder(new Order(Order.ID.Move, tile.gridPos));
            }); 
            e.Add("Reorganize");
            a.Add(() => {
                InterfaceManager.main.OpenWindow(InterfaceManager.main.transferWindow);
                InterfaceManager.main.transferWindow.AssignPlayer(player);
                InterfaceManager.main.transferWindow.MergeUnits(selArmy, null);
            });
            e.Add("Deselect");
            a.Add(() => { GameManager.main.armyManager.ClearSelectedArmy(); });
        }
        e.Add("<div>");
        a.Add(null);
        e.Add("Closest Army");
        a.Add(() => { GameManager.main.armyManager.SelectClosestArmy(tile.gridPos); });
        e.Add("Move All Armies");
        a.Add(() => { GameManager.main.armyManager.MoveAllArmies(); });
        e.Add("End Turn");
        a.Add(() => { GameManager.main.EndTurn(); });

        LoadEntries(e.ToArray(), a.ToArray());
    }
    public void OpenAtPosition(string[] Names, PlayerMenuAction[] ButtonAction, Vector3 Position)
    {
        Open();
        LoadEntries(Names, ButtonAction);
        //MoveToPosition(((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f));
        MoveToPosition(Position);
    }
    public void OpenAtTarget(string[] Names, PlayerMenuAction[] ButtonAction, Transform target)
    {
        Open();
        LoadEntries(Names, ButtonAction);
        //MoveToPosition(((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f));
        MoveToTarget(target);
    }
    public void MoveToTarget(Transform target)
    {
        MoveToPosition(target.position);
        RotateToPosition(target.rotation);
    }

    public void LoadEntries(string[] names, PlayerMenuAction[] buttonAction)
    {
        ClearList();
        Names = names;
        int nEntries = Names.Length;
        if (buttonAction != null)
        {
            Actions = buttonAction;
            nEntries = Mathf.Min(Names.Length, Actions.Length);

        }

        if (nEntries > 0)
        {
            for (int i = 0; i < nEntries; i++)
            {
                if (Names[i] == "<div>" || Actions[i] == null)
                    PoolDivider(i);
                else
                    PoolButton(i);
            }

        }
        else { Close(); }
    }
    void PoolDivider(int pos)
    {
        foreach (var div in dividers)
        {
            if (div != null && !div.activeSelf)
            {
                div.SetActive(true);
                div.transform.SetAsLastSibling();
                return;
            }
        }
        GameObject d = Instantiate(playerDividerPrefab, transform);
        dividers.Add(d);
        d.SetActive(true);
        d.transform.SetAsLastSibling();
    }
    void PoolButton(int pos)
    {
        foreach (var div in entries)
        {
            if (div != null && !div.activeSelf)
            {
                div.SetActive(true);
                div.transform.SetAsLastSibling();
                AssignButton(div, pos);
                return;
            }
        }

        GameObject d = Instantiate(playerButtonPrefab, transform);
        dividers.Add(d);
        d.SetActive(true);
        d.transform.SetAsLastSibling();
        AssignButton(d, pos);
    }
    void AssignButton(GameObject listle, int i)
    {
        listle.name = "Entry " + i;
        Button lBtn = listle.GetComponent<Button>();
        listle.GetComponentInChildren<TextMeshProUGUI>().text = Names[i];
        if (Actions != null)
        {
            lBtn.enabled = true;
            lBtn.onClick.RemoveAllListeners();
          //  lBtn.onClick.AddListener(() => { if (Actions[i]()) { Close(); } });
            lBtn.onClick.AddListener(() => { Actions[i](); { Close(); } });
            if (i == 0)
                lBtn.Select();
        }
        else
        {
            lBtn.enabled = false;
        }
    }
    public void MoveToPosition(Vector2 position)
    {
        parent.transform.position = position;

    }
    public void RotateToPosition(Quaternion rotation)
    {
        parent.transform.rotation = rotation;
    }
    void ClearList()
    {
        foreach (GameObject div in dividers)
        { div.SetActive(false); }
        foreach (GameObject listle in entries)
        { listle.SetActive(false); }
    }
}