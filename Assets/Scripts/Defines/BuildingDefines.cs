
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

    public enum RuinType
    {
        Metal,
        Gold,
        Mana,
        Rest,
        Heal,
        Pray,
    }
    public static int RuinMetalBase = 1000;
    public static int RuinMetalLevel = 100;
    public static int RuinGoldBase = 500;
    public static int RuinGoldLevel = 50;
    public static int RuinManaBase = 10;
    public static int RuinManaLevel = 2;
    public static float RuinHealBase = .2f;
    public static float RuinHealLevel = .05f;
    public static int RuinRefreshTurns = 3;
}
