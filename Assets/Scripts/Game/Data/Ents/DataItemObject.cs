using System.Collections.Generic;
using UnityEngine;

public class DataItemObject : DataItem
{
    public Vector2Int gridPos;
    public SidewaysTile tile;
    public DisplayItemObject<DataItemObject> display;

    private readonly HashSet<UnitGroup<DataItemObject>> _groups = new();
    public IReadOnlyCollection<UnitGroup<DataItemObject>> Groups => _groups;
    protected bool Selected = false;
    protected bool dead = false;
    protected DataItemPlayer currentOwner;

    public virtual void ChangeTile(Vector2Int t)
    {
        gridPos = t;
        tile = GameManager.main.map.GetTile(gridPos);
    }

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
    public void NotifyInterfaceChange()
    {
        //if (Selected)
          //  DungeonInterfaceController.main.unitInfo.Refresh();
    }
    public virtual void Select()
    {
        Selected = true;
        OnSelectStateChange(true);
        //    foreach (MobComponent cmp in GetComponents<MobComponent>())
        {
            //        cmp.OnRegisterNewOwner(GetPlayerOwner(), newOwner);
        }
        //  foreach (ISelectable sel in GetComponents<ISelectable>())
        {
            //     sel.OnSelected();
        }
        MessageGroups();
    }
    public void Deselect()
    {
        Selected = false;
        OnSelectStateChange(true);
    //    foreach (ISelectable sel in GetComponents<ISelectable>())
        {
     //       sel.OnDeselected();
        }
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
        return GetPlayerOwner().GetAlignment(other);
    }

    public virtual void SetPlayerOwner(DataItemPlayer player)
    {
         currentOwner =  player;
    }

    public void SetPlayerOwner(int player)
    {
        SetPlayerOwner(GameManager.main.playerManager.players[player]) ;
        display.OnPlayerOwnerChange();
    }

    public DataItemPlayer GetPlayerOwner()
    {
        return currentOwner;
    }
    #endregion

    public virtual void OnTurnEnd()
    {

    }
    #region Visible
    public  bool IsVisibleToAnother(DataItemObject other)
    {
        return true;
    }
    public  bool IsVisibleToPlayer(int playerID)
    {
        return true;
    }
    public virtual bool IsVisibleToPlayer(DataItemPlayer player)
    {
        return true;
    }
    public int GetSightRange()
    { return 0; }
    public int GetTrueRange()
    { return 0; }
    #endregion
    public virtual void Despawn()
    {

    }
}
