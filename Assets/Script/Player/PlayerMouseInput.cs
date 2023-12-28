using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMouseInput : MonoBehaviour
{
    public static PlayerMouseInput main;
    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        ClearTileColors();
        //ClearTileHighlights();
    }

    #region Pathfinder
    public DisplayItemTile mouseOverTile;
    Pathfinder.PathfinderPath pathToMouseTile;
    public void ChangeMouseTile(DisplayItemTile nTile)
    {
        if (mouseOverTile != nTile)
        {
            if (mouseOverTile != null)
                mouseOverTile.Highlight(DisplayItemTile.tileState.clear);
            mouseOverTile = nTile;
            ClearTileColors();
            if (nTile != null)
            {
                /*CastTable cast = GetCastData();
                if (cast != null)
                {
                    HighlightCastTiles(cast.ability);
                    cast.UpdatePointTarget(mouseOverTile.data);
                    if (cast.CanCastOnTile(mouseOverTile.data))
                    {
                        ColorTargetTile( cast);
                    }
                }
                else*/
                {
                    if (mouseOverTile.data.LocatedEntity != null)
                    {
                        HighlightEntity(mouseOverTile.data.LocatedEntity);
                    }
                    else
                    {

                        /*float mRange = EntityPlayer.main.movement.Stamina.GetValue();

                        if (mRange > 0 && nTile != null)
                        {
                            pathToMouseTile = Pathfinder.Solve(DataItemWorld.main, EntityPlayer.main, EntityPlayer.main.movement.coords, mouseOverTile.data.coords, 0);
                            pathToMouseTile.Cull(Mathf.FloorToInt(mRange));
                            HighlighPath(pathToMouseTile);
                        }*/
                        mouseOverTile.Highlight(DisplayItemTile.tileState.highlight);
                        ClearHighlightEntity();
                    }
                }
            }
        }
    }
    #endregion
    private void Update()
    {
        if (InterfaceManager.main != null && InterfaceManager.main.IsMouseOverUI())
            return;
        HandlePlayerOrders();
    }
    void HandlePlayerOrders()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*if (castData != null)
            {
               if ( CastAbility())
                {
                    ClearCastAbility();
                }
            }
            else if (highlightedEntity != null)
            {
                CastAbilityPoint(EntityPlayer.main.abilities.GetAbilitySlotIndex(0).ability, highlightedEntity.movement.coords, true);
                CastAbility();
                ClearCastAbility();
            }
            else*/
            if (mouseOverTile != null)
            {
                if (mouseOverTile.data.LocatedEntity != null)
                {
                    PlayerController.main.party.SelectCharacter(mouseOverTile.data.LocatedEntity);
                }
                /*if (Input.GetKey( KeyCode.LeftShift))
                {
                    CastAbilityPoint(EntityPlayer.main.abilities.GetAbilitySlotIndex(0).ability, mouseOverTile.data.coords, true);
                    CastAbility();
                    ClearCastAbility();
                }
                else if (pathToMouseTile != null)
                {
                    OnAction();
                    EntityPlayer.main.movement.MoveDownPath(pathToMouseTile, 1);
                    UpdateWalkPath(mouseOverTile);
                }*/
            }
        }
        /*if (Input.GetMouseButtonDown(1) && castData != null)
        {
            ClearCastAbility();
            ClearTileHighlights();
        }*/
    }
    /*bool CastAbility()
    {
        if (EntityPlayer.main.abilities.ResolveCastData(castData))
        {
            InGameInterface.main.abilities.Revise();
            OnAction();
            return true;
        }
        return false;
    }*/
    #region Highlight Entities
    Mob HighlightedEntity;
    void HighlightEntity(Mob ent)
    {
        if (ent != HighlightedEntity)
            ClearHighlightEntity();

        HighlightedEntity = ent;
        //HighlightedEntity.EnableDisableOverlay(true);

        /*foreach (DataItemTile tile in ent.movement.GetOccupiedTiles())
        {
            if (tile != null && tile.display != null)
                tile.display.Highlight( DisplayItemTile.tileState.enemy);
        }*/
    }
    void ClearHighlightEntity()
    {
        /*if (HighlightedEntity!=null)
        {
            HighlightedEntity.EnableDisableOverlay(false);
        }*/
        HighlightedEntity = null;
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
    public List<DisplayItemTile> colortiles = new List<DisplayItemTile>();
    public List<DisplayItemTile> lighttiles = new List<DisplayItemTile>();
    public List<DisplayItemTile> outlinetiles = new List<DisplayItemTile>();
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
    public void ColorTilePredictions()
    {
        /* foreach (EntityBase actor in EntityManager.main.GetAllVisibleActors())
         {
             propertyOrder currentOrder = actor.GetCurrentOrder();
             if (currentOrder != null)
             {
                 if (actor.getAlignment(GetPlayerSide()) != Defines.AlignmentType.enemy)
                 {
                     WorldTile.tileState newPhase = WorldTile.tileState.mindread_walkally;
                     if (currentOrder.Name == OrderDefines.Type.cast)
                     {
                         newPhase = WorldTile.tileState.mindread_ally;
                     }
                     foreach (WorldTile tile in currentOrder.highlight_tiles)
                     {
                         tile.ChangeColor(newPhase);
                         colortiles.Add(tile);
                     }

                 }
                 else
                 {
                     if (currentOrder.Name == OrderDefines.Type.cast && actor.Modifiers.GetState(ModifierDefines.modStates.mind_read))
                     {
                         foreach (WorldTile tile in currentOrder.highlight_tiles)
                         {
                             tile.ChangeColor(WorldTile.tileState.mindread_danger);
                             colortiles.Add(tile);
                         }
                     }
                 }
             }
         }*/
    }
    void HighlighPath(Pathfinder.PathfinderPath hPath)
    {
        for (int I = 0; I < hPath.walkpath.Count; I++)
        {
            DataItemTile tile = DataItemWorld.main.GetTile(hPath.walkpath[I]);
            if (tile != null && tile.display != null)
            {
                tile.display.ChangeColor(DisplayItemTile.tileState.walkpath);
                colortiles.Add(tile.display);
            }
        }
    }
    public void ClearTileColors()
    {
        foreach (DisplayItemTile tile in colortiles)
        { tile.ChangeColor(DisplayItemTile.tileState.clear); }
        colortiles.Clear();
    }
    #endregion
    #region Ability
    /*
    public void HighlightCastTiles(PropertyAbility Ability)
    {
            ClearTileHighlights();
        if (Ability != null)
        {
            foreach (DataItemTile hittile in Ability.GetValidCastTiles())
            {
                if (hittile != null && hittile.display != null)
                {
                    hittile.display.Highlight(DisplayItemTile.tileState.abilitycastable);
                    lighttiles.Add(hittile.display);
                }
            }
        }
    }
    void ColorTargetTile(CastTable castData)
    {

        if (castData != null)
        {
            foreach (DataItemTile hittile in castData.GetHitTiles(false))
            {
                if (hittile != null && hittile.display != null)
                {
                    hittile.display.Highlight(DisplayItemTile.tileState.abilitytarget);
                    lighttiles.Add(hittile.display);
                }
            }
            if (castData.rayresult != null)
            {
                foreach (DataItemTile hittile in castData.rayresult.trajectory)
                {
                    if (hittile != null && hittile.display != null)
                    {
                        hittile.display.Highlight(DisplayItemTile.tileState.abilitytrajectory);
                        lighttiles.Add(hittile.display);
                    }                    
                }
            }
            else
            {
                castData.targetTile.display.Highlight(DisplayItemTile.tileState.abilitytrajectory);
            }
        }
    }

    public void ClearTileHighlights()
    {
        foreach (DisplayItemTile tile in lighttiles)
        { tile.Highlight(DisplayItemTile.tileState.clear); }
        lighttiles.Clear();
    }*/
    #endregion
    #region AbilityCastData
    /*CastTable castData;
    public CastTable GetCastData()
    {
        return castData;
    }
    public void CastAbilitySelf(PropertyAbility ability, bool forceNew)
    {
        CastAbilityPoint(ability, EntityPlayer.main.movement.GetMyTile(), forceNew);
    }
    public void CastAbilityPoint(PropertyAbility ability, DataItemTile point, bool forceNew)
    {
        CastAbilityPoint(ability, point.coords, forceNew);
    }
    public void CastAbilityPoint(PropertyAbility ability, HexCoords point, bool forceNew)
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
