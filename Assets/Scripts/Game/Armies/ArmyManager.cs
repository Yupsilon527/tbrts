using System.Collections.Generic;

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
        return SpawnUnit(WorldManager.main.GetUnit(UnitName), Player, tTile);
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

        DataItemUnit Zim = DataItemUnit.MakeNewUnit(game, Player, uData, newArmy);

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

        newArmy.sanityCheck();
        return Zim;
    }
}

public class EntityManager : GameComponent
{
    public virtual void HandleEndOfTurn()
    {

    }
}