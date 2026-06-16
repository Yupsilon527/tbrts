using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemCastle : DataItemBuilding
{
    public string customName, customDescription;
    public Sprite citySprite;
    public bool isCapital = false;
    public int RazeTurn = -1;
    public List<SidewaysTile> castleTiles = new();


    public List<DataItemArmy> Production = new List<DataItemArmy>();
    public List<int> iProduction = new List<int>();
    public float iProductionTime = 0;
    public bool ContinuousProduction = false;
    public DataItemCastle(CistomCastle custom) : base()
    {
        customName = custom.customName;
        customDescription = custom.customDescription;
        isCapital = custom.isCapital;
        PlaceOnTile(custom.spawnPos);
    }

    public int GetSize()
    {
        return castleTiles.Count;
    }
    public override void PlaceOnTile(Vector2Int t)
    {
        base.PlaceOnTile(t);

        castleTiles = new List<SidewaysTile>();

        if (tile.elevation.elevation == TerrainDefines.Elevation.City)
        {
            List<SidewaysTile> openList = new List<SidewaysTile>();
            openList.Add(tile);
            while (openList.Count > 0)
            {
                var ct = openList[0];
                castleTiles.Add(ct);
                foreach (SidewaysTile Zyzyx in ct.neighbors)
                {
                    if (Zyzyx.elevation.elevation == TerrainDefines.Elevation.City && Zyzyx.buildingLayer == null)
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
    public bool isRazed() { return GameManager.main.currentTurn < RazeTurn; }
    #region Garrison and Control
    public IEnumerable<DataItemArmy> GetGarrison()
    {
        return castleTiles.Select(t => t.armyLayer);
    }

    public int GetMyDefenseLevel()
    {
        int Power = 0;
        foreach (entityArmy Panty in GetGarrison())
        {
            Power += Panty.GetPowerValue(false);
        }

        return Power;
    }

    public bool AmIRevealedByPlayer(DataItemPlayer Player)
    {
        return castleTiles.Any(t => t.IsRevealedByPlayer(Player));
    }

    public bool AmIBeingTakenOver(entityPlayer Conqueror)
    {

        bool Takeover = false;

        if (isRazed() && GetGarrison().Count > 0)
        {
            Takeover = true;
        }
        else if (PlayerOwner != Conqueror)
        {

            foreach (entityArmy Panty in GetGarrison())
            {
                if (Panty.GetOwner() == Conqueror)
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

    public bool BattleTroop(entityArmy Attacker, entityTile Tile)
    {
        if (GetGarrison().Count > 0)
        {
            foreach (entityArmy Zim in GetGarrison())
            {

                if (!Zim.isDead() && Zim.GetOwner() != Attacker.GetOwner())
                {
                    Actions.BattleArmies(game, Attacker, Zim, false, true);
                }
                if (Attacker.isDead())
                {
                    break;
                }
            }
        }
        return !AmIUnderAlliedControl();
    }

    public float GetMyRazeCost(DataFaction OwnerFaction, int Raze, bool Ruin)
    {//redo

        float Cost = Game.iCastleRazeBonus;

        switch (Raze)
        {
            case 0://raid
                Cost = Game.iCastleRaidPercent * GetIncome();
                //damages population
                if (Ruin)
                {
                    ClearPopulation();
                }
                razedThisTurn = true;
                break;
            case 1://occupy

                if (Ruin)
                {
                    ClearPopulation();
                }
                break;

            case 2://clear

                //clear production
                float Add = 0;
                if (Production.Count > 0)
                {
                    Add += Production[Production.Count - 1].GetPurchaseCost(null);
                    if (Ruin)
                    {
                        ChangeProduction(Production[Production.Count - 1].FileName);
                    }
                }
                Cost += Mathf.RoundToInt(Add * Game.iCastleRazePercent);

                //level down
                foreach (DataItemBuilding Upgrade in Upgrades)
                {
                    Cost += Upgrade.GetMyCost(null) * Game.iCastleRazePercent;
                }
                if (Ruin)
                {
                    Upgrades.Clear();
                    //ClearPopulation ();
                }
                break;
        }


        return Cost;
    }
    #endregion
    #region Production


    public bool CanProduce()
    {
        return !isRazed() && AmIUnderAlliedControl();
    }
    public void AddProduction(DataItemArmy army)
    {
        int iArmy = Production.IndexOf(army);
        if (iArmy >= 0)
        {
            AddProduction(iArmy);
        }
    }
    public void AddProduction(int army)
    {
        if (army < Production.Count)
        {
            if (game.RuleSet.CurrentTurn <= 1 && PlayerOwner.DoIHaveEnoughGold(Production[army].GetPower(true) * Game.iArmyInstantPurchaseMultiplier, false, false))
            {

                Produce(army);
            }
            else
            {
                iProduction.Add(army);
            }
        }
    }
    public bool CanMakeTroop(DataItemArmy Army)
    {
        if (Army == null || Army.GetAbility("mercenary") > 0 || Army.GetAbility("require") > GetLevel())
        {
            return false;
        }
        return GetValidTileForArmy(Army, false) != null;
    }

    public entityUnit Produce(int army)
    {
        PlayerOwner.VictoryStats.UnitsBuilt++;

        return entityArmy.SpawnUnit(game, Production[army], PlayerOwner, GetValidTileForArmy(Production[army], true), this);

    }

    public bool CanIForwardProduction()
    {
        if (iProduction.Count == 0)
        {
            return false;
        }
        else
        {
            return (iProductionTime >= Production[iProduction[0]].Stats[DataItemArmy.Stat_Resource] /*&& PlayerOwner.DoIHaveEnoughGold ( Production [iProduction [0]].GetPower (false, PlayerOwner), true, false)*/);   //no longer charges when spawning troops
        }
    }

    public entityTile GetValidTileForArmy(string Army, bool AccArmies)
    {
        return GetValidTileForArmy(game.game.LoadArmy(Army, false), AccArmies);
    }
    public entityTile GetValidTileForArmy(DataItemArmy Army, bool AccArmies)
    {
        int movement = Army.GetMovetype();
        List<entityTile> Temp = new List<entityTile>();

        foreach (entityTile Panty in myTiles)
        {

            if (!Temp.Contains(Panty) && Pathfinder.CanIWalkOver(Army.GetMovetype(), Panty.iElevation, Panty.isRoad))
            {
                if (!AccArmies || Panty.ArmyLocated == null)
                {

                    Temp.Add(Panty);
                }
                else if (Panty.ArmyLocated.GetOwner().GetAlliance(PlayerOwner) == 0 && Panty.ArmyLocated.CanIAccept(Army.GetCommand()))
                {
                    Temp.Add(Panty);
                }
            }

            foreach (entityTile Stocking in Panty.GetNeighbors(game))
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
        Temp.Sort(delegate (entityTile x, entityTile y) {

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
        else
            return null;

    }
    public List<DataItemArmy> GetAvailableProduction(bool affordable)
    {
        List<DataItemArmy> ProductionArmies = new List<DataItemArmy>();
        foreach (DataItemArmy Panty in PlayerOwner.Faction.GetRecruitableArmies(game.game))
        {
            if (CanMakeTroop(Panty) && !ProductionNames.Contains(Panty.FileName) &&
                (!affordable || PlayerOwner.DoIHaveEnoughGold(Panty.GetPurchaseCost(PlayerOwner), true, false)) &&
                GetValidTileForArmy(Panty, false) != null)
            {

                ProductionArmies.Add(Panty);
            }
        }
        return ProductionArmies;
    }
    #endregion
    #region Income
    public int GetResourceIncome()
    {
        int res = game.RuleSet.DefaultResources + (int)GetBonus("resources");

        if (PlayerOwner.ID == Game.iNeutrals)
        {
            res = game.RuleSet.NeutralsStrength + (int)GetBonus("resources");
        }

        return res;
    }
    #endregion
    public override void OnTurnEnd()
    {
        base.OnTurnEnd();


            if (AmIUnderAlliedControl())
            {


                if (CanProduce() && iProduction.Count > 0)
                {
                    iProductionTime += GetResourceIncome();
                    while (iProduction.Count > 0 && CanIForwardProduction())
                    {

                        //if (PlayerOwner.DoIHaveEnoughGold ( Production [iProduction [0]].GetCost (false, PlayerOwner), false, false)) { -- No longer pay for troop spawning

                        iProductionTime -= Production[iProduction[0]].Stats[DataItemArmy.Stat_Resource];
                        Produce(iProduction[0]);

                        if (ContinuousProduction)
                        {
                            iProduction.Add(iProduction[0]);
                        }
                        iProduction.RemoveAt(0);
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
            }
    }
}
