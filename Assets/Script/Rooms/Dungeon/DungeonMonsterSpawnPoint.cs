using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMonsterSpawnPoint : MonoBehaviour
{
    [Header("Config")]
    public DungeonComponent room;
    public bool AutoSpawn = true;
    public Transform walkPosition;
    [Header("Monster Data")]
    public AttitudeComponent.Attitude attitude = AttitudeComponent.Attitude.Defensive;
    public GameObject enemyPrefab;
    public bool Allied = false;
    public RewardTable rewardsTable;
    public void SpawnMonster()
    {
        if (room != null && enemyPrefab != null)
        {
            Mob mob = room.spawner.SpawnEnemyFromPrefab(GetMonsterPrefab(), room.grid.GetClosestToPoint(room.grid.TranslateCoordinate(transform.position)), Allied);
            if (mob.TryGetComponent(out MonsterLootComponent lootComponent))
            {
                lootComponent.lootTable = rewardsTable;
            }
            mob.attitude.SetAttitude(attitude);
            if (Allied)
            {
                PlayerController.main.heroMan.SpawnHero(mob);
            }
            if (walkPosition != null && walkPosition.gameObject.activeSelf)
            {
                mob.orders.GiveOrder(new OrdersComponent.MoveOrder(walkPosition.transform.position), 0);
            }
        }
    }
    protected virtual GameObject GetMonsterPrefab()
    {
        return enemyPrefab;
    }
}
