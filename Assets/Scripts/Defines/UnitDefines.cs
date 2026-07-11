public static class UnitDefines
{
    public static int iArmyRows = 2;
    public static int iArmyCols = 3;
    public static int iMaxTroopStack = iArmyRows * iArmyCols;

    public static int MoveBase = 15;
    public static int MoveAddition = 3;
    public static int MoveSubstraction = 3;

    public static int iArmyBaseLoS = 4;
    public static float fBribeMultiplier = 2;
    public static float fSalaryMultiplier = 2;
	public enum TileVisibility
	{
		hidden = 0,
		foggy = 1,
		visible = 2,
		revealed_visible=3,
		truesight = 4,
		revealed_truesight =5,
	}
    public enum UpgradeCondition
    {
        permanent,
        temp,
        upgrade
    }
    public enum ArmyAbilities
    {
        mercenary,
        scouting,
        spies,
        transport,
        command,
		regen,
		haste,
		slow,
		
        building,
        seaworthy,
        swim,
        amphibian,
        foot,
        groundfast,
        groundheavy,
        fly,
        ghost,
        teleport,

		raze,
		raider,
		vandal,

		aura,

		phase,
		magicimmune,

		rangeSupport,
		siegeSupport,

        metalIncome,
        goldIncome,
		manaIncome,

		//spies can see inside cities
		//banding, reverse banding (depending on troops in enemy team)
		//loner

		//slayer, trample?
		resolve,//prevents unit from taking too much damage

        // vampirism,
        // cannibalize, heals on kill

		// petrify - unit dies below X hp
    }
}
