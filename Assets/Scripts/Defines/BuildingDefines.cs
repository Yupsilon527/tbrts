using UnityEngine;

public static class BuildingDefines 
{
    public static int iCastleProductionMax = 4;

    public enum CastleRazeMode
    {
        raid,
        occupy,
        sack,
        raze
    }

    public enum BuildingAbilities
    {


    }
    public enum RuinType
    {
        Metal,
        Gold,
        Mana,
    }
    public static int RuinMetalBase = 1000;
    public static int RuinGoldBase = 500;
    public static int RuinManaBase = 10;
    public static int RuinRefreshTurns = 3;
}
