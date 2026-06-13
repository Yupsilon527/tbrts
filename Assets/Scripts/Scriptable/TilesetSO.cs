using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "GameData/Map Generation/Tileset")]
public class TilesetSO : ScriptableObject
{
    public Sprite[] tiles;
    public Sprite GetSprite(int id)
    {
        return tiles[id];
    }
    public Sprite GetSprite(bool[] Edges)
    {
        return GetSprite(TerrainDefines.GetSprite(Edges));
    }
}
