using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonComponent : EnvironmentController
{

    DungeonMonsterSpawnPoint[] monsterSpawns;
    protected override void Awake()
    {
        monsterSpawns = GetComponentsInChildren<DungeonMonsterSpawnPoint>();
        foreach (DungeonMonsterSpawnPoint spawnPoint in monsterSpawns)
        {
            spawnPoint.room = this;
        }
        base.Awake();
    }
    #region Dungeon Specific
    public void BeginDungeon()
    {
        foreach (DungeonMonsterSpawnPoint spawnPoint in monsterSpawns)
        {
            if (spawnPoint.AutoSpawn)
                spawnPoint.SpawnMonster();
        }
    }
    #endregion
}
