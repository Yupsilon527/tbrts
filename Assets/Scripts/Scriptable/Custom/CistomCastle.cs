using System;
using UnityEngine;

[Serializable]
public class CistomCastle : CustomObject
{
    public string customName, customDescription, customSprite;
    public bool isCapital = false;
    public string[] producedArmies, producedBuildings;

    public static CistomCastle GenerateRandom(Vector2Int tile)
    {
        CistomCastle castle = new();
        castle.customName = "Castle";
        castle.ownership = 0;
        castle.spawnPos = tile;
        castle.customSprite = "TODO";
        return castle;
    }
}
