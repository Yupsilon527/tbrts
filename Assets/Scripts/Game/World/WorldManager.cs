using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldManager : Initializable
{
    public HashSet<CharacterSO> characters = new();
    public HashSet<WorldData> worlds = new();
    public HashSet<UnitData> units = new();
    public HashSet<DataFaction> races = new();
    public HashSet<CustomMap> maps = new();
    public static WorldManager main;
    public static WorldData world;
    protected override void Initialize()
    {
        if (main == null)
        {

            base.Initialize();
            main = this;
            LoadFromUnityData();
            LoadPresetWorld();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void LoadPresetWorld()
    {
        string preworld = PlayerPrefs.GetString("DefaultWorld");
        if (LoadWorld(preworld) is WorldData found)
        {
            world = found;
        }
        else
        {
            Inspect("No world found, loading default!");
            if (worlds.Count == 0)
            {
                Debug.LogError("No worlds loaded!");
                return;
            }

            world = worlds.FirstOrDefault();
            if (world == null)
            {
                Debug.LogError("No world was found!");
                return;
            }
            PlayerPrefs.SetString("DefaultWorld", world.InternalName);
        }
    }
    void LoadFromUnityData()
    {
        foreach (var c in Resources.LoadAll<CharacterSO>("Canon"))
        {
            characters.Add(c);
        }
        foreach (var u in Resources.LoadAll<UnitSO>("Canon"))
        {
            units.Add(u.unit);
            if (u.character != null)
                u.unit.LoadCharacter(u.character);
            else
                u.unit.LoadCharacter(LoadCharacter(u.unit.InternalName));

        }
        foreach (var f in Resources.LoadAll<FactionSO>("Canon"))
        {
            races.Add(new DataFaction(f));
        }
        foreach (var f in Resources.LoadAll<MapChunkSO>("Canon"))
        {
            maps.Add(f.MapData);
        }
        foreach (var f in Resources.LoadAll<WorldSO>("Canon"))
        {
            worlds.Add(new WorldData(f));
        }
    }
    public CharacterSO LoadCharacter(string name)
    {
        return characters.FirstOrDefault(u => u.InternalName == name);
    }
    public UnitData LoadUnit(string name)
    {
        return units.FirstOrDefault(u => u.InternalName == name);
    }
    public DataFaction LoadFaction(string name)
    {
        return races.FirstOrDefault(u => u.InternalName == name);
    }
    public WorldData LoadWorld(string name)
    {
        return worlds.FirstOrDefault(u => u.InternalName == name);
    }
}
