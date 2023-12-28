using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRegenerationComponent : MobComponent
{
    public float RegenInterval = .5f;
    float LastRegenTime = 0;
    void Update()
    {
        if (SanityCheck()) RegenAtTime(Time.time);
    }
    public void RegenAtTime(float time)
    {
        if (LastRegenTime < time + RegenInterval )
        {
            TryCatchUp(time - LastRegenTime);

            LastRegenTime = time;
        }
    }
    void RegenHealth(float deltaTime)
    {
        if (parent.damageable != null && parent.stats != null && parent.stats.RealHealthRegen>0 && parent.damageable.Health.GetPercentage() < 1)
        {
            parent.damageable.Health.GiveValue(parent.stats.RealHealthRegen * deltaTime);
        }
    }
    public void TryCatchUp(float timePassed)
    {
        RegenHealth(timePassed);
    }
    DateTime realTime;
    public void OnGameClose(DateTime now)
    {
        realTime = now;
    }
    public void OnGameOpen()
    {
        DateTime AfkEnd = DateTime.Now;
        TimeSpan deltaTime = AfkEnd - realTime;
        TryCatchUp((float)deltaTime.TotalSeconds);
    }

    float runTime;
    public void OnGamePause(float now)
    {
        runTime = now;
    }
    public void OnGameResume()
    {
        TryCatchUp(Time.realtimeSinceStartup - runTime);
    }

}
