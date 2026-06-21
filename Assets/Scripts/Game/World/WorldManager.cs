using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldManager : Initializable
{
    HashSet<UnitData> units=new();
    HashSet<DataFaction> races=new();
    public static WorldManager main;
    public static WorldData world;
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
        foreach (var f in Resources.LoadAll<FactionSO>(""))
        {
            races.Add(new DataFaction(f));
        }
    }
    public UnitData LoadUnit(string name)
    {
        return units.FirstOrDefault(u => u.InternalName == name);
    }
    public DataFaction LoadFaction(string name)
    {
        return races.FirstOrDefault(u => u.InternalName == name);
    }
}
