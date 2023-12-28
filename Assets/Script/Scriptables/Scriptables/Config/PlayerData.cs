using System;
using UnityEngine;

[Serializable]
public class PlayerData

{
    public string Name;
    public string FactionName = "Random";
    public int Team;
    public Vector2Int startPosition;

    public int Gold = -1;
    public int Mana = -1;

    [NonSerialized]
    public bool Used = true;
    [NonSerialized]
    public bool Enabled = true;
    [NonSerialized]
    public bool Ready = true;
    public bool Locked = false;
    public bool LockedRace = false;
    public bool LockedTeam = false;
    public int AiLevel = 0;
}
