using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMonsterSpawnPointRandom : DungeonMonsterSpawnPoint
{
    public WeightList.WeightPrefab[] monsterPool;

    protected override GameObject GetMonsterPrefab()
    {
        return WeightList.PickWeight(monsterPool).prefab;
    }
}
