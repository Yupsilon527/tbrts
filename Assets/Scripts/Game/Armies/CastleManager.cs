
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastleManager : EntityManager
{
    public List<DataItemBuilding> buildings = new();

    public DataItemBuilding FindCastleByID(int ID)
    {
        return buildings.FirstOrDefault(a => a.eID == ID);
    }
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

        foreach (var tData in GameManager.main.map.tiles)
        {
            if (tData.GetWalkElevation() == TerrainDefines.Elevation.City && tData.buildingLayer == null)
            {
                buildings.Add(new DataItemCastle(CistomCastle.GenerateRandom(tData.gridPos)));
            }
        }

        foreach (var b in buildings)
        {
            if (b is DataItemCastle castle)
            {
                var castlePrefab = GameManager.main.displayPool.PoolItem(GameManager.main.displayPool.castlePrefab);
                if (castlePrefab.TryGetComponent(out DisplayItemCastle dic))
                {
                    dic.AssignObject(castle); 
                }
            }
        }
    }
    public void SelectNextIdleCity()
    {
        /*
            entityCastle Stocking = null;
            foreach (entityCastle Panty in GameCastles)
            {
                if (MyPlayer.GetAlliance(Panty.PlayerOwner) == 0 && Panty.iProduction.Count == 0 && Panty.CanProduce())
                {
                    Stocking = Panty;
                }
            }
            if (Stocking != null)
            {
                FocusCamera(new Vector3(Stocking.center.x, Stocking.center.y, 0));
                game.InGameMenus.OpenWindow(new CastleInfoWindow(game, Stocking, "info"));
            }*/
    }
}
