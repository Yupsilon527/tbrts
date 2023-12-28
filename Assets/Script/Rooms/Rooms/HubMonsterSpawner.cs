using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMonsterSpawner : MonsterSpawner
    {

    public NavPoint[] validSpawnPoints;

    [Header("Spawner Data")]
    public GameObject enemyPrefab;
    public float MonstersPerHour = 100;
    public float SpawnRange = 3;
    public int MaxEnemies = 10;

    public override bool IsActivelySpawning()
    {
        return (enemyPrefab != null && MaxEnemies > 0 && validSpawnPoints.Length > 0);
    }
    private void Awake()
    {
        if (!IsActivelySpawning())
        {
            gameObject.SetActive( false);
        }
    }

    #region Active Spawns
    private void Update()
    {
        ActiveSpawn();
    }
    float nextSpawnTime ;
    void ActiveSpawn()
    {
        if (nextSpawnTime < Time.time && currentEnemies< MaxEnemies)
        {
            SpawnSingle();
        }
    }
    #endregion
    #region Afk Spawns
    void OnDisable()
    {
        ClearActiveEnemies();
    }
    void OnEnable()
    {
        SpawnWave(MaxEnemies);

    }
    #endregion

    #region Spawn Enemies
    void SpawnWave(int amount)
    {
        if (!IsActivelySpawning()) return;
        for (int i = 0; i< amount; i++)
        {
            SpawnEnemyFromPrefab(enemyPrefab, FindNewSpotForEnemy(true));
        }
        nextSpawnTime = Time.time + GetSpawnInterval();
    }
    void SpawnSingle()
    {
        if (!IsActivelySpawning()) return;
        SpawnEnemyFromPrefab(enemyPrefab, FindNewSpotForEnemy(false));
        nextSpawnTime = Time.time + GetSpawnInterval();
    }
    float GetSpawnInterval()
    {
        return 3600 / MonstersPerHour;
    }
    #endregion
    public GridNav.Node FindNewSpotForEnemy(bool circular)
    {
        Vector2 center = Vector2.zero;
        if (validSpawnPoints.Length > 0)
        {
            Vector3? result = (validSpawnPoints[UnityEngine.Random.Range(0, validSpawnPoints.Length)]?.transform.position);
            if (result != null)
                center = (Vector2)result;
        }
        if (circular)
        {
            return parentRoom.grid.RandomNodeInCircle(center, SpawnRange);
        }
        else
        {
            if (parentRoom.grid.TryGetWorldNodeAt(center, out GridNav.Node result))
                return result;
        }
        return null;

    }
}
