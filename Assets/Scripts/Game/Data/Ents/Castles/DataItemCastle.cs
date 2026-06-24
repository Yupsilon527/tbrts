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
    public List<SidewaysTile> castleTiles = new();

    public CityBonuses bonuses;

    public List<UnitData> Production = new List<UnitData>();
    public List<ProductionData> production = new List<ProductionData>();
    public List<Vector2Int> wayPoints = new List<Vector2Int>();
    public float iProductionTime = 0;
    public bool ContinuousProduction = false;
    public DataItemCastle(CistomCastle custom) : base()
    {
        customName = custom.customName;
        customDescription = custom.customDescription;
        isCapital = custom.isCapital;
        ChangeTile(custom.spawnPos);
        bonuses = new(this);
        SetPlayerOwner(custom.ownership);
    }
    public int GetSize()
    {
        return castleTiles.Count;
    }
    public override void ChangeTile(Vector2Int t)
    {
        base.ChangeTile(t);

        castleTiles = new List<SidewaysTile>();

        if (tile.GetWalkElevation() == TerrainDefines.Elevation.City)
        {
            List<SidewaysTile> openList = new List<SidewaysTile>();
            openList.Add(tile);
            while (openList.Count > 0)
            {
                var ct = openList[0];
                if (!castleTiles.Contains(ct))
                    castleTiles.Add(ct);
                foreach (SidewaysTile Zyzyx in ct.neighbors)
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
        foreach (SidewaysTile Gir in castleTiles)
        {
            if (Gir.buildingLayer == null)
            {
                Gir.buildingLayer = this;
            }
        }
    }
    #region Garrison and Control
    public IEnumerable<DataItemArmy> GetGarrison()
    {
        return castleTiles.Select(t => t.armyLayer);
    }

    public int GetMyDefenseLevel()
    {
        int Power = 0;
        foreach (DataItemArmy army in GetGarrison())
        {
            Power += army.GetPowerValue(false);
        }

        return Power;
    }

    public bool AmIBeingTakenOver(DataItemPlayer Conqueror)
    {

        bool Takeover = false;

        if (isRazed() && GetGarrison().Count() > 0)
        {
            Takeover = true;
        }
        else if (GetPlayerOwner() != Conqueror)
        {
            foreach (DataItemArmy army in GetGarrison())
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
        return GetGarrison().Sum(a => a.GetAlignment(this) == PlayerDefines.Alignment.enemy ? 1 : 0) > 0;
    }

    public bool BattleTroop(DataItemArmy Attacker, SidewaysTile Tile)
    {
        if (GetGarrison().Count() > 0)
        {
            foreach (DataItemArmy Zim in GetGarrison())
            {

                if (Zim.GetPlayerOwner() != Attacker.GetPlayerOwner())
                {
                    //   Actions.BattleArmies(game, Attacker, Zim, false, true);
                }
            }
        }
        return !AmIUnderAlliedControl();
    }

    public float GetMyRazeCost(DataFaction OwnerFaction, CastleRazeMode Raze, bool Ruin)
    {//redo

        float Cost = EconomyDefines.CastleRazeReward;

        switch (Raze)
        {
            case CastleRazeMode.raid://raid
                //Cost = EconomyDefines.CastleRaidPercent * GetIncome();
                RaidTurn = GameManager.main.currentTurn;
                break;
            case CastleRazeMode.occupy://occupy
                break;

            case CastleRazeMode.raze://clear

                //clear production
                float Add = 0;
                /*if (Production.Count > 0)
                {
                    Add += Production[Production.Count - 1].GetPurchaseCost(null);
                    if (Ruin)
                    {
                        ChangeProduction(Production[Production.Count - 1].FileName);
                    }
                }
                Cost += Mathf.RoundToInt(Add * Game.iCastleRazePercent);

                //level down
                foreach (var Upgrade in Upgrades)
                {
                    Cost += Upgrade.GetMyCost(null) * Game.iCastleRazePercent;
                }
                if (Ruin)
                {
                    Upgrades.Clear();
                }*/
                break;
        }


        return Cost;
    }
    #endregion
    #region Raze
    public void Demolish()
    {
        SetPlayerOwner(GameManager.main.playerManager.players[0]);
        RazeTurn = GameManager.main.currentTurn;

        Production.Clear();
        production.Clear();
        wayPoints.Clear();
        bonuses.upgrades.researchedUpgrades.Clear();

        display.DrawAgain();
    }

    public void RebuildMe(DataItemPlayer Player)
    {

        SetPlayerOwner(Player);
        RazeTurn = GameManager.main.currentTurn;

        Production.Clear();
        production.Clear();
        wayPoints.Clear();
        bonuses.upgrades.researchedUpgrades.Clear();

        display.DrawAgain();
    }
    public bool isRazed() { return GameManager.main.currentTurn < RazeTurn; }
    #endregion
    #region Production


    public bool CanProduce()
    {
        return !isRazed() && AmIUnderAlliedControl();
    }
    public void AddProduction(int army, bool instant)
    {
        if (army < Production.Count)
        {
          //  if (game.RuleSet.CurrentTurn <= 1 && PlayerOwner.DoIHaveEnoughGold(Production[army].GetPower(true) * Game.iArmyInstantPurchaseMultiplier, false, false))
            {

                Produce(army);
            }
            //else
            {
                production.Add(Production[army]) ;
            }
        }
    }
    public bool CanMakeTroop(DataItemArmy Army)
    {
        return GetValidTileForArmy(Army, false) != null;
    }

    public void Produce(int army)
    {
        if (CanIForwardProduction())
        {
            production[0].CompleteProduction(new ProductionTable(GetPlayerOwner(), tile.GetWorldPosition(), tile.gridPos, this));
        }
    }

    public bool CanIForwardProduction()
    {
        if (production.Count == 0)
        {
            return false;
        }
        else
        {
            var costs = production[0].GetCostForPlayer(GetPlayerOwner());
            return iProductionTime >= costs[(int)EconomyDefines.EconomyResource.Labor].value && GetPlayerOwner().econ.CanAffordResources(costs);
        }
    }

    public SidewaysTile GetValidTileForArmy(string Army, bool AccArmies)
    {
        return null;// GetValidTileForArmy(game.game.LoadArmy(Army, false), AccArmies);
    }
    public SidewaysTile GetValidTileForArmy(DataItemArmy Army, bool AccArmies)
    {
       /* int movement = Army.GetMovetype();
        List<SidewaysTile> Temp = new List<SidewaysTile>();

        foreach (SidewaysTile tile in castleTiles)
        {

            if (!Temp.Contains(tile) && Pathfinder.CanIWalkOver(Army.GetMovetype(), tile.iElevation, tile.isRoad))
            {
                if (!AccArmies || tile.ArmyLocated == null)
                {

                    Temp.Add(tile);
                }
                else if (tile.ArmyLocated.GetOwner().GetAlliance(PlayerOwner) == 0 && tile.ArmyLocated.CanIAccept(Army.GetCommand()))
                {
                    Temp.Add(tile);
                }
            }

            foreach (SidewaysTile Stocking in tile.GetNeighbors(game))
            {

                if (Stocking != null && !Temp.Contains(Stocking) && Pathfinder.CanIWalkOver(Army.GetMovetype(), Stocking.iElevation, Stocking.isRoad))
                {
                    if (!AccArmies || Stocking.ArmyLocated == null)
                    {

                        Temp.Add(Stocking);
                    }
                    else if (Stocking.ArmyLocated.GetOwner().GetAlliance(PlayerOwner) == 0 && Stocking.ArmyLocated.CanIAccept(Army.GetCommand()))
                    {
                        Temp.Add(Stocking);
                    }
                }

            }
        }
        Temp.Sort(delegate (SidewaysTile x, SidewaysTile y)
        {

            if (x.CityLocated == this && y.CityLocated == this)
            {
                return 0;
            }
            else if (x.CityLocated == null || x.CityLocated != this)
            {
                if (y.CityLocated == null || y.CityLocated != this)
                {
                    return 0;
                }
                return 1;
            }
            else if (y.CityLocated == null || y.CityLocated != this)
            {
                if (x.CityLocated == null || x.CityLocated != this)
                {
                    return 0;
                }
                return -1;
            }
            return 0;
        });

        if (Temp.Count > 0)
        {
            return Temp[0];
        }
        else*/
            return null;

    }
    public List<DataItemArmy> GetAvailableProduction(bool affordable)
    {
        List<DataItemArmy> ProductionArmies = new List<DataItemArmy>();
        /*foreach (DataItemArmy Panty in PlayerOwner.Faction.GetRecruitableArmies(game.game))
        {
            if (CanMakeTroop(Panty) && !ProductionNames.Contains(Panty.FileName) &&
                (!affordable || PlayerOwner.DoIHaveEnoughGold(Panty.GetPurchaseCost(PlayerOwner), true, false)) &&
                GetValidTileForArmy(Panty, false) != null)
            {

                ProductionArmies.Add(Panty);
            }
        }*/ //TODO
        return ProductionArmies;
    }
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
    public override void OnTurnEnd()
    {
        base.OnTurnEnd();


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
    #region LoS
    public override bool IsVisibleToPlayer(DataItemPlayer player)
    {
        return castleTiles.Any(t => t.IsRevealedByPlayer(player,UnitDefines.TileVisibility.visible));
    }

    #endregion
}
