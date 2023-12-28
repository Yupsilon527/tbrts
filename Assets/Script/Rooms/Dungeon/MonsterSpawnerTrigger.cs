using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerTrigger : MonoBehaviour
{
    public DungeonMonsterSpawnPoint[] LinkedSpawns;
    public bool spawnOnlce = false;
    bool HasSpawned = false;
    private void OnEnable()
    {
        HasSpawned = false;
    }
    public void SpawnWave()
    {
        if (!spawnOnlce || !HasSpawned)
        {
            foreach (DungeonMonsterSpawnPoint spawn in LinkedSpawns)
            {
                spawn.SpawnMonster();
            }
            HasSpawned = true;
        }
    }
}
