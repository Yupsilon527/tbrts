using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class AttackData
{
    public AttackDefines.AttackType attack;
    public float BaseDamage = 0;
    public enum ScaleOff
    {
        attack = 0,
        special = 1,
        maxHealth =2,
        armor = 3,
        resistance = 4,
    }
    public ScaleOff ScaleType = ScaleOff.attack;
    public float ScaleDamage = 0;

    public AttackData() { }
    public AttackData(float damage, float ascale, ScaleOff hscale, AttackDefines.AttackType attack)
    {
        this.BaseDamage = damage;
        this.ScaleDamage = ascale;
        this.ScaleType = hscale;
        this.attack = attack;
    }
}