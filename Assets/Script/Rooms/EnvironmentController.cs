using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentController : MonoBehaviour
{
    public string InternalName = "MISSING";
    [Header("Components")]
    public GridNav grid;
    public CameraBounds bounds;
    public MonsterSpawner spawner;
    public NavPoint ArrivalPoint;

    protected virtual void Awake()
    {
        if (grid == null)
            grid = GetComponentInChildren<GridNav>();
        if (bounds == null)
            bounds = GetComponentInChildren<CameraBounds>();
        if (spawner == null)
        {
            spawner = GetComponentInChildren<HubMonsterSpawner>();
        }
        InitNavPoints();

    }
    protected void InitNavPoints()


    {
        foreach (NavPoint navPoint in GetComponentsInChildren<NavPoint>())
        {
            navPoint.room = this;
        }

    }

    #region Entities
    public MobList<Mob> LocalMobs = new();
    public void OnMobEnterRoom(Mob mob)
    {
        LocalMobs
            .Add(mob);
    }
    public void OnMobExitRoom(Mob mob)
    { LocalMobs.Remove(mob); 
    }
    #endregion
}
