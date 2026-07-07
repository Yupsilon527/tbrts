using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitData : ProductionData
{
    public UnitStatsTable unit;
    public Sprite[] armySprites = new Sprite[6];
    public AttackDefines.MobFlag[] unitFlags;
    public WeaponData[] attacks = new WeaponData[0];
    public SpellData[] spells = new SpellData[0];
    public AbilityData[] abilities = new AbilityData[0];
    public InnateData[] innates= new InnateData[0];
    public override AvailableState GetAvailableState(DataItemPlayer player, DataItemCastle castle)
    {
        var avs = base.GetAvailableState(player, castle);
        if (avs == AvailableState.available && !player.econ.CanAffordResources(GetCostForPlayer(player)))
        {
            return AvailableState.unavailable;
        }
        return avs;
    }
    public void LoadCharacter(CharacterSO character)
    {
        if (character == null) return;
        if (character.sprites.Length ==1)
        {
            for (int i = 0; i< armySprites.Length; i++)
            {
                armySprites[i] = character.sprites[0].sprite;
            }
        }
        foreach (var sprite in character.sprites)
        {
            armySprites[(int)sprite.frame] = sprite.sprite;
        }
    }
    public Sprite GetSprite(CharacterSO.SpriteFrame id)
    {
        return GetSprite((int)id);
    }
    public Sprite GetSprite(int id)
    {
        return armySprites[id];
    }
    public override bool CompleteProduction(ProductionTable table)
    {
        foreach (var troop in table.castle.GetGarrison())
        {
            if (troop == null) continue;
            if (troop.GetAlignment(table.castle) == PlayerDefines.Alignment.playerowned && troop.formation.CanIAccept(this))
            {
                ArmyManager.SpawnUnit(this, troop, table.castle);
                return true;
            }
        }
        if (table.castle.production.GetValidTileForArmy(this) is DataItemTile tile)
        {
            if ( ArmyManager.SpawnUnitInCastle(this, table.playerOwner, tile, table.castle) != null)
                return true;
        }
        return false;

    }
    public override ResourceCost[] GetCostForPlayer(DataItemPlayer player, float mult = 1)
    {
       var costs= base.GetCostForPlayer(player, mult);
        if (GameManager.main.currentTurn == 0)   //HACK instant free on turn 0 TODO setting
            costs[(int)EconomyDefines.EconomyResource.Labor].value = 0;
        return costs;
    }
    public int GetCommandValue()
    {
        return 1 + GetAbilityLevel("command");
    }
    public bool isTransport()
    {
        return GetAbilityLevel("transport") > 0;
    }
    public TerrainDefines.Movement GetMovetype()
    {
        if (GetAbilityLevel("ghost") > 0)
        {
            return TerrainDefines.Movement.Ghost;
        }
        else if (GetAbilityLevel("fly") > 0)
        {
            return TerrainDefines.Movement.Fly;
        }
        else if (GetAbilityLevel("teleport") > 0)
        {
            return TerrainDefines.Movement.Teleport;
        }
        else if (GetAbilityLevel("wheels") > 0)
        {
            return TerrainDefines.Movement.GroundWheels;
        }
        else if (GetAbilityLevel("mounted") > 0)
        {
            return TerrainDefines.Movement.GroundMounted;
        }
        else if (GetAbilityLevel("foot") > 0)
        {
            return TerrainDefines.Movement.GroundFoot;
        }
        else if (GetAbilityLevel("amphibian") > 0)
        {
            return TerrainDefines.Movement.Amphibian;
        }
        else if (GetAbilityLevel("swim") > 0)
        {
            return TerrainDefines.Movement.Swimmer;
        }
        else if (GetAbilityLevel("seaworthy") > 0)
        {
            return TerrainDefines.Movement.Boat;
        }
        else if (GetAbilityLevel("building") > 0)
        {
            return TerrainDefines.Movement.Building;
        }

        return TerrainDefines.Movement.Basic;
    }
    public bool HasAbility(UnitDefines.ArmyAbilities ability)
    {
        return HasAbility(ability.ToString());
    }
    public bool HasAbility(string abilityID)
    {
        return abilities.Any(a => a.abilityID == abilityID);
    }
    public int GetAbilityLevel(string abilityID)
    {
        return abilities.Sum(a => a.abilityID == abilityID ? a.abilityLevel : 0);
    }
    public string OutputStatsTable()
    {
        string output = "";
        output += $"Combat: {unit.Offense}/{unit.Defense}<br>";

        output += $"Damage: {unit.Attack}<br>";

        output += $"Magic: {unit.Magic}<br>";

        output += $"Health: {unit.Health}<br>";
        output += $"Armor: {unit.Armor}/{unit.Shield}/{unit.Padding}<br>";
        output += $"Magic Resist: {Mathf.Round(100-UnitDamageable.AccountResistances(100, unit.Resistance))}<br>";
        output += $"Action Points: {unit.Action}/Reaction Points: {unit.Mana}/Supply Points: {unit.Supply} <br>";

        return output;
    }
    public string OutputAbilityTable()
    {
        string output = "";


        var abs = attacks;
        var sps = spells;

        var ins = abilities;
        var mds = innates;

        if (abs.Length > 0)
        {
            output += "<b>Abilities</b><br>";
            foreach (var a in abs)
            {
                output += a.InternalName + "<br>";
            }
        }
        if (sps.Length > 0)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += "<b>Spells</b><br>";
            foreach (var a in sps)
            {
                output = a.InternalName + "<br>";
            }
        }
        if (ins.Length > 0 || mds.Length > 0)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += "<b>Passives</b><br>";
            foreach (var ability in ins)
            {
                output += $"{ability.abilityID} {ability.abilityLevel}<br>";
            }
            foreach (var innate in mds)
            {
                output += $"{innate.InternalName}<br>";
            }
        }

        return output;
    }
}
