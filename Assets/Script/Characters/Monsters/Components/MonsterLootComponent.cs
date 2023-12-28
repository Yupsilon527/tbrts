using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLootComponent : MobComponent
{
    public RewardTable lootTable;
    public void GrantRewards()
    {
        RewardGoldFromTable();
        RewardManaFromTable();
        UpdateVariablesFromTable();
    }
    void RewardGoldFromTable()
    {
        PlayerController.main.resources.gold.GiveValue(lootTable.GetRandomGold());
    }
    void RewardManaFromTable()
    {
        if (lootTable.RewardsMana())
        {
            PlayerController.main.resources.IncreaseMana();
        }
    }
    void UpdateVariablesFromTable()
    {
        foreach (Variables.Change varchange in lootTable.Variables)
        {
            PlayerController.main.GetGlobalScope().Apply(varchange);
        }
    }
}
