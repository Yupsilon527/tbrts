using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardTable
{
    [Header("Mana")]
    public float manaRewardChance = 0;
    public bool RewardsMana()
    {
        return Random.value * 100f <= manaRewardChance;
    }

    [Header("Gold")]
    public float minGoldDrop = 5;
    public float maxGoldDrop = 10;
    public float GetAverageGold()
    {
        return (minGoldDrop + maxGoldDrop) / 2;
    }
    public float GetRandomGold()
    {
        if (minGoldDrop == maxGoldDrop)
        {
            return minGoldDrop;
        }
        else
        {
            return (maxGoldDrop - minGoldDrop) * Random.value + minGoldDrop;
        }
    }
    [Header("Vars")]
    public List<Variables.Change> Variables = new();
}
