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
        return base.GetCostForPlayer(player, mult);
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
        else if (GetAbilityLevel("ghost") > 0)
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
        else if (GetAbilityLevel("giant") > 0)
        {
            return TerrainDefines.Movement.GroundVersatile;
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
}
