using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemPlayer 
{
    public int ID, Team;
    public Color color;

    public Resource[] resources;

    public string Name;
    public int AiLevel = -1;
    public bool Defeated = false;
    public int TurnDefeat = -1;
    public int playerTurn = -1;

    public DataFaction faction = new();
    public float[] baseIncome = new float[(int)EconomyDefines.IncomeResource.Total];
    public float[] realIncome = new float[(int)EconomyDefines.IncomeResource.Total];

    public UnitGroup<DataItemArmy> units = new();
    public UnitGroup<DataItemCastle> buildings = new();

    public DataItemPlayer(int iD, Color color)
    {
        Team = iD;
        ID = iD;
        this.color = color;
    }
    public DataItemPlayer(int iD, int team, Color color)
    {
        Team = team;
        ID = iD;
        this.color = color;
    }
    public override string ToString()
    {
        return $"Player {ID} (team {Team})";
    }
    public bool isNeutral()
    {
        return ID == 0;
    }
    public bool isPlayer()
    {
        return ID == 1;
    }
    public PlayerDefines.Alignment GetAlignment(DataItemPlayer Other)
    {
        if (Other == null || Other.isNeutral())
        {
            if (ID == 0)
            {
                return PlayerDefines.Alignment.playerowned;
            }
            return PlayerDefines.Alignment.enemy;
        }
        else if (Other.Team != Team)
        {
            return PlayerDefines.Alignment.enemy;
        }
        else if (Other.ID == ID)
        {
            return PlayerDefines.Alignment.playerowned;
        }
        else
        {
            return PlayerDefines.Alignment.ally;
        }
    }
    public void RegisterIncome(ResourceIncome[] income, float mult)
    {
        foreach (var e in income)
            IncreaseIncome(e, mult);
        if (income.Length > 0)
            ReviseRealIncome();
    }
    public void DeregisterIncome(ResourceIncome[] income, float mult)
    {
        foreach (var e in income)
            IncreaseIncome(e, 0 - mult);
    }
    #region Upgrades

    public class ResearchedUpgrade
    {
        public UpgradeData upgrade;
        public int level;
        public int maxes;

        public ResearchedUpgrade(UpgradeData upgrade) : this(upgrade, 1, 1)
        {
        }
        public ResearchedUpgrade(UpgradeData upgrade, int level, int maxes)
        {
            this.upgrade = upgrade;
            this.level = level;
            this.maxes = maxes;
        }

        public override string ToString()
        {
            return $"{upgrade.InternalName} level ({level}/{maxes})";
        }
    }
    [Header("Move to player")]
    public HashSet<ResearchedUpgrade> researchedUpgrades = new HashSet<ResearchedUpgrade>();
    ResearchedUpgrade FindUpgrade(UpgradeData upgrade) =>
    researchedUpgrades.FirstOrDefault(x => x.upgrade == upgrade);

    public int GetUpgradeLevel(UpgradeData upgrade) =>
        FindUpgrade(upgrade)?.level ?? 0;

    public bool HasReachedLimitForUpgrade(UpgradeData upgrade) =>
        !upgrade.infinite && FindUpgrade(upgrade) is ResearchedUpgrade u && u.level >= u.maxes;

    public bool UpgradeResearched(string name) =>
        researchedUpgrades.Any(x => x.level > 0 && x.upgrade.InternalName == name);
    public void SetUpgradeLevel(UpgradeData upgrade, int newLevel)
    {
        var existing = FindUpgrade(upgrade);
        int oldLevel = existing?.level ?? 0;

        if (!upgrade.infinite)
            newLevel = Mathf.Clamp(newLevel, 0, existing?.maxes ?? 1);

        if (newLevel == oldLevel) return;

        if (existing == null)
            researchedUpgrades.Add(existing = new ResearchedUpgrade(upgrade));

        existing.level = newLevel;
        upgrade.SetPlayerLevel(this, oldLevel, newLevel);
        ApplyUpgradeToAllUnits(upgrade, oldLevel, newLevel);

        if (existing.level <= 0)
            researchedUpgrades.Remove(existing);
    }

    public void ApplyBonus(UpgradeData bonus, int levels = 1)
    {
        if (FindUpgrade(bonus) is ResearchedUpgrade upgrade)
        {
            upgrade.level += levels;
            upgrade.maxes += levels;
        }
        else
        {
            researchedUpgrades.Add(new ResearchedUpgrade(bonus));
        }
    }

    public void RemoveBonus(UpgradeData bonus, int levels = 1)
    {
        if (FindUpgrade(bonus) is ResearchedUpgrade upgrade)
        {
            upgrade.level -= levels;
            upgrade.maxes -= levels;
        }
    }

    public void CompleteUpgrade(UpgradeData upgrade, int levels = 1) =>
        SetUpgradeLevel(upgrade, GetUpgradeLevel(upgrade) + levels);

    public void RemoveUpgrade(UpgradeData upgrade) =>
        SetUpgradeLevel(upgrade, 0);

    public void RevertUpgrade(UpgradeData upgrade, int levels = 1) =>
        SetUpgradeLevel(upgrade, GetUpgradeLevel(upgrade) - levels);
    void ApplyUpgradeToAllUnits(UpgradeData upgrade, int oldLevel, int newLevel)
    {
        foreach (var unit in units)
        {
      //   unit.upgrades.GrantUpgrade(upgrade, false, newLevel - oldLevel);
        }
    }
    public void ApplyResearchedUpgradeToNewlySpawnedUnit(DataItemUnit unit)
    {
        foreach (var upgrade in researchedUpgrades)
        {
        //   unit.upgrades.GrantUpgrade(upgrade.upgrade, true, upgrade.level);
        }
    }

    #endregion
    #region Resources
    public void InitResources()
    {
        resources = new[] {
            new ResourceInt(0, "Player Metal", false, false),
            new ResourceInt(0, "Player Gold", false, false),
            new ResourceInt(0, "Player Stone", false, false),
            new ResourceInt(0, "Player Tech", false, false),
            new ResourceInt(0, "Player Pop", false, true)
    };
    }
    public Resource GetResource(EconomyDefines.EconomyResource res)
    {
        if ((int)res < resources.Length)
            return resources[(int)res];
        return new ResourceInt(0, "temp", false, false);  //TODO
    }
    public float GetResourceValue(EconomyDefines.EconomyResource resource)
    {
        if ((int)resource < resources.Length)
            return GetResource(resource).GetValue();
        return 0;
    }
    public bool CanAffordResource(ResourceCost check, float mult = 1)
    {
        if (check.value <= 0 || check.resource == EconomyDefines.EconomyResource.Labor) return true;
        return GetResourceValue(check.resource) >= check.value * mult;
    }
    public bool CanAffordResources(ResourceCost[] resourceCosts, float mult = 1)
    {
        foreach (var cost in resourceCosts)
        {
            if (!CanAffordResource(cost, mult))
            {
                return false;
            }
        }
        return true;
    }
    public void GiveResources(ResourceCost[] granted, float mult = 1, bool roundstart = false)
    {
        foreach (var res in granted)
            GiveResource(res, mult, roundstart);
    }
    public void GiveResource(ResourceCost granted, float mult = 1, bool roundstart = false)
    {
        GiveResource(granted.resource, granted.value * mult, roundstart);
    }
    public void GiveResource(EconomyDefines.EconomyResource resource, float value, bool roundstart = false)
    {
        if ((int)resource < resources.Length)
            GetResource(resource).GiveValue(value);
    }
    public float Spend(ResourceCost spent)
    {

        if ((int)spent.resource < resources.Length)
            return GetResource(spent.resource).SubstractedValue(spent.value);
        return 0;
    }
    public void SpendResources(ResourceCost[] spent)
    {
        foreach (var res in spent)
        {
            Spend(res);
        }
    }
    public void RefundCosts(ResourceCost[] refund)
    {
        foreach (var r in refund)
            Refund(r);
    }
    public void Refund(ResourceCost refund)
    {
        GetResource(refund.resource).GiveValue(refund.value);
    }

    #endregion
    #region Income
    public void IncreaseIncome(ResourceIncome income, float mult)
    {
        IncreaseIncome(income.resource, income.value * mult);

    }
    public void IncreaseIncome(EconomyDefines.IncomeResource resource, float delta)
    {
        baseIncome[(int)resource] += delta;
    }
    public void DecreaseIncome(EconomyDefines.IncomeResource resource, float delta)
    {
        baseIncome[(int)resource] = Mathf.Max(0, baseIncome[(int)resource] - delta);
    }
    public void HandleIncome(float minute)
    {
        for (int i = 0; i < (int)EconomyDefines.EconomyResource.Total; i++)
        {
            GiveResource((EconomyDefines.EconomyResource)i, GetIncome((EconomyDefines.EconomyResource)i, minute));
        }
    }
    float GetIncome(EconomyDefines.EconomyResource resource, float minute)
    {
        return realIncome[(int)resource] * (1 + realIncome[(int)resource + (int)EconomyDefines.IncomeResource.RawIncome]) * minute;
    }
    public void ReviseRealIncome()
    {
      /*  if (LevelController.main.gameState != LevelController.GameState.playing) return;
        for (int i = 0; i < baseIncome.Length; i++)
        {
            realIncome[i] = baseIncome[i];
            foreach (var b in buildings)
            {
                if ((EconomyDefines.IncomeResource)i == EconomyDefines.IncomeResource.PopLimit)
                    realIncome[i] += b.buildingData.GetResourceIncome((EconomyDefines.IncomeResource)i);
                else
                    realIncome[i] += b.buildingData.GetResourceIncome((EconomyDefines.IncomeResource)i) * b.stats.realStats.HarvestRate;
            }
        }

    */
    }
    #endregion
    public bool IsAiControlled()
    {
        return false;
    }
}
