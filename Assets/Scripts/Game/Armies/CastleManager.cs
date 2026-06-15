
using System.Collections.Generic;
using UnityEngine;

public class CastleManager : EntityManager
{
    public List<DataItemBuilding> buildings = new();

    public void redoCastleRegions()
    {
        foreach (var tData in GameManager.main.map.tiles)
        {
            float Distance = Mathf.Infinity;
            foreach (var cData in buildings)
            {
                float tDistance = (cData.gridPos - tData.gridPos).sqrMagnitude;
                if (tData.regionCastle == null || Distance > tDistance)
                {
                    tData.regionCastle = cData;
                    Distance = tDistance;
                }
            }
        }
    }

    public void DrawTheCastlesFromEditorData(CistomCastle[] customData)
    {

        foreach (CistomCastle Zim in customData)
        {
            buildings.Add(new DataItemCastle(Zim));
        }

        foreach (entityTile tData in game.TileList)
        {
            if (tData.iElevation == TileSetData.Elevation_City && tData.CityLocated == null)
            {
                game.GameCastles.Add(entityCastle.MakeFromEditorData(game, DataItemCastle.GenerateRandom(tData.Pos.x, tData.Pos.y, game.game.language, game.Scenario)));
            }
        }

        foreach (var b in buildings)
        {
            if (b is DataItemCastle castle)
            {
                var castlePrefab = GameManager.main.displayPool.PoolItem(GameManager.main.displayPool.castlePrefab);
                if (castlePrefab.TryGetComponent(out DisplayItemCastle dic))
                {
                    dic.AssignCastle(castle);
                }
            }
        }
    }
}
