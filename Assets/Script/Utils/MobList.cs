using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobList <mobType> :List<mobType> where mobType:Mob
{
    public mobType[] Filter(bool includePlayer = true, bool includeEnemies = true, EnvironmentController checkRoom = null)
    {
        List<mobType> filtered = new List<mobType>();
        foreach (Mob mob in this)
        {
            if( (includePlayer && mob.IsPlayerControlled()) || (includeEnemies && !mob.IsPlayerControlled()))
            {
            if ((checkRoom == null || mob.transition.CurrentRoom == checkRoom) && mob is mobType myMob)
                {
                filtered.Add(myMob);
            }

            }
        }
        return filtered.ToArray();
    }
    public mobType[] FindEntitiesInCircle(Vector2 center, float radius, bool includePlayer = true, bool includeEnemies = true, EnvironmentController checkRoom = null)
    {
        float sqrRadius = radius * radius;

        List<mobType> found = new List<mobType>();
        foreach (Mob mob in Filter(includePlayer,includeEnemies, checkRoom))
        {
            float dist = (((Vector2)mob.transform.position) - center).sqrMagnitude;
            if (dist  <= sqrRadius && mob is mobType filterMob)
            {
                found.Add(filterMob);
            }
        }
        return found.ToArray();
    }

}
