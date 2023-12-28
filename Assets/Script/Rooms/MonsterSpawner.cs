using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public EnvironmentController parentRoom;
    public virtual bool IsActivelySpawning()
    {
        return false;
    }

    protected int currentEnemies = -1;
    protected void RecountCurrentEnemies()
    {
        currentEnemies = LevelController.main.monsterPool.activeObjs.childCount;
    }
    protected void ClearActiveEnemies()
    {
        foreach (Mob enemy in LevelController.main.monsterPool.GetComponentsInChildren<Mob>())
        {
            LevelController.main.monsterPool.DeactivateObject(enemy.gameObject);
        }
    }
    #region Spawn Enemies
    public virtual Mob SpawnEnemyFromPrefab(GameObject prefab, GridNav.Node position, bool playerOwned = false)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Trying to spawn entity from null data!");
            return null;
        }
        else if (position == null)
        {
            Debug.LogWarning("Trying to spawn entity on null position!");
            return null;
        }
        GameObject parent = LevelController.main.monsterPool.PoolItem(prefab);
        parent.name = prefab.name;

        Mob enemy = parent.GetComponent<Mob>();
        enemy.SetAlignment(playerOwned);
        enemy.transition.MovePlayerToNewRoom(parentRoom, position);
        enemy.Spawn();
        currentEnemies++;


        return enemy;
    }
    public void DespawnEnemy(GameObject enemy)
    {
        LevelController.main.monsterPool.DeactivateObject(enemy);
        currentEnemies--;
    }
    #endregion
}
