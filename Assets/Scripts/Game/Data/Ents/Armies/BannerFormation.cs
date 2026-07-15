using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BannerFormation : BannerComponent
{

    public DataItemUnit[] Formation = new DataItemUnit[6];
    public DataItemUnit transport;

    public BannerFormation(DataItemBanner parent) : base(parent)
    {
    }

    public DataItemUnit[] GetUnits(bool incTransport = true, bool incDead = true)
    {
        List<DataItemUnit> units = new();
        units.AddRange(Formation);
        if (incTransport)
            units.Add(transport);
        units.RemoveAll(u => u == null || (!incDead && !u.IsAlive()));
        return units.ToArray();
    }
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        HandleEvent(AbilityDefines.Event.OnDayBegin);
    }
    public void HandleEvent(AbilityDefines.Event e)
    {
        foreach (var unit in GetUnits())
            unit.FireEventOnSelf(e);
    }
    public int CountLivingTroopsInRow(int row)
    {
        return Formation.Sum(u => u != null && u.troopPosition.y == row && u.damageable.IsAlive() ? 1 : 0);
    }
    public int CountLivingTroops()
    {
        return Formation.Sum(u => u != null && u.damageable.IsAlive() ? 1 : 0);
    }
    public int CountFightingTroops(CombatDefines.AttackPhase phase)
    {
        return Formation.Sum(u => u != null && u.damageable.IsAlive() && u.actions.actions.Any(a => a.CanBeCast(phase)) ? 1 : 0);
    }
    public DataItemUnit GetTroopInPosition(int x, int y)
    {
        if (x < 0 || y < 0) return transport;
        return Formation[Translate(x, y)];
    }
    int Translate(int x, int y)
    {
        return x * UnitDefines.iArmyRows + y;
    }
    public void GiveUnitInPosition(int x, int y, UnitData unit)
    {
        SetTroopInPosition(x, y, new DataItemUnit(unit, parent));
    }
    public void GiveUnitInPosition(int p, UnitData unit)
    {
        SetTroopInPosition(p, new DataItemUnit(unit, parent));
    }
    public void SetTroopInPosition(int x, int y, DataItemUnit unit)
    {
        if (x < 0 || y < 0)
            SetTroopInPosition(-1, unit);
        else
            SetTroopInPosition(Translate(x, y), unit);
    }
    public void SetTroopInPosition(int p, DataItemUnit unit)
    {
        if (unit == null)
        {
            if (p < 0)
                transport = null;
            else
                Formation[p] = null;
        }
        else
        {
            if (unit.troop != parent)
            {
                if (!CanIAccept(unit))
                {
                    return;
                }
                if (unit.troop != null)
                {
                    unit.troop?.formation.RemoveUnit(unit, false);
                }
                if (p < 0)
                    transport = unit;
                else
                    Formation[p] = unit;
                unit.troop = parent;
                unit.FireEventOnSelf(AbilityDefines.Event.OnMoveTile);
                parent.auras.OnUnitEnterFormation(unit);
            }
            else
            {
                MoveUnit(unit, p, false);
            }
        }
        OnFormationUpdate();
    }
    public bool TransferUnit(DataItemUnit unit)
    {
        if (parent.CanBeMerged(true) && CanIAccept(unit) )
        {
            if (unit.troop != null)
            {
                unit.troop.formation.RemoveUnit(unit, false);
            }


            for (int iX = 0; iX < UnitDefines.iArmyCols; iX++)
            {
                for (int iY = 0; iY < UnitDefines.iArmyRows; iY++)
                {
                    int rY = unit.IsRanged() ? (UnitDefines.iArmyCols - iY - 1) : iY;
                    if (IsEmptyAt(iX, rY))
                    {
                        SetTroopInPosition(iX, rY, unit);
                        return true;
                    }
                    else
                    {
                        var replaced = GetTroopInPosition(iX, rY);
                        if (!replaced.IsAlive())
                        {
                            DisposeCorpse(replaced);
                            SetTroopInPosition(iX, rY, unit);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public void RemoveUnit(DataItemUnit unit, bool refactor)
    {
        if (transport == unit)
            transport = null;
        else if (unit != null)
        {
            Vector2Int pos = GetPositionForUnit(unit);
            if (pos.x >= 0 && pos.y >= 0)
                Formation[Translate(pos.x, pos.y)] = null;
        }
        unit.troop.auras.OnUnitExitFormation(unit);
        unit.troop = null;
        if (refactor)
            OnFormationUpdate();

    }
    public void OnFormationUpdate()
    {
        if (CountLivingTroops() == 0)
        {
            parent.Despawn();
        }
        else
        {
            parent.movement.UpdateMaxMovement();
            parent.display?.OnGraphicsChange();
        }
    }
    public Vector2Int GetPositionForUnit(DataItemUnit unit)
    {
        for (int iX = 0; iX < UnitDefines.iArmyCols; iX++)
        {
            for (int iY = 0; iY < UnitDefines.iArmyRows; iY++)
            {
                if (GetTroopInPosition(iX, iY) == unit)
                {
                    return new Vector2Int(iX, iY);
                }
            }
        }
        return Vector2Int.one * -1;
    }
    public bool IsEmptyAt(int x, int y)
    {
        return GetTroopInPosition(x, y) == null;
    }

    #region Abilities
    public bool HasAbility(string abid)
    {
        return Formation.Any(u => u != null && u.innates.HasAbility(abid)) || (transport?.innates.HasAbility(abid) ?? false);
    }
    public int GetAbilitiyMax(string abid)
    {
        int total = 0;
        foreach (var troop in Formation)
        {
            total = Mathf.Max(troop?.innates.GetAbilityLevel(abid) ?? 0, total);
        }
        return Mathf.Max(transport?.innates.GetAbilityLevel(abid) ?? 0, total);

    }
    public int GetAbilitiySum(string abid)
    {
        int total = 0;
        foreach (var troop in Formation)
        {
            total += troop?.innates.GetAbilityLevel(abid) ?? 0;
        }
        total += transport?.innates.GetAbilityLevel(abid) ?? 0;
        return total;
    }
    #endregion
    #region Command and Accepting
    public bool CanIAccept(DataItemUnit target)
    {
        if (target == null  )
        {
            return true;
        }
        else if (target.GetAlignment(parent) != PlayerDefines.Alignment.playerowned || !parent.GetMainTile().IsPassible(target.GetMovetype()))
        {
            return false;
        }
        else if (target.isTransport())
        {
                return Formation.All(u => u.GetUnitSize() < transport.GetUnitSize()) && GetCommandValue()<= transport.innates.GetAbilityLevel("transport"); ;
        }
        else if (transport != null)
        {
            return target.GetUnitSize() < transport.GetUnitSize() && CanIAccept(target.GetCommandValue());
        }
        return CanIAccept(target.GetCommandValue());
    }
    public bool CanIAccept(UnitData target)
    {
        if ((transport == null || !transport.IsAlive()) && target.isTransport())
        {
            return true;
        }
        return CanIAccept(target.GetCommandValue());
    }
    public bool CanIAccept(int Value)
    {
        if (Value <= 0)
        {
            return transport == null;
        }
        return GetUnits(false,false).Count() < UnitDefines.iMaxTroopStack && GetCommandValue() + Value <= GetMaxCommand();
    }
    public int GetCommandValue()
    {
        int iData = 0;
        foreach (DataItemUnit aData in Formation)
        {
            iData += aData?.GetCommandValue() ?? 0;
        }
        return iData;
    }

    public int GetMaxCommand()
    {
        if (transport != null)
        {

            return transport.innates.GetAbilityLevel("transport");
        }
        return UnitDefines.iMaxTroopStack;
    }
    #endregion

    public Vector2Int getEmptySpot()
    {

        for (int iX = 0; iX < UnitDefines.iArmyRows; iX++)
        {
            for (int iY = 0; iY < UnitDefines.iArmyCols; iY++)
            {
                if (GetTroopInPosition(iY, iX) == null)
                {
                    return new Vector2Int(iX, iY);
                }
            }
        }
        return Vector2Int.one * -1;
    }
    /*public DataItemUnit MakeMeANewUnit(DataItemArmy Zim, List<float> Stats, string[] Abilities)
    {

        if (Zim != null)
        {

            DataItemUnit Stocking = new DataItemUnit( Zim, parent);

            if (Stats != null)
            {
                Stocking.ApplyStats(Stats, null);
            }
            if (Abilities != null)
            {
                List<string> ability = new List<string>();
                ability.AddRange(Abilities);
                Stocking.Abilities.Add(ability);
            }
            return Stocking;
        }
        return null;
    }*/

    public void MoveUnit(DataItemUnit uUnit, int aX, int aY, bool updateVisual)
    {
        MoveUnit(uUnit, Translate(aX, aY), updateVisual);
    }
    public void MoveUnit(DataItemUnit uUnit, int d, bool updateVisual)
    {
        if (uUnit.troop != parent) return;
        int o = -1;
        for (int i = 0; i < Formation.Length; i++)
        {
            if (Formation[i] == uUnit)
            {
                o = i;
                if (o == d) return;
                Formation[i] = null;
                break;
            }
        }
        Formation[o] = Formation[d];
        Formation[d] = uUnit;
    }

    public void SwapTroops(DataItemUnit a, DataItemUnit b, bool updateVisual)
    {
        int aY = a.troopPosition.x;
        int aX = a.troopPosition.y;
        int bY = b.troopPosition.x;
        int bX = b.troopPosition.y;

        ExchangeTroops(aX, aY, bX, bY, a.troop, b.troop, updateVisual);
    }

    public static void SwapTroops(int aX, int aY, int bX, int bY, DataItemBanner aTroop, bool updateVisual)
    {
        ExchangeTroops(aX, aY, bX, bY, aTroop, aTroop, updateVisual);
    }

    public static void ExchangeTroops(int aX, int aY, int bX, int bY, DataItemBanner aTroop, DataItemBanner bTroop, bool updateVisual)
    {
        if (aTroop == bTroop)
        {
            aTroop.formation.MoveUnit(aTroop.formation.GetTroopInPosition(aY, aX), bX, bY, updateVisual);
        }
        DataItemUnit aUnit = aTroop.formation.GetTroopInPosition(aY, aX);
        DataItemUnit bUnit = bTroop.formation.GetTroopInPosition(bY, bX);


        //Debug.Log ("Swap");
        if ((aUnit == null && !aTroop.formation.IsEmptyAt(aY, aX)) || (bUnit == null && !bTroop.formation.IsEmptyAt(bY, bX)))
        {
            Debug.Log("Not empty");
            return;
        }


        if ((aUnit == null || aUnit.isTransport()) && (bUnit == null || bUnit.isTransport()))
        {

            bTroop.formation.SetTroopInPosition(-1, -1, aUnit);
            aTroop.formation.SetTroopInPosition(-1, -1, bUnit);

        }
        else if (aTroop != bTroop)
        {

            if (!aTroop.CanBeMerged(false) || !bTroop.CanBeMerged(false))
            {
                Debug.Log("No open army");
                return;
            }

            if (aUnit == null)
            {
                if (!aTroop.formation.CanIAccept(bUnit))
                {
                    Debug.Log("no room unit B");

                    return;
                }
            }
            else if (bUnit == null)
            {
                if (!bTroop.formation.CanIAccept(aUnit))
                {
                    Debug.Log("no room unit A");
                    return;
                }
            }
            else
            {
                if (!aTroop.formation.CanIAccept(bUnit.GetCommandValue() - aUnit.GetCommandValue()) || !bTroop.formation.CanIAccept(aUnit.GetCommandValue() - bUnit.GetCommandValue()))
                {
                    Debug.Log("no room");
                    return;
                }
            }
        }

        bTroop.formation.SetTroopInPosition(bX, bY, aUnit);
        aTroop.formation.SetTroopInPosition(aX, aY, aUnit);

    }

   public void DisposeCorpse(DataItemUnit unit)
    {
        RemoveUnit(unit,false);
    }
}
