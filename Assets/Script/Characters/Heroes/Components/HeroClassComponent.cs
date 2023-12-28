using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroClassComponent : HeroComponent
{
    public HeroStatTableSO AssignedClass;
    public void SetStats(HeroStatTableSO data)
    {
        if (data == null)
        {
            Debug.LogError("Error! Class for " + name + " is null!");
            return;
        }
        //if (AssignedClass!=null) TODO
          //  UnloadClass(AssignedClass);

        AssignedClass = data;
        hero.stats.LoadStatsTable(data);

        foreach (AbilitySO ab in data.Abilities)
        {
            hero.abilities.AddAbility(ab);
        }

        hero.stats.Recalculate();
    }
    public bool IsHealer()
    {
        return AssignedClass.HeroArchetype == MobDefines.Archetype.support;
    }
    public bool IsSniper()
    {
        return AssignedClass.HeroArchetype == MobDefines.Archetype.support;
    }
}
