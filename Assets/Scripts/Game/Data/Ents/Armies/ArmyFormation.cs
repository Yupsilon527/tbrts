using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyFormation : ArmyComponent
{

    public DataItemUnit[] Formation = new DataItemUnit[6];
    public DataItemUnit transport;

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
        return Formation.Sum(u => u != null && u.damageable.IsAlive() && u.abilities.attacks.Any(a => a.CanBeCast(phase)) ? 1 : 0);
    }
    public DataItemUnit GetTroopInPosition(int x, int y)
    {
        return Formation[x + y * 3];
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
        else if (transport == null || target.isTransport())
        {
            if (!TerrainDefines.CanIWalkOver(target.GetMovetype(), parent.tile.elevation))
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
        foreach (DataItemUnit aData in getUnits(false))
        {
            if (!aData.IsDead(true))
            {
                iData += aData.GetCommand();
            }
        }
        return iData;
    }

    public int GetMaxCommand()
    {
        if (transporter != null)
        {

            return transporter.GetAbility("transport");
        }
        return Game.iMaxTroopStack;
    }
    #endregion

    public Vector2Int getEmptySpot()
    {

        for (int iX = 0; iX < 3; iX++)
        {
            for (int iY = 0; iY < 3; iY++)
            {
                if (getUnitAt(iY, iX) == null)
                {
                    return new Vector2Int(iX, iY);
                }
            }
        }
        return Vector2Int.one * -1;
    }
    public DataItemUnit MakeMeANewUnit(DataItemArmy Zim, List<float> Stats, string[] Abilities)
    {

        if (Zim != null)
        {

            DataItemUnit Stocking = DataItemUnit.MakeNewUnit(game, GetOwner(), Zim, this);

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
    }
    public bool TakeUnit(DataItemUnit Army, bool updateVisual)
    {
        if (CanIAccept(Army.GetCommand()) && canMerge(true))
        {

            if (Army.GetCommand() == 0)
            {
                if (Army.Troop != null)
                {
                    Army.Troop.transporter = null;
                    Army.Troop.reviseDisplay(true);
                }
            }
            else if (Army.Troop != null)
            {
                Army.Troop.Formation[Army.GetFormation().y, Army.GetFormation().x] = null;
                Army.Troop.reviseDisplay(true);
            }

            int[] row = new int[] { 0, 1, 2 };
            if (Army.BaseData.isRangedCreature())
            {
                row = new int[] { 1, 2, 0 };
            }

            for (int iY = 0; iY < 3; iY++)
            {
                for (int iX = 0; iX < 3; iX++)
                {
                    if (isEmptyAt(row[iY], iX))
                    {
                        TakeUnit(iX, row[iY], Army, updateVisual);
                        return true;
                    }
                }
            }
        }
        return false;


        public static void SwapTroops(DataItemUnit uUnit, int aX, int aY, bool updateVisual)
        {
            uUnit.Troop.SwapTroops(uUnit.Troop.getUnitAt(aY, aX), uUnit, updateVisual);
        }

        public void SwapTroops(DataItemUnit Panty, DataItemUnit Stocking, bool updateVisual)
        {
            int aY = Panty.GetFormation().x;
            int aX = Panty.GetFormation().y;
            int bY = Stocking.GetFormation().x;
            int bX = Stocking.GetFormation().y;

            SwapTroops(aX, aY, bX, bY, Panty.Troop, Stocking.Troop, updateVisual);
        }

        public static void SwapTroops(int aX, int aY, int bX, int bY, entityArmy aTroop, bool updateVisual)
        {
            SwapTroops(aX, aY, bX, bY, aTroop, aTroop, updateVisual);
        }

        public static void SwapTroops(int aX, int aY, int bX, int bY, entityArmy aTroop, entityArmy bTroop, bool updateVisual)
        {
            DataItemUnit aUnit = aTroop.getUnitAt(aY, aX);
            DataItemUnit bUnit = bTroop.getUnitAt(bY, bX);

            //Debug.Log ("Swap");
            if ((aUnit == null && !aTroop.isEmptyAt(aY, aX)) || (bUnit == null && !bTroop.isEmptyAt(bY, bX)))
            {
                Debug.Log("Not empty");
                return;
            }


            if ((aUnit == null || aUnit.isTransport()) && (bUnit == null || bUnit.isTransport()))
            {

                bTroop.TakeUnit(1, 3, aUnit, updateVisual);
                aTroop.TakeUnit(1, 3, bUnit, updateVisual);

            }
            else if (aTroop != bTroop)
            {

                if (!aTroop.canMerge(false) || !bTroop.canMerge(false))
                {
                    Debug.Log("No open army");
                    return;
                }

                if (aUnit == null)
                {
                    if (!aTroop.CanIAccept(bUnit))
                    {
                        Debug.Log("no room unit B");

                        return;
                    }
                }
                else if (bUnit == null)
                {
                    if (!bTroop.CanIAccept(aUnit))
                    {
                        Debug.Log("no room unit A");
                        return;
                    }
                }
                else
                {
                    if (!aTroop.CanIAccept(bUnit.GetCommand() - aUnit.GetCommand()) || !bTroop.CanIAccept(aUnit.GetCommand() - bUnit.GetCommand()))
                    {
                        Debug.Log("no room");
                        return;
                    }
                }
            }

            bTroop.TakeUnit(bX, bY, aUnit, updateVisual);
            aTroop.TakeUnit(aX, aY, bUnit, updateVisual);

        }

        public static DataItemUnit SpawnUnit(GameHubWorld game, DataItemArmy UnitName, entityPlayer Player, entityTile tTile, int iDur)
    {
        DataItemUnit Panty = SpawnUnit(game, UnitName, Player, tTile);
        if (iDur >= 0)
        {
            Panty.AddModifier(new entityModifier(entityModifier.Names.timedlife, 0, iDur, entityModifier.Behavior.world_modifier, entityModifier.Alignment.neutral, false), 0);
        }
        return Panty;
    }

    public static DataItemUnit SpawnUnit(GameHubWorld game, string UnitName, entityPlayer Player, entityTile tTile)
    {
        return SpawnUnit(game, game.game.LoadArmy(UnitName, false), Player, tTile);
    }
    public static DataItemUnit SpawnUnit(GameHubWorld game, DataItemArmy uData, entityPlayer Player, entityTile tTile)
    {
        return SpawnUnit(game, uData, Player, tTile, null);
    }

    public static DataItemUnit SpawnUnit(GameHubWorld game, DataItemArmy uData, entityPlayer Player, entityTile tTile, entityCastle myCastle)
    {
        if (tTile == null)
        {
            return null;
        }

        entityArmy Panty = tTile.ArmyLocated;
        if (Panty == null || !Panty.CanIAccept(uData.GetCommand()))
        {
            Panty = new entityArmy(game, tTile.Pos.x, tTile.Pos.y, Player.ID, true);
            //Panty.Exhaust();
        }

        DataItemUnit Zim = DataItemUnit.MakeNewUnit(game, Player, uData, Panty);

        if (Zim == null)
        {
            return null;
        }

        if (myCastle != null)
        {
            List<string> Bonuses = new List<string>();

            if (myCastle.GetBonus("attack") > 0)
            {
                Bonuses.Add("attack-" + myCastle.GetBonus("attack"));
            }
            if (myCastle.GetBonus("speed") > 0)
            {
                Bonuses.Add("speed-" + myCastle.GetBonus("speed"));
            }
            if (myCastle.GetBonus("hitpoints") > 0)
            {
                Bonuses.Add("hitpoints-" + myCastle.GetBonus("hitpoints"));
            }

            foreach (DataItemBuilding Gir in myCastle.Upgrades)
            {
                foreach (string data in Gir.BuildingData)
                {
                    string[] temp = Game.separateString(data);
                    if (temp[0] == "ability_min" || temp[0] == "ability_add" || temp[0] == "ability_improve" || temp[0] == "ability_learn")
                    {
                        Bonuses.Add(temp[0] + "-" + temp[1] + ", " + temp[2]);
                    }

                }
            }

            Zim.ApplyBonuses(Bonuses);
        }

        Panty.sanityCheck();
        return Zim;
    }

    public void TakeUnit(int iX, int iY, DataItemUnit unit, bool updateVisual)
    {
        if (!CanIAccept(unit))
        {
            return;
        }
        if (unit == null)
        {
            if (iY > 2)
            {
                transporter = null;
            }
            else
            {
                Formation[iY, iX] = null;
            }
        }
        else if (unit.isTransport())
        {
            transporter = unit;
            unit.Troop = this;
        }
        else
        {

            Formation[iY, iX] = unit;
            unit.Troop = this;

        }

        Movement = Mathf.Min(Movement, GetMyMovement());
        if (updateVisual)
        {
            UpdateAdjenctedLoS();
            reviseDisplay(true);
        }
    }

    public static void SwapTroops(DataItemUnit uUnit, int aX, int aY, bool updateVisual)
    {
        uUnit.Troop.SwapTroops(uUnit.Troop.getUnitAt(aY, aX), uUnit, updateVisual);
    }

    public void SwapTroops(DataItemUnit Panty, DataItemUnit Stocking, bool updateVisual)
    {
        int aY = Panty.GetFormation().x;
        int aX = Panty.GetFormation().y;
        int bY = Stocking.GetFormation().x;
        int bX = Stocking.GetFormation().y;

        SwapTroops(aX, aY, bX, bY, Panty.Troop, Stocking.Troop, updateVisual);
    }

    public static void SwapTroops(int aX, int aY, int bX, int bY, entityArmy aTroop, bool updateVisual)
    {
        SwapTroops(aX, aY, bX, bY, aTroop, aTroop, updateVisual);
    }

    public static void SwapTroops(int aX, int aY, int bX, int bY, entityArmy aTroop, entityArmy bTroop, bool updateVisual)
    {
        DataItemUnit aUnit = aTroop.getUnitAt(aY, aX);
        DataItemUnit bUnit = bTroop.getUnitAt(bY, bX);

        //Debug.Log ("Swap");
        if ((aUnit == null && !aTroop.isEmptyAt(aY, aX)) || (bUnit == null && !bTroop.isEmptyAt(bY, bX)))
        {
            Debug.Log("Not empty");
            return;
        }


        if ((aUnit == null || aUnit.isTransport()) && (bUnit == null || bUnit.isTransport()))
        {

            bTroop.TakeUnit(1, 3, aUnit, updateVisual);
            aTroop.TakeUnit(1, 3, bUnit, updateVisual);

        }
        else if (aTroop != bTroop)
        {

            if (!aTroop.canMerge(false) || !bTroop.canMerge(false))
            {
                Debug.Log("No open army");
                return;
            }

            if (aUnit == null)
            {
                if (!aTroop.CanIAccept(bUnit))
                {
                    Debug.Log("no room unit B");

                    return;
                }
            }
            else if (bUnit == null)
            {
                if (!bTroop.CanIAccept(aUnit))
                {
                    Debug.Log("no room unit A");
                    return;
                }
            }
            else
            {
                if (!aTroop.CanIAccept(bUnit.GetCommand() - aUnit.GetCommand()) || !bTroop.CanIAccept(aUnit.GetCommand() - bUnit.GetCommand()))
                {
                    Debug.Log("no room");
                    return;
                }
            }
        }

        bTroop.TakeUnit(bX, bY, aUnit, updateVisual);
        aTroop.TakeUnit(aX, aY, bUnit, updateVisual);

    }

    public void TakeUnit(int iX, int iY, DataItemUnit unit, bool updateVisual)
    {
        if (!CanIAccept(unit))
        {
            return;
        }
        if (unit == null)
        {
            if (iY > 2)
            {
                transporter = null;
            }
            else
            {
                Formation[iY, iX] = null;
            }
        }
        else if (unit.isTransport())
        {
            transporter = unit;
            unit.Troop = this;
        }
        else
        {

            Formation[iY, iX] = unit;
            unit.Troop = this;

        }

        Movement = Mathf.Min(Movement, GetMyMovement());
        if (updateVisual)
        {
            UpdateAdjenctedLoS();
            reviseDisplay(true);
        }
    }s

}
