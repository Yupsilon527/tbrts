using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightList 
{
    [System.Serializable]
    public class WeightEntry
    {
        public float Weight = 1; 

        public WeightEntry(float weight)
        {
            Weight = Mathf.Max(1,weight);
        }
    }
    [System.Serializable]
    public class WeightPrefab : WeightEntry
    {
        public GameObject prefab;
        public WeightPrefab(GameObject enemyPrefab, float weight) : base(weight)
        {
            prefab = enemyPrefab;
        }
    }
        public static witem PickWeight<witem>(witem[] value) where witem: WeightEntry
	{
		float total = 0;
		foreach (witem item in value)
        {
			total += item.Weight;
        }
        total = Random.Range(0, total);

        foreach (witem item in value)
        {
            total -= item.Weight;
            if (total <= 0)
                return item;
        }
        return null;
	}
}
