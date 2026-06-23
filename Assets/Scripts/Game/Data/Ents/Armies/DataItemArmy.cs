using System.Linq;
using UnityEngine;

public class DataItemArmy : DataItemObject
{
    public DataItemArmy() : base()
    {
        formation = new(this);
        movement = new(this);
        status = new(this);
        orders = new(this);
    }
    public DataItemArmy (Vector2Int pos, int playerOwner):this()
    {

        ChangeTile(pos);
        SetPlayerOwner(playerOwner);
        GameManager.main.armyManager.RegisterArmy(this);
    }

    public DataItemArmy (CustomArmy army) : this (army.spawnPos, army.ownership)
    {
        for (int i =0; i< army.formation.Length; i++)
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
        display?.OnGraphicsChange();
    }

    public ArmyFormation formation;
    public ArmyMovementComponent movement;
    public ArmyStatusComponent status;
    public ArmyOrders orders;
    #region Move
    public override void ChangeTile(Vector2Int t)
    {
        if (tile != null)
        {
            tile.armyLayer = null;
        }
        tile = SidewaysMap.main.GetTile(t);
        gridPos = t;
        tile.armyLayer = this;
    }
    public bool MoveToTile(Vector2Int t)
    {
        var ntile = SidewaysMap.main.GetTile(t);
        if (ntile!=null && ntile.armyLayer == null)
        {
            ChangeTile(t);
            return true;
        }
        return false;
    }
    #endregion
    public bool IsAlive()
    {
        return dead || formation.GetUnits().Length > 0;
    }
    public bool IsInCombat()
    {
        return Combat.main.attackers == this || Combat.main.defenders == this;
    }
    public override void SetPlayerOwner(DataItemPlayer player)
    {
        if (GetPlayerOwner()!= null)
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
        display?.OnPlayerOwnerChange();
    }

    public void ApplyEffect(ApplyEffects effect) { }
    public float GetCityBonuses(string Bonus)
    {
        if (tile.buildingLayer != null && tile.buildingLayer.GetAlignment(this) ==  PlayerDefines.Alignment.playerowned)
        {
            return 1;//TODO tile.buildingLayer.GetSightRange(Bonus);
        }
        return 0;
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();
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
    public bool CanMerge(bool forced)
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
        if (CanMerge(false) && other.CanMerge(false))
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

        if (!Instant && GameManager.main.playerManager.GetCurrentPlayer().IsAiControlled())
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
            GameManager.main.armyManager.SelectArmy(other) ;
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
    void UpdateAdjenctedLoS()
    {

    }
    #endregion
    public bool TryBattle(DataItemArmy other, bool canFlee)
    {
        if (other.IsAlive() && tile.IsNeighboring(other.tile) && GetAlignment(other) == PlayerDefines.Alignment.enemy)
        {

            //   if (other == OrderList[0].TargetUnit)
            //  {
            //      OrderList.RemoveAt(0);
            //  }
            return true;//TODO Actions.BattleArmies(game, this, other, false, canFlee);

        }
        return false;
    }
    public bool InvadeCastle(DataItemCastle castle, bool Instant)
    {

        if (!Instant && GameManager.main.playerManager.GetCurrentPlayer().IsAiControlled())
        {

           // game.game.InGameMenus.OpenWindow(new CastleInvadeWindow(game.game, this, Defender));
            return false;
        }

        return true;
    }
    #region Selection

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
        return (GetMyBribeCost() > 0 && Attacker.GetPowerValue(false) > GetPowerValue(false) );
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
        if (!CanMerge(false) || isMercenary() || GetPlayerOwner().isNeutral() )
        {
            return -1;
        }

        return Mathf.RoundToInt(GetPowerValue(false) * UnitDefines.fBribeMultiplier);
    }
    #endregion
    public override void Despawn()
    {
        if (!dead)
        {
            dead = true;
            base.Despawn();
            UpdateAdjenctedLoS();

            tile.armyLayer = null;
            if (IsSelected()) GameManager.main.armyManager.ClearSelectedArmy();

            GameManager.main.armyManager.ForgetArmy(this);
        }
    }

}
