using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeList
{
    #region Upgrades

    [Header("Move to player")]
    public HashSet<ResearchedUpgrade> researchedUpgrades = new HashSet<ResearchedUpgrade>();
    public Action<TechData , int ,  int > onUpgradeLevelChange;

    ResearchedUpgrade FindUpgrade(TechData upgrade) =>
    researchedUpgrades.FirstOrDefault(x => x.upgrade == upgrade);

    public int GetUpgradeLevel(TechData upgrade) =>
        FindUpgrade(upgrade)?.level ?? 0;

    public bool HasReachedLimitForUpgrade(TechData upgrade) =>
        (upgrade.infinite == TechData.UpgradeType.unique && GetUpgradeLevel(upgrade) == 1)
        || (upgrade.infinite == TechData.UpgradeType.stacking && FindUpgrade(upgrade) is ResearchedUpgrade u && u.level >= u.maxes);

    public void CompleteUpgrade(TechData upgrade, int levels = 1) =>
        SetUpgradeLevel(upgrade, GetUpgradeLevel(upgrade) + levels);
    public void CatchUpUpgrade(TechData upgrade, int levels = 1)
    {
        if (GetUpgradeLevel(upgrade) < levels)
            SetUpgradeLevel(upgrade, GetUpgradeLevel(upgrade) + levels);
    }
    public bool UpgradeResearched(string name) =>
        researchedUpgrades.Any(x => x.level > 0 && x.upgrade.InternalName.ToLower() == name.ToLower());


    public void SetUpgradeLevel(TechData upgrade, int newLevel)
    {
        var existing = FindUpgrade(upgrade);
        int oldLevel = existing?.level ?? 0;

        if (upgrade.infinite == TechData.UpgradeType.unique)
            newLevel = Mathf.Clamp(newLevel, 0, 1);
        else if (upgrade.infinite == TechData.UpgradeType.stacking)
            newLevel = Mathf.Clamp(newLevel, 0, existing?.maxes ?? 1);

        if (newLevel == oldLevel) return;

        if (existing == null)
        {
            existing = new ResearchedUpgrade(upgrade);
            researchedUpgrades.Add(existing);
        }

        onUpgradeLevelChange.Invoke(upgrade, newLevel, oldLevel);
        existing.level = newLevel;

        if (existing.level <= 0)
            researchedUpgrades.Remove(existing);
    }

    public void ApplyBonus(TechData bonus, int levels = 1)
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

    public void RemoveBonus(TechData bonus, int levels = 1)
    {
        if (FindUpgrade(bonus) is ResearchedUpgrade upgrade)
        {
            upgrade.level -= levels;
            upgrade.maxes -= levels;
        }
    }

    public void RemoveUpgrade(TechData upgrade) =>
        SetUpgradeLevel(upgrade, 0);

    public void RevertUpgrade(TechData upgrade, int levels = 1) =>
        SetUpgradeLevel(upgrade, GetUpgradeLevel(upgrade) - levels);

    #endregion

    public void Clear()
    {
        foreach (var upgrade in researchedUpgrades)
            RemoveUpgrade(upgrade.upgrade);
    }
}
