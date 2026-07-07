using System;
using System.Linq;
using UnityEngine;

public class DataItemUnit : DataItemMob
{
    public int nextAction = 0;
    public int initiative = 50;
    public int hitCounter = 1;
    public int critCounter = 1;
    public int dodgeCounter = 1;


    public override Vector2Int GetCoords()
    {
        return troop.GetCoords();
    }

    public override DataItemTile[] GetOccupiedTiles()
    {
        return troop.GetOccupiedTiles();
    }

    public override DataItemTile GetMainTile()
    {
        return troop.GetMainTile();
    }


    public UnitData data;
    public ResourceInt health=new(100,"True Helath",false,true);

    public DataItemArmy troop;
    public Vector2Int troopPosition => troop.formation.GetPositionForUnit(this);


    public UnitStats stats;
    public UnitDamageable damageable;
    public CombatantAbilities actions;
    public UnitBonuses bonuses;
    public CombatantModifiers modifiers;
    public AbilityComponentn innates;
    public UnitUpgrades upgrades;

    public override string ToString()
    {
        return $"Unit {data.InternalName} {eID} ({nextAction})";
    }
    public DataItemUnit(UnitData table)
    {
        data = table;
        stats = new(this, table.unit);
        
        damageable = new(this);

        modifiers = new(this);
        upgrades = new(this);

        actions = new(this);
        innates = new(this);

        bonuses = new(this);
    }

    public DataItemUnit( UnitData uData, DataItemArmy newArmy) :this(uData)
    {
        SetPlayerOwner(newArmy.GetPlayerOwner());
        newArmy.formation.TransferUnit(this);
    }
    #region events
    public void FireEventOnSelf(AbilityDefines.Event evtData, bool refresh = false)
    {
        HandleEvent(evtData, new DataItemUnit[0], refresh);
    }
    public void FireEventOnTarget(AbilityDefines.Event evtData, DataItemUnit target, bool refresh = false)
    {
        HandleEvent(evtData, new DataItemUnit[] { target }, refresh);
    }
    public virtual void HandleEvent(AbilityDefines.Event evt, DataItemUnit[] targets, bool refresh = false)
    {
        stats.TriggerFuncs(evt);
        damageable.TriggerFuncs(evt);
        actions.TriggerFuncs(evt);
        modifiers.EventReaction(evt, targets);
        if (evt == AbilityDefines.Event.CombatPhase)
        {
            UpdateNextAction();
        }
    }
    #endregion
    public void Act()
    {
        Act(Combat.main.currentTick);
    }
    public void Act(int currentTick)
    {
        actions.Tick(currentTick);
        modifiers.Tick(currentTick);
        UpdateNextAction();
    }
    public bool CanAct(CombatDefines.AttackPhase phase)
    {
        return damageable. IsAlive() && actions.GetAttacks().Any(a => a.original.attackPhase == phase && a.HasResourcesToCast());
    }
    protected void UpdateNextAction()
    {
        initiative = (int)(UnityEngine.Random.value * 25)   ;
        nextAction = Mathf.Min(actions.GetNextTick(), modifiers.GetNextTick());
        Combat.main.Inspect($"{this} next action is set to {nextAction}");
    }
    #region States
    public virtual void Refresh(bool force = false)
    {
        FireEventOnSelf(AbilityDefines.Event.OnRefresh);
        modifiers.Refresh(force);
    }
    public virtual bool GetState(ModifierDefines.State State)
    {
        return modifiers.GetState(State);
    }
    public float GetProperty(ModifierDefines.Property Property)
    {
        if (ModifierDefines.IsPropertyMultiplicative(Property))
            return modifiers.GetPropertyMultiplicative(Property);
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyAdditive(ModifierDefines.Property Property)
    {
        return modifiers.GetPropertyAdditive(Property);
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Property Property)
    {
        return modifiers.GetPropertyMultiplicative(Property);
    }
    #endregion
    public bool IsInCombat()
    {
        return troop.IsInCombat();
    }
    public int GetCommandValue()
    {
        return 1 + innates.GetAbilityLevel("command");
    }
    public int GetPowerValue(bool accountPenalty)
    {
        return 0;
    }
    public bool IsRanged()
    {
        return false;
    }

    public int GetMyMovement()
    {
        return Mathf.Max(1, Mathf.CeilToInt(UnitDefines.MoveBase
            + UnitDefines.MoveAddition * innates.GetAbilityCombined(UnitDefines.ArmyAbilities.haste)
            - UnitDefines.MoveSubstraction * innates.GetAbilityCombined(UnitDefines.ArmyAbilities.slow)));
    }
    public TerrainDefines.Movement GetMovetype()
    {

        if (innates.GetAbilityLevel("ghost") > 0)
        {
            return TerrainDefines.Movement.Ghost;
        }
        else   if (innates.GetAbilityLevel("ghost") > 0)
        {
            return TerrainDefines.Movement.Ghost;
        }
        else if (innates.GetAbilityLevel("fly") > 0)
        {
            return TerrainDefines.Movement.Fly;
        }
        else if (innates.GetAbilityLevel("teleport") > 0)
        {
            return TerrainDefines.Movement.Teleport;
        }
        else if (innates.GetAbilityLevel("wheels") > 0)
        {
            return TerrainDefines.Movement.GroundWheels;
        }
        else if (innates.GetAbilityLevel("giant") > 0)
        {
            return TerrainDefines.Movement.GroundMounted;
        }
        else if (innates.GetAbilityLevel("foot") > 0)
        {
            return TerrainDefines.Movement.GroundFoot;
        }
        else if (innates.GetAbilityLevel("amphibian") > 0)
        {
            return TerrainDefines.Movement.Amphibian;
        }
        else if (innates.GetAbilityLevel("swim") > 0)
        {
            return TerrainDefines.Movement.Swimmer;
        }
        else if (innates.GetAbilityLevel("seaworthy") > 0)
        {
            return TerrainDefines.Movement.Boat;
        }

        return TerrainDefines.Movement.Basic;
    }

    public ResourceCost[] GetPurchaseCost(DataItemPlayer Owner)
    {
        return data.GetCostForPlayer(Owner);
    }
    public float GetUpkeep()
    {
        return Mathf.Max(1, Mathf.CeilToInt(GetPowerValue(true) * UnitDefines.fSalaryMultiplier));
    }

    public bool isTransport()
    {
        return innates.GetAbilityLevel("transport") > 0;
    }

    public bool IsAlive()
    {
        return health.GetValue() > 0;
    }
    public virtual string OutputStatsTable()
    {
        string output = "";

        output += $"Combat: {stats.realStats.Offense}/{stats.realStats.Defense}<br>";

        string damage = stats.realStats.Attack == stats.baseStats.Attack  ? "" :  stats.realStats.Attack > stats.baseStats.Attack ? ("+" + (stats.realStats.Attack - stats.baseStats.Attack)) : ("" + (stats.realStats.Attack - stats.baseStats.Attack));
        output += $"Damage: {stats.baseStats.Attack}{damage}<br>";

        string magic = stats.realStats.Magic == stats.baseStats.Magic ? "" : stats.realStats.Magic > stats.baseStats.Magic ? ("+" + (stats.realStats.Magic - stats.baseStats.Magic)) : ("" + (stats.realStats.Magic - stats.baseStats.Magic));
        output += $"Magic: {stats.baseStats.Magic}{magic}<br>";

        string armor = stats.realStats.Armor == stats.baseStats.Armor ? "" : stats.realStats.Armor > stats.baseStats.Armor ? ("+" + (stats.realStats.Armor - stats.baseStats.Armor)) : ("" + (stats.realStats.Armor - stats.baseStats.Armor));
        string shield = stats.realStats.Shield == stats.baseStats.Shield ? "" : stats.realStats.Shield > stats.baseStats.Shield ? ("+" + (stats.realStats.Shield - stats.baseStats.Shield)) : ("" + (stats.realStats.Shield - stats.baseStats.Shield));
        string padding = stats.realStats.Padding == stats.baseStats.Padding ? "" : stats.realStats.Padding > stats.baseStats.Padding ? ("+" + (stats.realStats.Padding - stats.baseStats.Padding)) : ("" + (stats.realStats.Shield - stats.baseStats.Padding));

        output += $"Armor: {stats.baseStats.Armor}{armor}/{stats.baseStats.Shield}{shield}/{stats.baseStats.Padding}{padding}<br>";
        output += $"Magic Resist: {Mathf.Round(UnitDamageable.AccountResistances(100, stats.realStats.Resistance))}<br>";
        output += $"Action Points: {stats.realStats.Action}/Reaction Points: {stats.realStats.Mana}/Supply Points: {stats.realStats.Supply} <br>";

        output += $"Movement: {GetMyMovement()} ({GetMovetype()})";

        return output;
    }
    public virtual string OutputAbilityTable()
    {
        string output = "";

        var abs = actions.GetAttacks();
        var sps = actions.GetSpells();

        var ins = innates.abilities;
        var mds = modifiers.GetModifiers().OfType<PropertyInnate>().ToArray();

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
        if (ins.Count > 0 ||mds.Length > 0)
        {
            if (output.Length > 0)
            {
                output += "<br>";
            }
            output += "<b>Passives</b><br>";
            foreach (var ability in ins)
            {
                output += $"{ability.Key} {ability.Value}<br>";
            }
            foreach (var innate in mds)
            {
                output += $"{innate.InternalName}<br>";
            }
        }

        return output;
    }

}