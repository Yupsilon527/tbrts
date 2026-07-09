using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BuildingDefines;

public class DataItemCastle : DataItemBuilding
{
    public string customName, customDescription;
    public Sprite citySprite;
    public bool isCapital = false;
    public int RaidTurn, RazeTurn = -1;

    public CityBonuses bonuses;
    public CityProduction production;
    public CityIncome income;

    public DataItemCastle(CistomCastle custom) : base()
    {
        customName = custom.customName;
        customDescription = custom.customDescription;
        isCapital = custom.isCapital;
        ChangeTile(custom.spawnPos, DisplayPositionChange.instant);
        bonuses = new(this);
        production = new(this);
        income = new(this);
        SetPlayerOwner(custom.ownership);
        bonuses.GrantFreeBuildings();
    }
    public int GetSize()
    {
        return occupiedTiles.Count;
    }
    public override void SetPlayerOwner(DataItemPlayer player)
    {
        if (GetPlayerOwner() != null)
        {
            GetPlayerOwner().buildings.Remove(this);
        }
        base.SetPlayerOwner(player);
        GetPlayerOwner().buildings.Add(this);
    }
    public override void ChangeTile(Vector2Int t, DisplayPositionChange position)
    {
        base.ChangeTile(t, position);

        occupiedTiles = new List<DataItemTile>();

        var tile = SidewaysMap.main.GetTile(t);
        if (tile.GetWalkElevation() == TerrainDefines.Elevation.City)
        {
            List<DataItemTile> openList = new List<DataItemTile>();
            openList.Add(tile);
            while (openList.Count > 0)
            {
                var ct = openList[0];
                if (!occupiedTiles.Contains(ct))
                    occupiedTiles.Add(ct);
                foreach (DataItemTile Zyzyx in ct.neighbors)
                {
                    if (Zyzyx.GetWalkElevation() == TerrainDefines.Elevation.City && Zyzyx.buildingLayer == null)
                    {
                        Zyzyx.buildingLayer = this;
                        openList.Add(Zyzyx);
                    }

                }
                openList.RemoveAt(0);
            }
        }
        foreach (DataItemTile Gir in occupiedTiles)
        {
            if (Gir.buildingLayer == null)
            {
                Gir.buildingLayer = this;
            }
        }
    }
    #region Garrison and Control
    public IEnumerable<DataItemArmy> GetSituatedArmies(bool friendlies)
    {
        if (friendlies)
            if (AmIDemolished())
                return Array.Empty<DataItemArmy>();
            else
            return occupiedTiles.Select(t => t.armyLayer).Where(a => a!=null && a.GetAlignment(this) == PlayerDefines.Alignment.playerowned);
        return occupiedTiles.Select(t => t.armyLayer);
    }

    public int GetMyDefenseLevel()
    {
        int Power = 0;
        foreach (DataItemArmy army in GetSituatedArmies(true))
        {
            Power += army.GetPowerValue(false);
        }

        return Power;
    }

    public bool AmIBeingTakenOver(DataItemPlayer Conqueror)
    {

        bool Takeover = false;

        if (AmIDemolished() && GetSituatedArmies(false).Count() > 0)
        {
            Takeover = true;
        }
        else if (GetPlayerOwner() != Conqueror)
        {
            foreach (DataItemArmy army in GetSituatedArmies(false))
            {
                if (army.GetPlayerOwner() == Conqueror)
                {
                    Takeover = true;
                }
            }

        }
        return Takeover;
    }
    public bool AmIUnderAlliedControl()
    {
        return GetSituatedArmies(false).Sum(a => a?.GetAlignment(this) == PlayerDefines.Alignment.enemy ? 1 : 0) == 0;
    }

    public bool BattleTroop(DataItemArmy Attacker, DataItemTile Tile)
    {
        if (GetSituatedArmies(false).Count() > 0)
        {
            foreach (DataItemArmy Zim in GetSituatedArmies(true))
            {

                if (Zim.GetPlayerOwner() != Attacker.GetPlayerOwner())
                {
                    Attacker.BattleAnother(Zim) ;
                    return false;
                }
            }
        }
        return !AmIUnderAlliedControl();
    }

    #endregion
    public virtual void OnArmyEnterCastle(DataItemArmy army, bool region)
    {
        foreach (var bonus in bonuses.GetBonusesByType(region ? BuildingData.GrantBonus.aura : BuildingData.GrantBonus.garrison))
        {
            if (bonus == null) continue;
            foreach (var unit in army.formation.GetUnits())
            {
                if (unit == null) continue;
                if (!region)
                    unit.upgrades.upgrades.CatchUpUpgrade(bonus.upgrade, bonus.level);
                else
                    unit.upgrades.upgrades.ApplyBonus(bonus.upgrade, bonus.level);
            }
        }
    }
    public virtual void OnArmyLeaveCastle(DataItemArmy army, bool region)
    {
        foreach (var bonus in bonuses.GetBonusesByType(region ? BuildingData.GrantBonus.aura : BuildingData.GrantBonus.garrison))
            {
            if (bonus == null) continue;
            foreach (var unit in army.formation.GetUnits())
            {
                unit.upgrades.upgrades.RevertUpgrade(bonus.upgrade, bonus.level);
        }
        }
    }
    #region Raze
    public void Demolish()
    {
        SetPlayerOwner(GameManager.main.playerManager.players[0]);
        RazeTurn = GameManager.main.currentTurn;

        bonuses.OnCastleRaze();
        income.OnCastleRaze();
        production.OnCastleRaze();

        display.DrawAgain();
    }

    public void RebuildMe(DataItemPlayer Player)
    {

        SetPlayerOwner(Player);
        RazeTurn = GameManager.main.currentTurn;

        bonuses.OnCastleRaze();
        income.OnCastleRaze();
        production.OnCastleRaze();

        display.DrawAgain();
    }
    public bool AmIDemolished() { return GameManager.main.currentTurn < RazeTurn; }
    #endregion
    #region Income TODO
    /* public int GetResourceIncome()
     {
         int res = game.RuleSet.DefaultResources + (int)GetBonus("resources");

         if (PlayerOwner.ID == Game.iNeutrals)
         {
             res = game.RuleSet.NeutralsStrength + (int)GetBonus("resources");
         }

         return res;
     }

     public int GetIncome()
     {
         return Game.iCastleIncomeBase * GetSize() + Game.iCastleIncomeLevel * GetLevel() + (int)GetBonus("income");

     }*/
    #endregion
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();


        if (RazeTurn > 0 && GameManager.main.currentTurn >= RazeTurn)
        {
            RebuildMe(GameManager.main.playerManager.neutrals);
        }
        bonuses.OnTurnBegin();
        production.OnTurnBegin();
        income.OnTurnBegin();

        /*if (AmIUnderAlliedControl())
        {


            if (CanProduce() && production.Count > 0)
            {
                iProductionTime += GetResourceIncome();
                while (production.Count > 0 && CanIForwardProduction())
                {

                    //if (PlayerOwner.DoIHaveEnoughGold ( Production [iProduction [0]].GetCost (false, PlayerOwner), false, false)) { -- No longer pay for troop spawning

                    iProductionTime -= Production[production[0]].Stats[DataItemArmy.Stat_Resource];
                    Produce(production[0]);

                    if (ContinuousProduction)
                    {
                        production.Add(production[0]);
                    }
                    production.RemoveAt(0);
                    //}
                }
            }


        }
        else
        {
            if (RazeTurn > 0 && game.RuleSet.CurrentTurn >= RazeTurn)
            {
                RebuildMe(game.Players[Game.iNeutrals]);
            }
        }*/
    }
    public  void RazeCastle(DataItemArmy attacker,  CastleRazeMode razeMode)
    {
        var raidingPlayer = attacker.GetPlayerOwner();
        var raidedPlayer = GetPlayerOwner();

        bool canSteal = attacker.formation.HasAbility("raider");
        int stealStrength = attacker.formation.GetAbilitiySum("vandal");

        float earnedMetal            = EconomyDefines.CastleRazeRewardMetal * (stealStrength + 5 ) / 5;
        float earnedGold = EconomyDefines.CastleRazeRewardGold * (stealStrength + 3) / 5;
        switch (razeMode)
        {
            case CastleRazeMode.occupy:
                earnedMetal = 0;
                earnedGold = 0;
                SetPlayerOwner(attacker.GetPlayerOwner());
                attacker.Exhaust();
                break;

            case CastleRazeMode.raid:

                attacker.Exhaust();
                break;
            case CastleRazeMode.sack:
                foreach (var building in bonuses.upgrades.researchedUpgrades.ToArray())
                {
                    if (building.upgrade.flags.Contains("defensive")) //TODO
                        bonuses.upgrades.RemoveUpgrade(building.upgrade);
                }
                earnedMetal /= 3;
                earnedGold /= 2;

                attacker.Exhaust();
                break;
            case CastleRazeMode.raze:

                earnedMetal /= 4;
                earnedGold /= 3;

                Demolish();
                attacker.Exhaust();
                break;

        }
        if (canSteal)
        {
            float stolenMetal = Mathf.Min(earnedMetal / 3, raidedPlayer.econ.GetResourceValue(EconomyDefines.EconomyResource.Metal)/4);
            float stolenGold = Mathf.Min(earnedGold / 4, raidedPlayer.econ.GetResourceValue(EconomyDefines.EconomyResource.Gold)/5);

            raidingPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal).GiveValue(raidedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal).SubstractedValue(stolenMetal));
            raidingPlayer.econ.GetResource(EconomyDefines.EconomyResource.Gold).GiveValue(raidedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Gold).SubstractedValue(stolenGold));
        }

        raidingPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal).GiveValue(earnedMetal);
        raidingPlayer.econ.GetResource(EconomyDefines.EconomyResource.Gold).GiveValue(earnedGold);
    }

}
