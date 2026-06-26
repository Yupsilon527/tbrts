using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitData : ProductionData
{
    public UnitStatsTable unit;
    public Sprite[] armySprites = new Sprite[6];
    public AttackDefines.MobFlag[] unitFlags;
    public WeaponData[] weapons;
    public SpellData[] spells;
    public AbilityData[] abilities;
    public override AvailableState GetAvailableState(DataItemPlayer player,DataItemCastle castle)
    {
        var avs = base.GetAvailableState(player, castle);
        if (avs == AvailableState.available && !player.econ.CanAffordResources(GetCostForPlayer(player)))
        {
            return AvailableState.greyedout;
        }
        return avs;
    }
    public void LoadCharacter(CharacterSO character)
    {
        if (character == null) return;
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
    public override void CompleteProduction(ProductionTable table)
    {
        // PlayerController.main.troopMan.SpawnBannerAtPoint(this, table.point, table.playerOwner);
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
            return TerrainDefines.Movement.Wheels;
        }
        else if (GetAbilityLevel("giant") > 0)
        {
            return TerrainDefines.Movement.GroundGiant;
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

        return TerrainDefines.Movement.Ground;
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
