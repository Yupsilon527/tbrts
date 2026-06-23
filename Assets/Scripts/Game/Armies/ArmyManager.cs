using System.Collections.Generic;
using Unity.Burst.Intrinsics;

public class ArmyManager : EntityManager
{
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

    public static DataItemUnit SpawnUnit( string UnitName, DataItemPlayer Player, SidewaysTile tTile)
    {
        return SpawnUnit(WorldManager.main.LoadUnit(UnitName), Player, tTile);
    }
    public static DataItemUnit SpawnUnit(UnitData uData, DataItemPlayer Player, SidewaysTile tTile)
    {
        return SpawnUnit( uData, Player, tTile, null);
    }
    public static DataItemUnit SpawnUnit(UnitData uData, DataItemPlayer Player, SidewaysTile tTile, DataItemCastle myCastle)
    {
        if (tTile == null)
        {
            return null;
        }

        DataItemArmy newArmy = tTile.armyLayer;
        if (newArmy == null || !newArmy.formation.CanIAccept(uData.GetCommandValue()))
        {
            newArmy = new DataItemArmy(tTile.gridPos, Player.ID);
            //Panty.Exhaust();
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
        armies.Remove(army);
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
        if (army.dead)
        {
            army.Despawn();
        }
        armies.Remove(army);
        if (army.display == null)
        {
            GameManager.main.displayPool.DeactivateObject(army.display.GetParentObject());
        }
    }

    public void SelectArmy(DataItemArmy army)
    {
        ClearSelectedArmy();
        if (army != null)
        {
            mainSelectedArmy = army;
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
}

public class EntityManager : GameComponent
{
    public virtual void HandleEndOfTurn()
    {

    }
}