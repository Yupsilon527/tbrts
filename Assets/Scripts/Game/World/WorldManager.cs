using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Initializable
{
    HashSet<UnitData> units=new();
    public static WorldManager main;
    protected override void Initialize()
    {
        base.Initialize();
        main = this;
        LoadFromUnityData();
    }
    void LoadFromUnityData()
    {
        foreach (var u in Resources.LoadAll<UnitSO>(""))
        {
            units.Add(u.unit);
        }
    }
}
