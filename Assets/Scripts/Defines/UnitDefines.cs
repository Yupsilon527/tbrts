

using System.Collections.Generic;

public static class UnitDefines
{
    public static int iArmyRows = 2;
    public static int iArmyCols = 3;
    public static int iMaxTroopStack = iArmyRows * iArmyCols;

    public static int MoveBase = 20;
    public static int MoveAddition = 4;
    public static int MoveSubstraction = 4;

    public static int iArmyBaseLoS = 4;
    public static float fBribeMultiplier = 2;
    public static float fSalaryMultiplier = 2;
	public enum TileVisibility
	{
		hidden,
		foggy,
		visible,
		truesight,
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

        seaworthy,
        swim,
        amphibian,
        foot,
        giant,
        fly,
        ghost,
        wheels,
        teleport,

		raze,
		raider,
		vandal,

		aura,

		rangeSupport,
		siegeSupport,
    }

    public static List<string> AbilityIndex = new List<string>{
		//----------- PLAIN STATS -----------
		//Abilities that offer stat value increase regardless of situation

		"type",//types and resistances
		"require",//needs castle level to produce
		"rfire","rnature","rstorm","rdeath","rholy","relements","rneutral","rmagic",
        "armored","armorpiercing","unarmed","shield",//armor
		"phase","mgImm",//immune
		"scouting",//sight
		//spies,truesight
		"fly","swim","amphibian","seaworthy","spaceworthy","foot",//movement
		"fast","slow",
        "banding",
        "plains_fighter","forest_fighter","hill_fighter","marsh_fighter","mountain_fighter","road_fighter",// terrain bonuses
		"siege","defend",//morale bonuses
		"fear","command","fearless",
        "rage",
        "antiair","antisea","antispace",
        "large",//sizes
		"slayer",

        "agility",//faster first strike
		"windUp",//slower first attack
		"charge", // attack bonus
		"battlecaster", // speed bonus casting

		//----------- TRAITS AND PASSIVES -----------
		//Abilities that offer a passive effect
		"cowardly",
        "marauder",//takes some spoils from castle raids
		//raider
		"mercenary",
        "kamikaze",//suicide
		//assassin
		//mindless
		//blood slave?
		//wood concealment
		//banding
		"peasant",//can turn itself into town population

		//warding
		"warding",//negates indirect damage
		"resolve",//prevents unit from taking too much damage

		//on hit
		"bash",//attacks ministun
		"rabid",//attacks apply plague
		"venom",//poisons on attack done
		"fireatk","stormatk","natureatk","holyatk","darkatk",//attacks apply elements
		//acid attack

		//heals
		"vampirism",//lifesteal
		"canibalize",//heals on kill
		"aid",//heals entire troop
		"regen",//regenerates hp
		"leech",//turns dead units into mana
		"blessed",//increased healing

		//zombie, zombie related
		"zombie",//zombie ability, cancels out all abilities
		"revive",//revives as zombie 
		"necromancy",//turn dead units into zombies, 1 ally, 2 enemy, 3 both
		"zombieatk","zombiehp",
		//zombieatk/hp aura
		"honorable",//morale on death

		//misc,transport
		"transport",
        "repairbay",//repairs units as transport

		//----------- SPELLS AND CASTING -----------
		//Abilities that allow a troop to cast things in battle

		"terror",
        "chaos",	//DONE
		"doom",		//DONE
		"archery",	//DONE
		"silence",	//DONE
		"heal",		//DONE
		"trueheal",
        "curse",	//DONE
		"bless",	//DONE
		"cure","cleanse",//DONE
		"ward",//DONE
		"paralysis",//slow, slows all enemy units //DONE
		"firecast","naturecast","stormcast","deathcast","holycast",//DONE

		"breathefire",//fire to the first row //DONE
		"swarm", //nature damage, equally distributed to all troops //DONE
		"chainlightning",//damage to the first column //DONE

		"viperstrike",//poisons target	//DONE
		"mercy",//heal over time		//DONE
		//beautiful
		//acid

		"plague",//DONE
		"battlecry",//done
		"skirmisher",//done
		"stun",

		//mental damage
		//mind decay
	};
}
