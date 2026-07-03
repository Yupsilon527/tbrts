using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyManager : EntityManager
{
    int idleArmyIndex = 0;
    public DataItemArmy mainSelectedArmy = null;
    public List<DataItemArmy> armies = new();
    public HashSet<DataItemArmy> movingArmies = new();


    /*public static DataItemUnit SpawnUnit(GameHubWorld game, DataItemArmy UnitName, entityPlayer Player, entityTile tTile, int iDur)
    {
        DataItemUnit Panty = SpawnUnit(game, UnitName, Player, tTile);
        if (iDur >= 0)
        {
            Panty.AddModifier(new entityModifier(entityModifier.Names.timedlife, 0, iDur, entityModifier.Behavior.world_modifier, entityModifier.Alignment.neutral, false), 0);
        }
        return Panty;
    }*/

    public DataItemArmy FindArmyByID(int ID)
    {
        return armies.FirstOrDefault(a => a.eID == ID) ;
    }
    public static DataItemUnit SpawnUnit( string UnitName, DataItemPlayer Player, DataItemTile tTile)
    {
        return SpawnUnit(WorldManager.main.LoadUnit(UnitName), Player, tTile);
    }
    public static DataItemUnit SpawnUnit(UnitData uData, DataItemPlayer Player, DataItemTile tTile)
    {
        return SpawnUnitInCastle( uData, Player, tTile, null);
    }
    public static DataItemUnit SpawnUnitInCastle(UnitData uData, DataItemPlayer Player, DataItemTile tTile, DataItemCastle myCastle)
    {
        if (tTile == null)
        {
            return null;
        }
        DataItemArmy newArmy = tTile.armyLayer;

        if (newArmy == null || newArmy.formation.CanIAccept(uData))
        {
            var newTile = GameManager.main.map.GetClosestToPoint(tTile.gridPos, uData.GetMovetype(), empty: true);
            if (newTile != null)
                newArmy = new DataItemArmy(newTile.gridPos, myCastle.GetPlayerOwner().ID);
            else 
                return null;

        }

        DataItemUnit Zim = new DataItemUnit(  uData, newArmy);

        if (Zim == null)
        {
            return null;
        }

        Player.upgrades.ApplyResearchedUpgradeToNewlySpawnedUnit(Zim);
        myCastle.bonuses.ApplyResearchedUpgradeToNewlySpawnedUnit(Zim);
        Zim.FireEventOnSelf(AbilityDefines.Event.OnSpawn);

        return Zim;
    }
    public static DataItemUnit SpawnUnit(UnitData uData, DataItemArmy army, DataItemCastle myCastle)
    {



        DataItemUnit Zim = new DataItemUnit(uData, army);
        army.GetPlayerOwner().upgrades.ApplyResearchedUpgradeToNewlySpawnedUnit(Zim);
        myCastle.bonuses.ApplyResearchedUpgradeToNewlySpawnedUnit(Zim);
        Zim.FireEventOnSelf(AbilityDefines.Event.OnSpawn);
        return Zim;
    }

    public void GenerateTheArmiesFromEditorData(CustomArmy[] armies)
    {
        foreach (var Zim in armies)
        {
            new DataItemArmy(Zim);
        }
    }
    public void RegisterArmy(DataItemArmy army)
    {
        if (!armies.Contains(army))
        armies.Add(army);
        if (army.display == null)
        {
            var armyPrefab = GameManager.main.displayPool.PoolItem(GameManager.main.displayPool.armyPrefab);
            if (armyPrefab.TryGetComponent(out DisplayItemArmy dia))
            {
                dia.AssignObject(army);
            }
        }
    }
    public void ForgetArmy(DataItemArmy army)
    {
        if (!army.dead)
        {
            army.Despawn();
        }
        armies.Remove(army);
        if (army.display != null)
        {
            GameManager.main.displayPool.DeactivateObject(army.display.GetParentObject());
        }
    }

    public void SelectArmy(DataItemArmy army)
    {
        if (army != null && mainSelectedArmy != army)
        {
            ClearSelectedArmy();
            mainSelectedArmy = army;
            if (!army.IsSelected())
            army.SetSelected(true);
        }
    }
    public void ClearSelectedArmy()
    {
        if (mainSelectedArmy != null)
        {
            mainSelectedArmy.SetSelected(false);
            mainSelectedArmy = null;
        }
    }
    public void SelectNextIdleArmy()
    {
        var playerArmies = GameManager.main.playerManager.currentPlayer.units;
        if (playerArmies.Count == 0) return;
        if (idleArmyIndex > playerArmies.Count)
        {
            idleArmyIndex = 0;
        }
      for (int i = 0; i< playerArmies.Count; i++)
        {
            int index = (i + idleArmyIndex) % playerArmies.Count;
            if (playerArmies[index] is DataItemArmy army && army.orders.IsIdle())
            {
                playerArmies[index].Select();
                return;
            }
        }
    }
    public void SelectClosestArmy(Vector2Int gridpos)
    {
        var playerArmies = GameManager.main.playerManager.currentPlayer.units;
        if (playerArmies.Count == 0) return;

        var selection = playerArmies[0];
        int tileDistance = int.MaxValue;
        foreach (var army in playerArmies)
        {
            if ((army.gridPos- gridpos).sqrMagnitude < tileDistance)
            {
                selection = army;
                tileDistance = (army.gridPos - gridpos).sqrMagnitude;
            }
        }
        selection.Select();
    }
    public void MoveAllArmies()
    {
        var playerArmies = GameManager.main.playerManager.currentPlayer.units;
        if (playerArmies.Count == 0) return;

        foreach (var army in playerArmies)
        {
            army?.movement?.ResolveMovement();
        }
    }
}

public class EntityManager : GameComponent
{
    public virtual void HandleEndOfTurn()
    {

    }
}