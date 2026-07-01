using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController main;
    float lastClick = 0;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        ClearTileColors();
        ClearTileHighlights();
    }

    #region Pathfinder
    public SidewaysTile mouseOverTile;
    //Pathfinder.PathfinderPath pathToMouseTile;
    public void ChangeMouseTile(SidewaysTile nTile)
    {
        if (mouseOverTile == nTile) return;
        if (mouseOverTile != null)
            mouseOverTile.display.Highlight(DisplayItemTile.tileState.clear);
        mouseOverTile = nTile;
        ClearTileColors();
        if (nTile != null)
        {
            /*AbilityData.CastData cast = GetCastData();
            if (cast != null)
            {
                HighlightCastTiles(cast.ability);
                cast.UpdatePointTarget(mouseOverTile);
                if (cast.CanCastOnTile(mouseOverTile))
                {
                    ColorTargetTile(mouseOverTile, cast);
                }
            }
            else*/
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
        if (InterfaceManager.main.IsMouseOverUI() )
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
                if (mouseOverTile.armyLayer != null )
                {
                    if (mouseOverTile.armyLayer.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.playerowned)
                        GameManager.main.armyManager.SelectArmy(mouseOverTile.armyLayer);
                    else if (mouseOverTile.armyLayer.IsVisibleToPlayer(GameManager.main.playerManager.GetActivePlayer()))
                        if (queue)
                            selArmy.orders.GiveOrder(new AttackOrder(Order.ID.Follow, mouseOverTile.gridPos, mouseOverTile.armyLayer));
                        else
                            selArmy.orders.ReplaceOrder(new AttackOrder(Order.ID.Follow, mouseOverTile.gridPos, mouseOverTile.armyLayer));
                }
                else if (mouseOverTile.buildingLayer != null)
                {
                    if (mouseOverTile.buildingLayer is DataItemCastle castle)
                    {
                        if (castle.IsVisibleToPlayer(GameManager.main.playerManager.GetActivePlayer()))
                        {
                            //   order raze
                        }

                    }
                }
                else
                {
                    if (queue)
                        selArmy.orders.GiveOrder(new Order(Order.ID.Move, mouseOverTile.gridPos));
                    else
                    {
                        selArmy.orders.ReplaceOrder(new Order(Order.ID.Move, mouseOverTile.gridPos));
                    }
                }
            }
        }
        else
        {
            GameManager.main.armyManager.ClearSelectedArmy();
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
    DataItemArmy HighlightedEntity;
    void HighlightEntity(DataItemArmy ent)
    {
        if (ent != HighlightedEntity)
            ClearHighlightEntity();

        HighlightedEntity = ent;
        ent.tile.display.Highlight(ent.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.ally ? DisplayItemTile.tileState.select_ally : DisplayItemTile.tileState.select_enemy);
    }
    void ClearHighlightEntity()
    {
        HighlightedEntity = null;
    }
    #endregion
    #region Highlight Cities
    void HighlightCity(DataItemBuilding ent)
    {
        ent.tile.display.Highlight(ent.GetAlignment(GameManager.main.playerManager.GetActivePlayer()) == PlayerDefines.Alignment.ally ? DisplayItemTile.tileState.mindread_ally : DisplayItemTile.tileState.mindread_danger);
    }
    #endregion
    #region Tile Highlights
    public enum HighlightState
    {
        clear = 0,
        setup,
        actor,
        ability
    }
    public List<SidewaysTile> colortiles = new List<SidewaysTile>();
    public List<SidewaysTile> lighttiles = new List<SidewaysTile>();
    public List<SidewaysTile> outlinetiles = new List<SidewaysTile>();
    public void ChangeHighlightState(HighlightState state)
    {
        Debug.Log("[stateInGame] Highlight Tiles In State " + state);
        /*ClearTileColors();
        switch (state)
        {
            case HighlightState.setup:
                foreach (WorldTile t in GetPlayerSide().GetStartingTiles())
                {
                    t.ChangeColor(WorldTile.tileState.setup);
                    colortiles.Add(t);
                }
                return;
            case HighlightState.actor:
                EntityBase selection = GetPlayerSide().getSelectedEntity();
                if (selection == null)
                {
                    ChangeHighlightState(HighlightState.clear);
                }
                if (CurrentPhase == Phase.Setup)
                {
                    foreach (WorldTile walkpath in GetPlayerSide().GetStartingTiles())
                    {
                        if (Pathfinder.CanIWalkOver(selection.GetMovementType(), walkpath))
                        {
                            walkpath.ChangeColor(WorldTile.tileState.setup);
                            colortiles.Add(walkpath);
                        }

                    }
                }
                ColorTilePredictions();
                return;
            case HighlightState.ability:
                if (castAbility.ability == null)
                {
                    ChangeHighlightState(HighlightState.clear);
                }
                foreach (WorldTile walkpath in castAbility.ability.GetValidCastTiles())
                {
                    walkpath.ChangeColor(WorldTile.tileState.highlight_ability);
                    colortiles.Add(walkpath);

                }
                return;
            case HighlightState.clear:
                ColorTilePredictions();
                return;
        }*/
    }
    /* void HighlighPath(Pathfinder.PathfinderPath hPath)
     {
         for (int I = 0; I < hPath.walkpath.Count; I++)
         {
             hPath.walkpath[I].ChangeColor(SidewaysTile.tileState.walkpath);
             colortiles.Add(hPath.walkpath[I]);
         }
     }*/
    public void ClearTileColors()
    {
        foreach (SidewaysTile tile in colortiles)
        { tile.display.ChangeColor(DisplayItemTile.tileState.clear); }
        colortiles.Clear();
    }
    /*public void HighlightCastTiles(propertyAbility Ability)
    {
        ClearTileHighlights();
        if (Ability != null)
        {
            foreach (SidewaysTile hittile in Ability.GetValidCastTiles())
            {
                hittile.Highlight(tileState.abilitycastable);
                lighttiles.Add(hittile);
            }
        }
    }
    void ColorTargetTile(SidewaysTile target, AbilityData.CastData Ability)
    {

        if (Ability != null)
        {
            foreach (SidewaysTile hittile in Ability.GetHitTiles(true))
            {
                hittile.Highlight(tileState.abilitytarget);
                lighttiles.Add(hittile);
            }
        }
    }*/

    public void ClearTileHighlights()
    {
        foreach (SidewaysTile tile in lighttiles)
        { tile.display.Highlight(DisplayItemTile.tileState.clear); }
        lighttiles.Clear();
    }

    #endregion
    #region AbilityCastData
    /* AbilityData.CastData castData;
     public AbilityData.CastData GetCastData()
     {
         return castData;
     }
     public void CastAbilitySelf(propertyAbility ability, bool forceNew)
     {
         CastAbilityPoint(ability, EntityPlayer.main.movement.GetMyTile(), forceNew);
     }
     public void CastAbilityPoint(propertyAbility ability, SidewaysTile point, bool forceNew)
     {
         CastAbilityPoint(ability, point.gridPos, forceNew);
     }
     public void CastAbilityPoint(propertyAbility ability, Vector2Int point, bool forceNew)
     {
         if (forceNew || (castData == null || castData.ability != ability))
         {
             castData = EntityPlayer.main.abilities.CastAbility(ability);
         }
         castData.UpdatePointTarget(point);
     }
     void ClearCastAbility()
     {
         castData = null;
     }*/
    #endregion
}
