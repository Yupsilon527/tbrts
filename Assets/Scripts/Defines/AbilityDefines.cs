
using System;
using System.Collections.Generic;

public static class AbilityDefines
{
    public enum Event
    {
        Nothing = -1,
        Time = 29,

        //functional
        OnCreated = 0,
        OnDestroyed = 1,
        OnExpired = 2,
        OnRefresh = 3,
        OnStacksChange = 4,
        OnThink = 25,

        //combat
        OnSpawn = 5,
        CombatBegin = 6,
        CombatEnd = 7,

		//board
        OnTurnEnd = 24,
        OnMoveTile = 23,

		//aura
		OnUnitEnterStack = 24,
		OnUnitExitStack = 25,

        OnScoreKill = 8,
        OnKilled = 9,

        BeforeAttack = 10,
        AttackHit = 11,

        OnHitByEnemy = 13,
        CastSpell = 12,
        OnHitSpell = 31,
        OnHitBySpell = 32,

        OnTakeDamage = 14,
        OnTakeLifeDamage = 15,
        OnLifeChange = 30,

        OnHealRecieved = 16,
        OnShieldRecieved = 17,
        OnArmorRecieved = 18,

        OnShieldBlock = 19,
        OnShieldBreak = 20,

        OnArmorBlock = 21,
        OnArmorBreak = 22,

        Total = 33
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

public delegate void AbilityFunction(EventTable CastData);
    public class AbilityListener
    {
        public Event aEvent;
        public AbilityFunction aFunction;

        public AbilityListener(AbilityEvent evt, DataItemUnit activator)
        {
            aEvent = evt.Event;
            aFunction = (EventTable castData) =>
            {
                foreach (var effect in evt.defaultEffects)
                {
                    effect?.ActivateOnUnit(castData);
                }
            };
        }
        public AbilityListener(Event aEvent, AbilityFunction aFunction)
        {
            this.aEvent = aEvent;
            this.aFunction = aFunction;
        }

		public static int GetArmyBaseLoS = 4;
    }
    [System.Serializable]
    public class AbilityEvent
    {
        public Event Event;
        public ApplyEffects[] defaultEffects = new ApplyEffects[0];
    }
}
