using System.Collections.Generic;
using System.Linq;
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
    public UnitData GetUnit(string name)
    {
        return units.FirstOrDefault(u => u.InternalName == name);
    }
}
