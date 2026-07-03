using Astar;
using System.Linq;
using UnityEngine;

public class DataItemArmy : DataItemObject
{

    public ArmyFormation formation;
    public ArmyAbilities abilities;
    public ArmyMovementComponent movement;
    public ArmyStatusComponent status;
    public OrderComponent orders;
    public Pathfinder pathfinder;
    public DataItemArmy() : base()
    {
        formation = new(this);
        movement = new(this);
        status = new(this);
        orders = new(this);
        pathfinder = new(this);
        abilities = new(this);
        GameManager.main.armyManager.RegisterArmy(this);
    }
    public override string ToString()
    {
        return "Army " + eID;
    }
    public DataItemArmy(Vector2Int pos, int playerOwner) : this()
    {
        ChangeTile(pos, DisplayPositionChange.instant);
        SetPlayerOwner(playerOwner);
    }

    public DataItemArmy(CustomArmy army) : this(army.spawnPos, army.ownership)
    {
        for (int i = 0; i < army.formation.Length; i++)
        {
            var unit = WorldManager.main.LoadUnit(army.formation[i]);
            if (unit != null)
            {
                formation.GiveUnitInPosition(i, unit);
            }
        }
        if (!string.IsNullOrEmpty(army.transporter))
        {
            var unit = WorldManager.main.LoadUnit(army.transporter);
            if (unit != null)
            {
                formation.GiveUnitInPosition(-1, unit);
            }
        }
        foreach (var u in formation.GetUnits())
        {
            GetPlayerOwner().upgrades.ApplyResearchedUpgradeToNewlySpawnedUnit(u);
            if (tile.buildingLayer is DataItemCastle castle)
                castle.bonuses.ApplyResearchedUpgradeToNewlySpawnedUnit(u);
            u.FireEventOnSelf(AbilityDefines.Event.OnSpawn);
        }
        display?.OnGraphicsChange();
    }
    #region Move
    public virtual bool ChangeTile(Vector2Int t, int cost, DisplayPositionChange m)
    {
        if (movement.CanPayMovement(cost))
        {
            movement.PayMovement(cost);
            ChangeTile(t, m);
            return true;
        }
        return false;
    }
    public override void ChangeTile(Vector2Int t, DisplayPositionChange m)
    {
        Vector2Int oldtile = t;
        var newTile = SidewaysMap.main.GetTile(t);
        if (tile != null)
        {
            tile.armyLayer = null;
            oldtile = tile.gridPos;

            var exitCastle = tile.buildingLayer as DataItemCastle;
            var enterCastle = newTile.buildingLayer as DataItemCastle;

            var exitRegion = tile.regionCastle as DataItemCastle;
            var enterRegion = newTile.regionCastle as DataItemCastle;

            ChangeRegion(enterRegion, exitRegion);
            ChangeCastle(enterCastle, exitCastle);
        }
        tile = newTile;
        gridPos = t;
        tile.armyLayer = this;
        UpdateLoSAroundTile(gridPos);
        UpdateLoSAroundTile(oldtile);
        display?.OnPositionChange(t, m);
    }
    void ChangeRegion(DataItemCastle enter, DataItemCastle exit)
    {
        if (exit != enter)
        {
            if (exit != null && exit.GetAlignment(this) == PlayerDefines.Alignment.playerowned)
                exit.OnArmyLeaveCastle(this, true);
            if (enter != null && enter.GetAlignment(this) == PlayerDefines.Alignment.playerowned)
                enter.OnArmyEnterCastle(this, true);
        }
    }
    void ChangeCastle(DataItemCastle enter, DataItemCastle exit)
    {
        if (exit != enter)
        {
            if (exit != null && exit.GetAlignment(this) == PlayerDefines.Alignment.playerowned)
                exit.OnArmyLeaveCastle(this, false);
            if (enter != null && enter.GetAlignment(this) == PlayerDefines.Alignment.playerowned)
                enter.OnArmyEnterCastle(this, false);
        }
    }
    public bool MoveToTile(Vector2Int t, bool attack)
    {
        var ntile = SidewaysMap.main.GetTile(t);
        var m = movement.GetMyMovement();
        int totalMove = ntile.GetMoveCost(m);
        if (ntile != null)
        {
            if (ntile.armyLayer != null )
            {
                if (ntile.armyLayer.GetAlignment(this) == PlayerDefines.Alignment.enemy)
                {
                    if (attack) BattleAnother(ntile.armyLayer);
                    else return false;
                }
                else
                {
                    var currentOrder = orders.GetCurrentOrder();

                    
                        if (currentOrder.path.Remaining() > 1)
                        {
                            var firstTile = currentOrder.path.Following(1);
                            var secondTile = currentOrder.path.Following(2);
                            if (movement.CanWalkOnTile(firstTile) && movement.CanWalkOnTile(secondTile))
                            {
                                totalMove = firstTile.GetMoveCost(m) + secondTile.GetMoveCost(m);
                                return ChangeTile(t, totalMove, DisplayPositionChange.move);
                            }
                    }
                }
            }
            else if (ntile.buildingLayer != null)
            {
                if (attack && ntile.buildingLayer.GetAlignment(this) == PlayerDefines.Alignment.enemy && ntile.buildingLayer is DataItemCastle enemyCastle)
                {
                    enemyCastle.BattleTroop(this, ntile);
                    return false;
                }

                /*if (getMyTile().RuinData != null)
                {
                    getMyTile().RuinData.RevealedByPlayer[GetOwner().ID] = true;
                    game.Events.Add(Reporter.EventReport.ReportGameRuinDiscover(game, getMyTile().RuinData, GetOwner()));
                }

                if (getMyTile().PowerUp != null)
                {
                    getMyTile().PowerUp.Apply(this);
                }*/
            }
            return ChangeTile(t, totalMove, DisplayPositionChange.move);
        }
        return false;
    }
    #endregion
    public bool IsAlive()
    {
        return dead || formation.GetUnits().Length > 0;
    }
    public bool CanAttack()
    {
        return movement.movementLeft > 0;
    }
    public bool IsInCombat()
    {
        return Combat.main.attackers == this || Combat.main.defenders == this;
    }
    public override void SetPlayerOwner(DataItemPlayer player)
    {
        if (GetPlayerOwner() != null)
        {
            GetPlayerOwner().units.Remove(this);
        }
        base.SetPlayerOwner(player);
        foreach (var unit in formation.GetUnits())
        {
            unit.SetPlayerOwner(player);
        }
        GetPlayerOwner().units.Add(this);
        orders.Clear();
    }

    public void ApplyEffect(ApplyEffects effect) { }
    public int GetAuraBonuses(UnitDefines.ArmyAbilities Bonus)
    {
        //       if (tile.buildingLayer != null && tile.buildingLayer.GetAlignment(this) ==  PlayerDefines.Alignment.playerowned && tile.buildingLayer is DataItemCastle castle)
        //     {
        //       return 0;//TODO (Bonus);
        // }
        return 0;
    }

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        formation.OnTurnBegin();
        movement.OnTurnBegin();
        status.OnTurnBegin();
        orders.OnTurnBegin();

    }
    #region Power

    public int GetPowerValue(bool accountPenalty)
    {
        return formation.Formation.Sum(u => u.GetPowerValue(accountPenalty)) + formation.transport?.GetPowerValue(accountPenalty) ?? 0;
    }
    public float GetUpkeep()
    {
        return GetPowerValue(true) * UnitDefines.fSalaryMultiplier;
    }
    #endregion
    #region Merging
    public bool CanBeMerged(bool forced)
    {
        if (!forced && movement.movementLeft > 0)
        {
            return false;
        }

        /*foreach (entityUnit Zim in getUnits(true))
        {
            if (Zim.getOwner(false) != Zim.getOwner(true) || Zim.HasModifier(entityModifier.Names.mercenary))
            {
                return false;
            }
        }*/
        return true;
    }
    public bool CanWeMerge(DataItemArmy other)
    {
        if (CanBeMerged(false) && other.CanBeMerged(false))
        {
            return (other.GetAlignment(this) == PlayerDefines.Alignment.playerowned && other.formation.CanIAccept(formation.GetCommandValue()));
        }
        else
        {
            return false;
        }
    }
    public bool Transfer(DataItemArmy other, bool Instant)
    {

        if (!Instant && GameManager.main.playerManager.GetActivePlayer().IsAiControlled())
        {
            //            game.game.InGameMenus.OpenWindow(new ArmyUINew(game.game, this, Defender));
            return false;
        }

        foreach (var unit in formation.Formation)
        {
            other.formation.TransferUnit(unit);
        }

        if (AmISelected(false))
        {
            GameManager.main.armyManager.SelectArmy(other);
        }
        other.movement.movementLeft = movement.movementLeft;
        other.display.OnGraphicsChange();
        Despawn();
        return true;
    }
    #endregion
    #region LoS
    public int GetLineOfSight()
    {
        return Mathf.Max(1, UnitDefines.iArmyBaseLoS + formation.GetAbilitiyMax("scouting"));
    }
    public float GetTrueSight()
    {
        return Mathf.Min(UnitDefines.iArmyBaseLoS + formation.GetAbilitiyMax("spies"), GetLineOfSight());
    }
    public override bool IsVisibleToPlayer(DataItemPlayer player)
    {
        if (GetAlignment(player) == PlayerDefines.Alignment.enemy)
        {
            return true;    //TODO LoS
            return tile.IsRevealedByPlayer(player, status.IsCloaked() ? UnitDefines.TileVisibility.truesight : UnitDefines.TileVisibility.visible);
        }
        return base.IsVisibleToPlayer(player);
    }
    public void UpdateLoSAroundTile(Vector2Int tile)
    {

    }
    #endregion
    public bool BattleAnother(DataItemArmy other, bool showPopup = true)
    {
        if (other.IsAlive() && tile.IsNeighboring(other.tile) && GetAlignment(other) == PlayerDefines.Alignment.enemy && movement.CanPayMovement(4))
        {
            if (showPopup)
            {
                InterfaceManager.main.OpenPrepareCombatWindow(this,other);
                return false;
            }
            else {
                Exhaust(4);
                Combat.main.BattleTroops(this, other, other.tile);
                foreach (var unit in formation.GetUnits())
                {
                    Combat.main.Inspect($"Unit {unit} remaining with {unit.health.GetValue()} health!");
                }
                foreach (var unit in other.formation.GetUnits())
                {
                    Combat.main.Inspect($"Unit {unit} remaining with {unit.health.GetValue()} health!");
                }
                return true;
            }

        }
        return false;
    }
    public bool CanInvadeCastle(DataItemCastle castle)
    {
        return !castle.IsDemolished() && castle.GetAlignment(this) == PlayerDefines.Alignment.enemy && CanAttack() && formation.GetAbilitiyMax("raze") > 0;
    }
    public void PostDamageUpdate()
    {
        foreach (var unit in formation.GetUnits())
        {
            if (!unit.IsAlive())
                formation.RemoveTroop(unit, false);
        }
        if (formation.GetUnits(incDead: false).Length == 0)
            Despawn();
        else
            formation.OnFormationUpdate();
    }
    #region Selection
    public override void SetSelected(bool value)
    {
        if (value == Selected) return;
        base.SetSelected(value);
        if (value)
            GameManager.main.armyManager.SelectArmy(this);
    }
    public bool AmISelected(bool Moving)
    {
        if (Moving)
        {
            return GameManager.main.armyManager.mainSelectedArmy == this || GameManager.main.armyManager.movingArmies.Contains(this);
        }
        return GameManager.main.armyManager.mainSelectedArmy == this;
    }
    #endregion

    #region Bribes/Mercs
    public bool CanBeBribed(DataItemArmy Attacker)
    {
        return (GetMyBribeCost() > 0 && Attacker.GetPowerValue(false) > GetPowerValue(false));
    }

    public bool isMercenary()
    {
        foreach (var unit in formation.GetUnits())
        {
            if (unit.innates.HasAbility("mercenary"))
                return true;
        }
        return false;
    }

    public int GetMyBribeCost()
    {
        if (!CanBeMerged(false) || isMercenary() || GetPlayerOwner().isNeutral())
        {
            return -1;
        }

        return Mathf.RoundToInt(GetPowerValue(false) * UnitDefines.fBribeMultiplier);
    }
    #endregion
    public void Revise()
    {
        formation.OnFormationUpdate();
        display?.DrawAgain();
    }
    public void Exhaust(int amt = 9999)
    {
        movement.PayMovement(amt);
    }
    public override void Despawn()
    {
        if (!dead)
        {
            dead = true;
            base.Despawn();
            UpdateLoSAroundTile(gridPos);

            tile.armyLayer = null;
            if (IsSelected()) GameManager.main.armyManager.ClearSelectedArmy();

            GameManager.main.armyManager.ForgetArmy(this);
        }
    }

    public bool HasAura()
    {
        return GetAuraRange() > 0;
    }
    public override int GetAuraRange()
    {
        return formation.GetAbilitiyMax("aura");
    }

    public bool CanAssist(DataItemArmy other)
    {
        if (GetAlignment(other) == PlayerDefines.Alignment.enemy)
            return false;
        if (formation.GetAbilitiyMax("defenseSupport") > 0 && other.tile.buildingLayer == tile.buildingLayer)
            return true;
        if (formation.GetAbilitiyMax("rangeSupport") > 0 && (other.gridPos - gridPos).magnitude > GetAssistRange())
            return true;
        return false;
    }
    public int GetAssistRange()
    {
        return formation.GetAbilitiyMax("rangeSupport");
    }
}
