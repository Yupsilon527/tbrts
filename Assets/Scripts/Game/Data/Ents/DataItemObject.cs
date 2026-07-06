using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public abstract class DataItemObject : DataItem
{
    public IDisplayItemObject<DataItemObject> display;

    private readonly HashSet<UnitGroup<DataItemObject>> _groups = new();
    public IReadOnlyCollection<UnitGroup<DataItemObject>> Groups => _groups;
    protected bool Selected = false;
    public bool dead = false;
    protected DataItemPlayer currentOwner;

    public abstract Vector2Int GetCoords();
    public abstract DataItemTile[] GetOccupiedTiles();
    public virtual void ChangeTile(Vector2Int t, DisplayPositionChange position)
    {
        OnPositionChange(t, t);
        display?.OnPositionChange(t, position);
    }
    public int GetDistanceFromTile(DataItemTile tile) {
        return GetDistanceFromTile(tile.gridPos);
    }
    public bool IsAdjecent(DataItemTile tile)
    {
        return IsAdjecent(tile.gridPos);
    }
    public bool IsAdjecent(Vector2Int tile) {
        return GetOccupiedTiles().Any(t => t.IsAdjecent(tile));
    }
    public int GetDistanceFromTile(Vector2Int tile) {
        float dist = int.MaxValue;
        foreach (var t in GetOccupiedTiles())
        {
            dist= Mathf.Min((tile - t.gridPos).magnitude,dist);
        }
        return (int)dist;
    }
    public int GetDistanceFromObject(DataItemObject other)
    {
        float dist = int.MaxValue;
        foreach (var t in other.GetOccupiedTiles())
        {
            dist = Mathf.Min(GetDistanceFromTile(t.gridPos), dist);
        }
        return (int)dist;
    }
    public virtual int GetAuraRange() { return 3; }

    #region Unit Groups
    public bool IsInGroup(UnitGroup<DataItemObject> group) => _groups.Contains(group);
    public bool IsInAnyGroup => _groups.Count > 0;

    internal void RegisterGroup(UnitGroup<DataItemObject> group) { if (group != null) _groups.Add(group); }
    internal void UnregisterGroup(UnitGroup<DataItemObject> group) => _groups.Remove(group);
    internal void MessageGroups() { foreach (var group in _groups) { group?.OnContentsUpdate(); } }
   
    #endregion
    #region Selection
    public bool IsSelected()
    {
        return Selected;
    }
    public void Select()
    {
        SetSelected(true);
    }
    public void NotifyInterfaceChange()
    {
        //if (Selected)
          //  DungeonInterfaceController.main.unitInfo.Refresh();
    }
    public virtual void SetSelected( bool value)
    {
        Selected = value;
        OnSelectStateChange(value);
        //    foreach (MobComponent cmp in GetComponents<MobComponent>())
        {
            //        cmp.OnRegisterNewOwner(GetPlayerOwner(), newOwner);
        }
        //  foreach (ISelectable sel in GetComponents<ISelectable>())
        {
            //     sel.OnSelected();
        }
        display?.OnSelectionChange();
        MessageGroups();
    }
    public virtual void OnSelectStateChange(bool hard)
    {

      //  if (hard && Selected)
      //      DungeonInterfaceController.main.unitInfo.ChangeSelection(this);
     //   else
      //      DungeonInterfaceController.main.unitInfo.Refresh();
    }
    #endregion
    #region Alignment
    public PlayerDefines.Alignment GetAlignment(DataItemObject other)
    {
        return GetAlignment(other.currentOwner);
    }
    
    public PlayerDefines.Alignment GetAlignment(DataItemPlayer other)
    {
        if (GetPlayerOwner() == null || other == null) return PlayerDefines.Alignment.enemy;
        return GetPlayerOwner().GetAlignment(other);
    }

    public virtual void SetPlayerOwner(DataItemPlayer player)
    {
         currentOwner =  player;
        display?.OnPlayerOwnerChange();
    }

    public void SetPlayerOwner(int player)
    {
        SetPlayerOwner(GameManager.main.playerManager.players[player]) ;
    }

    public DataItemPlayer GetPlayerOwner()
    {
        return currentOwner;
    }
    #endregion

    public virtual void OnTurnBegin()
    {

    }
    #region Visible
    public  bool IsVisibleToAnother(DataItemObject other)
    {
        return true;
    }
    public virtual bool IsVisibleToPlayer(int playerID)
    {
        return GetOccupiedTiles().Any(t => t.IsRevealedByPlayer(playerID, UnitDefines.TileVisibility.visible) );
    }
    public virtual bool IsVisibleToPlayer(DataItemPlayer player)
    {
        return IsVisibleToPlayer(player.ID);
    }
    public int GetSightRange()
    { return 3; }
    public int GetTrueRange()
    { return 0; }
    public void OnPositionChange(Vector2Int newPos, Vector2Int oldPos) { }
    public void RevealPosition()
    {
        foreach (var tile in GetOccupiedTiles())
        {
            if (GetSightRange() <= 0) return;
                GameManager.main.los.RevealCircle(GetPlayerOwner().ID, tile.gridPos, GetSightRange(), UnitDefines.TileVisibility.revealed_visible);
            if (GetTrueRange()>0)
            GameManager.main.los.RevealCircle(GetPlayerOwner().ID, tile.gridPos, GetTrueRange(), UnitDefines.TileVisibility.revealed_truesight);
        }
    }
    #endregion
    public virtual void Despawn()
    {

    }
}
