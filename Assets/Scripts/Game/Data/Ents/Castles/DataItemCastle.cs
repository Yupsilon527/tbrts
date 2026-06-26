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
    }
    public int GetSize()
    {
        return castleTiles.Count;
    }
    public override void ChangeTile(Vector2Int t, DisplayPositionChange position)
    {
        base.ChangeTile(t, position);

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
        return GetGarrison().Sum(a => a?.GetAlignment(this) == PlayerDefines.Alignment.enemy ? 1 : 0) > 0;
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
    public void OnNewTurnBegin()
    {

        if (RazeTurn > 0 && GameManager.main.currentTurn >= RazeTurn)
        {
            RebuildMe(GameManager.main.playerManager.neutrals);
        }
        else
        {
            bonuses.OnTurnBegin();
            production.OnTurnBegin();
            income.OnTurnBegin();
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
    public bool isRazed() { return GameManager.main.currentTurn < RazeTurn; }
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
