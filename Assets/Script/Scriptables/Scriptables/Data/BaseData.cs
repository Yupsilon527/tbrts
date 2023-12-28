using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData : ScriptableObject
{
    public string id = "";
    public float Attack, Range, AttackSpeed, Health, Armor, Shield, MoveSpeed, SightRange, Cost = 0;
    public string[] flags = new  string[0];
}
