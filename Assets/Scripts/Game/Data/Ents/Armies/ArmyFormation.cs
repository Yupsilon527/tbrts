using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyFormation : ArmyComponent
{

    public DataItemUnit[] Formation = new DataItemUnit[6];
    public DataItemUnit transport;

    public ArmyFormation(DataItemArmy parent) : base(parent)
    {
    }

    public DataItemUnit[] GetUnits()
    {
        List<DataItemUnit> units = new();
        units.AddRange(Formation);
        units.Add(transport);
        units.RemoveAll(u => u == null);
        return units.ToArray();
    }
    public int CountLivingTroops()
    {
        return Formation.Sum(u => u != null && u.damageable.IsAlive() ? 1 : 0);
    }
    public int CountFightingTroops(CombatDefines.AttackPhase phase)
    {
        return Formation.Sum(u => u != null && u.damageable.IsAlive() && u.abilities.abilities.Any(a => a.CanBeCast(phase)) ? 1 : 0);
    }
    public DataItemUnit GetTroopInPosition(int x, int y)
    {
        return Formation[x + y * UnitDefines.iArmyCols];
    }
    public void GiveUnitInPosition(int x, int y, UnitData unit)
    {
        SetTroopInPosition(x, y, new DataItemUnit(unit, parent));
    }
    public void GiveUnitInPosition(int p, UnitData unit)
    {
        SetTroopInPosition( p, new DataItemUnit(unit, parent));
    }
    public void SetTroopInPosition(int x, int y, DataItemUnit unit)
    {
        if (x < 0 || y < 0)
            SetTroopInPosition(-1, unit);
        else
            SetTroopInPosition(x + y * UnitDefines.iArmyCols, unit);
    }
    public void SetTroopInPosition(int p, DataItemUnit unit)
    {
        if (!CanIAccept(unit))
        {
            return;
        }
        if (unit.troop != null)
        {
            unit.troop?.formation.RemoveTroop(unit);
        }
        if (p<0)
            transport = unit;
        else 
            Formation[p] = unit;
        OnFormationUpdate();
    }
    public bool TransferUnit(DataItemUnit unit)
    {
        if (CanIAccept(unit.GetCommandValue()) && parent.CanMerge(true))
        {
            if (unit.troop != null)
                {
                    unit.troop.formation.RemoveTroop(transport);
                }


            for (int iX = 0; iX < UnitDefines.iArmyRows; iX++)
            {
                for (int iY = 0; iY < UnitDefines.iArmyCols; iY++)
                {
                    int rY = unit.IsRanged() ? (UnitDefines.iArmyCols - iY - 1) : iY;
                    if (IsEmptyAt (iX,rY))
                    {
                        SetTroopInPosition(iX, rY, unit);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void RemoveTroop(DataItemUnit unit)
    {
        if (transport == unit)
            transport = null;
        else
        {
            Vector2Int pos = GetPositionForUnit(unit);
            Formation[pos.x + pos.y * UnitDefines.iArmyCols] = null;
        }
        OnFormationUpdate();

    }
    void OnFormationUpdate()
    {
        parent.movement.UpdateMaxMovement();
        parent.display?.OnGraphicsChange();
    }
    public Vector2Int GetPositionForUnit(DataItemUnit unit)
    {
        for (int iX = 0; iX < UnitDefines.iArmyRows; iX++)
        {
            for (int iY = 0; iY < UnitDefines.iArmyCols; iY++)
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
        if (target == null)
        {
            return true;
        }
        else if (transport == null && target.isTransport())
        {
            if (!TerrainDefines.CanIWalkOver(target.GetMovetype(), parent.tile.GetWalkElevation()))
            {
                return false;
            }
        }
        return CanIAccept(target.GetCommandValue());
    }
    public bool CanIAccept(int Value)
    {
        if (Value <= 0)
        {
            return transport == null;
        }
        return GetUnits().Count() < UnitDefines.iMaxTroopStack && GetCommandValue() + Value <= GetMaxCommand();
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

    public static void SwapTroops(DataItemUnit uUnit, int aX, int aY, bool updateVisual)
    {
        uUnit.Troop.SwapTroops(uUnit.Troop.GetTroopInPosition(aY, aX), uUnit, updateVisual);
    }

    public void SwapTroops(DataItemUnit a, DataItemUnit b, bool updateVisual)
    {
        int aY = a.GetFormation().x;
        int aX = a.GetFormation().y;
        int bY = b.GetFormation().x;
        int bX = b.GetFormation().y;

        ExchangeTroops(aX, aY, bX, bY, a.troop, b.troop, updateVisual);
    }

    public static void SwapTroops(int aX, int aY, int bX, int bY, DataItemArmy aTroop, bool updateVisual)
    {
        ExchangeTroops(aX, aY, bX, bY, aTroop, aTroop, updateVisual);
    }

    public static void ExchangeTroops(int aX, int aY, int bX, int bY, DataItemArmy aTroop, DataItemArmy bTroop, bool updateVisual)
    {
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

            if (!aTroop.CanMerge(false) || !bTroop.CanMerge(false))
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


}
