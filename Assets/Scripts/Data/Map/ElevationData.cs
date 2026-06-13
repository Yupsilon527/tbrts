
using System;
using UnityEngine;

[System.Serializable]
public class ElevationData
{
    [NonSerialized] public int elevationID = -1;
    public char CharID = '-';
    public TilesetSO[] TilesetVariations = new TilesetSO[0];
    public TerrainDefines.Elevation elevation = TerrainDefines.Elevation.Void;

    public Sprite GetSprite(bool[] eid, int v)
    {
        if (TilesetVariations.Length > 0)
            return TilesetVariations[Mathf.Clamp(v, 0, TilesetVariations.Length - 1)].GetSprite(eid);
        return null;
    }
    public Sprite GetSprite(int sid, int v)
    {
        if (TilesetVariations.Length > 0)
            return TilesetVariations[Mathf.Clamp(v, 0, TilesetVariations.Length - 1)].GetSprite(sid); ;
        return null;
    }
    public override string ToString()
    {
        return "Elevation " + CharID;
    }
}