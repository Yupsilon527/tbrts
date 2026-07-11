using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController main;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        Clear();
    }

    public void Clear()
    {
        ClearTileColors();
        ClearTileHighlights();
        mouseOverTile = null;
    }

    #region Pathfinder
    public DataItemTile mouseOverTile;
    //Pathfinder.PathfinderPath pathToMouseTile;
    public void ChangeMouseTile(DataItemTile nTile)
    {
        if (mouseOverTile == nTile) return;
        if (mouseOverTile != null)
            mouseOverTile.display.Highlight(DisplayItemTile.tileState.clear);
        mouseOverTile = nTile;
        ClearTileColors();
        HandleAbilityInput();
    }
    void HandleAbilityInput()
    {
        if (mouseOverTile != null)
        {
            PropertySpell cast = GetCastData();
            if (cast != null)
            {
                ClearHighlightEntity();
                HighlightCastTiles(cast);
                ColorTargetTile(mouseOverTile, cast);
            }
            else
            {
                if (mouseOverTile.armyLayer != null)
                {
                    HighlightEntity(mouseOverTile.armyLayer);
                }
                else if (mouseOverTile.buildingLayer != null)
                {
                    HighlightCity(mouseOverTile.buildingLayer);
                }
                else
                {

                    /*float mRange = EntityPlayer.main.movement.Stamina.GetValue();

                    if (mRange > 0 && nTile != null)
                    {
                        pathToMouseTile = Pathfinder.Solve(RoomData.main, EntityPlayer.main, EntityPlayer.main.movement.gridPos, mouseOverTile.gridPos, 0);
                        pathToMouseTile.Cull(Mathf.FloorToInt(mRange));
                        HighlighPath(pathToMouseTile);
                    }*/
                    mouseOverTile.display.Highlight(DisplayItemTile.tileState.highlight);
                    ClearHighlightEntity();
                }
            }
        }
    }
    #endregion
    private void Update()
    {
        if (InterfaceManager.main.IsMouseOverUI())
            return;
        TrackMouseTile();
        if (Input.GetMouseButtonDown(0))
            HandleMainInput(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift), Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
        if (Input.GetMouseButtonDown(1))
            HandleSideInput();
    }
    void TrackMouseTile()
    {
        var mouseCoords = SidewaysMap.TranslateWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        mouseCoords.y = SidewaysMap.main.height - mouseCoords.y - 2;
        ChangeMouseTile(SidewaysMap.main.GetTile(mouseCoords));
    }

    public void CastSpellOnTile(DataItemTile tile)
    {
        if (GameManager.main.armyManager.mainSelectedArmy.abilities.CastAbilityOnTile(castData, tile))
        {
            ClearCastAbility();
        }
    }
    void HandleMainInput(bool queue, bool overlaymenu)
    {
        var selArmy = GameManager.main.armyManager.mainSelectedArmy;
        if (mouseOverTile != null)
        {
            if (overlaymenu)
            {
                InterfaceManager.main.commandMenu.OpenAtPosition(Input.mousePosition);
                InterfaceManager.main.commandMenu.OpenTileCommands(mouseOverTile);
            }
            else if (castData != null)
            {
                CastSpellOnTile(mouseOverTile);
            }
            else if (selArmy == null)
            {
               if (mouseOverTile.armyLayer != null)
                {
                    if (mouseOverTile.armyLayer.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.playerowned)
                        GameManager.main.armyManager.SelectArmy(mouseOverTile.armyLayer);
                }
                else if (mouseOverTile.buildingLayer != null)
                {
                    if (mouseOverTile.buildingLayer is DataItemCastle castle)
                        if (castle.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.playerowned)
                            InterfaceManager.main.OpenCastleWindow(castle, false);
                        else
                            InterfaceManager.main.infoWindow.ShowCastleInfo(castle);
                }
            }
            else
            {
                if (mouseOverTile.armyLayer != null)
                {
                    if (mouseOverTile.armyLayer.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.playerowned)
                        GameManager.main.armyManager.SelectArmy(mouseOverTile.armyLayer);
                    else if (mouseOverTile.armyLayer.IsVisibleToPlayer(GameManager.main.playerManager.GetActivePlayer()))
                        GiveOrder(selArmy,new FollowOrder(Order.ID.Follow, mouseOverTile.gridPos, mouseOverTile.armyLayer),  queue);
                }
                else if (mouseOverTile.buildingLayer != null)
                {
                    if (mouseOverTile.buildingLayer is DataItemCastle castle)
                    {
                        if (castle.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) != PlayerDefines.Alignment.enemy)
                            GiveOrder(selArmy, new Order(Order.ID.Move, mouseOverTile.gridPos), queue);
                       else if (castle.IsVisibleToPlayer(GameManager.main.playerManager.GetActivePlayer()))
                        {
                            GiveOrder(selArmy, new RazeOrder(Order.ID.Raze, mouseOverTile.gridPos), queue);
                        }

                    }
                }
                else
                {
                    GiveOrder(selArmy,new Order(Order.ID.Move, mouseOverTile.gridPos), queue);
                }
            }
        }
        else
        {
            GameManager.main.armyManager.ClearSelectedArmy();
            ClearCastAbility();
        }
    }
    void GiveOrder(DataItemBanner selArmy, Order order, bool queue)
    {

        if (queue)
            selArmy.orders.GiveOrder(order);
        else
        {
            foreach (var o in selArmy.orders.orders)
            {
                if (o.gridDest == mouseOverTile.gridPos)
                {
                    selArmy.movement.ResolveMovement();
                    return;
                }
            }
            selArmy.orders.ReplaceOrder(order);
    }
    }
    void HandleSideInput()
    {
        if (mouseOverTile != null)
        {
            InterfaceManager.main.commandMenu.OpenAtPosition(Input.mousePosition);
            InterfaceManager.main.commandMenu.OpenTileDetails(mouseOverTile);
        }
    }

    #region Highlight Entities
    DataItemBanner HighlightedEntity;
    void HighlightEntity(DataItemBanner ent)
    {
        if (ent != HighlightedEntity)
            ClearHighlightEntity();

        HighlightedEntity = ent;
        foreach (var tile in ent.GetOccupiedTiles())
        {
            tile.display.ChangeColor(ent.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.ally ? DisplayItemTile.tileState.select_ally : DisplayItemTile.tileState.select_enemy);
            colortiles.Add(tile);
        }
    }
    void ClearHighlightEntity()
    {
        HighlightedEntity = null;
    }
    #endregion
    #region Highlight Cities
    void HighlightCity(DataItemBuilding ent)
    {
        foreach (var tile in ent.GetOccupiedTiles())
        {
            tile.display.ChangeColor(ent.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.ally ? DisplayItemTile.tileState.mindread_ally : DisplayItemTile.tileState.mindread_danger);
            colortiles.Add(tile);
        }
    }
    #endregion
    #region Tile Highlights
    public List<DataItemTile> colortiles = new List<DataItemTile>();
    public List<DataItemTile> lighttiles = new List<DataItemTile>();
    public List<DataItemTile> outlinetiles = new List<DataItemTile>();
    /* void HighlighPath(Pathfinder.PathfinderPath hPath)
     {
         for (int I = 0; I < hPath.walkpath.Count; I++)
         {
             hPath.walkpath[I].ChangeColor(DataItemTile.tileState.walkpath);
             colortiles.Add(hPath.walkpath[I]);
         }
     }*/
    public void ClearTileColors()
    {
        foreach (DataItemTile tile in colortiles)
        { tile.display.ChangeColor(DisplayItemTile.tileState.clear); }
        colortiles.Clear();
    }
    public void HighlightCastTiles(PropertySpell Ability)
    {
        ClearTileHighlights();
        if (Ability != null)
        {
            foreach (DataItemTile hittile in Ability.GetValidCastTiles())
            {
                hittile.display.Highlight(DisplayItemTile.tileState.valid_tile);
                lighttiles.Add(hittile);
            }
        }
    }
    void ColorTargetTile(DataItemTile target, PropertySpell Ability)
    {
        if (Ability != null)
        {
            bool castable = Ability.CanCastOnTile(target);
            foreach (DataItemTile hittile in Ability.GetHitTiles(target))
            {
                hittile.display.ChangeColor(castable ? DisplayItemTile.tileState.select_unit : DisplayItemTile.tileState.select_enemy);
                colortiles.Add(hittile);
            }
        }
    }

    public void ClearTileHighlights()
    {
        foreach (DataItemTile tile in lighttiles)
        { tile.display.Highlight(DisplayItemTile.tileState.clear); }
        lighttiles.Clear();
    }

    #endregion
    #region AbilityCastData
    PropertySpell castData;
    public PropertySpell GetCastData()
    {
        return castData;
    }
    public void AssignCastAbility(PropertySpell ability)
    {
        castData = ability;
        HandleAbilityInput();
    }
    public void ClearCastAbility()
    {
        castData = null;
        HandleAbilityInput();
        ClearTileHighlights();
        ClearTileColors();
    }
    #endregion
}
