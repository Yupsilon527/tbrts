using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroComponent : MobComponent
{
     public Hero hero;
    protected override void Awake()
    {
        base.Awake();
        if (hero == null)
        {
            hero = GetComponent<Hero>();
        }
    }
    protected override bool SanityCheck()
    {
        return hero.gameObject.activeSelf && hero.damageable.isAlive();
    }
}
