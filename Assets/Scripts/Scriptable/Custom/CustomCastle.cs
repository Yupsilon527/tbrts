using System;
using UnityEngine;

[Serializable]
public class CustomCastle : CustomObject
{
    public string customName, customDescription, customSprite;
    public bool isCapital = false;
    public string[] producedArmies, producedBuildings;

    public static CustomCastle GenerateRandom(Vector2Int tile)
    {
        CustomCastle castle = new();
        castle.customName = "Castle";
        castle.ownership = 0;
        castle.spawnPos = tile;
        castle.customSprite = "TODO";
        return castle;
    }
}
