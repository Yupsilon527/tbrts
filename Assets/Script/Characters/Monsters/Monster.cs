using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Mob
{
    [Header("Monster Data")]
    public MonsterTableSO monsterData;
    public MonsterLootComponent monsterLoot;
    protected override void Awake()
    {
        base.Awake();
        if (monsterLoot == null)
        {
            monsterLoot = GetComponent<MonsterLootComponent>();
        }
        LoadCharacterData(monsterData);
    }
    void LoadCharacterData(MonsterTableSO data)
    {
        if (data == null)
        {
            Debug.LogError("Error! Data for " + name + " is null!");
            return;
        }
        monsterData = data;
        stats.LoadStatsTable(data);

        foreach (AbilitySO ab in data.Abilities)
        {
            abilities.AddAbility(ab,true);
        }
        stats.Recalculate();



    }

    public override void Spawn()
    {
        abilities.OnSpawn();
        attitude.GuardPoint = movement.GetNode();
        combatant.AutoAssignActiveAbility();
        base.Spawn();
    }
    public override void Despawn()
    {
        base.Despawn();
       /* if (monsterLoot != null)
        {
            monsterLoot.GrantRewards() ;
        }*/
        transition.CurrentRoom.spawner.DespawnEnemy(gameObject);
    }
}
