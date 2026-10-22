using System.Linq;
using UnityEngine;

public class DataItemSite : DataItemBuilding
{
    public Sprite ruinSprite;
    public int visitTurn=-1;
    public BuildingDefines.RuinType ruinType;
    public int ruinLevel;
    public DataItemSite(CustomSite custom) : base()
    {
        ChangeTile(custom.spawnPos, DisplayPositionChange.instant);
        ruinLevel = custom.RuinLevel;
        ruinType = custom.ruinType;
        InitSprite();
    }
    void InitSprite()
    {
        var possible = GameManager.main.map.mapData.biome.Sites.Where(s => s.ruin == ruinType).ToArray();
        if (possible.Length == 0) return;
        else if (possible.Any(s=>s.ruinLevel == ruinLevel) ) {
            possible = possible.Where(s => s.ruinLevel == ruinLevel).ToArray();
        }
        else if (possible.Any(s=>s.ruinLevel > ruinLevel) ) {
            possible = possible.Where(s => s.ruinLevel > ruinLevel).ToArray();
        }
        else  {
            possible = possible.Where(s => s.ruinLevel > ruinLevel).ToArray();
        }

        if (possible.Length == 0) return;
        var selected = possible[Mathf.RoundToInt(Random.value * (possible.Length - 1))];
        ruinSprite = selected.sprite;
    }
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        if (GameManager.main.currentTurn > visitTurn + BuildingDefines.RuinRefreshTurns)
        {
            visitTurn = -1;
        }
        display?.DrawAgain();
    }
    public override void ChangeTile(Vector2Int t, DisplayPositionChange position)
    {
        base.ChangeTile(t, position);
        occupiedTiles.Clear();
        occupiedTiles.Add(GameManager.main.map.GetTile(t)); 
    }
    public bool IsVisited()
    {
        return visitTurn >= 0;
    }
    public void OnVisited(DataItemBanner visitor)
    {
        if (IsVisited()) return;
        visitTurn = GameManager.main.currentTurn ;
        switch (ruinType)
        {
            case BuildingDefines.RuinType.Metal:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Metal, BuildingDefines.RuinMetalBase);
                visitor.formation.HandleEvent(AbilityDefines.Event.OnHarvestRuin);
                break;
            case BuildingDefines.RuinType.Gold:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Gold, BuildingDefines.RuinGoldBase);
                visitor.formation.HandleEvent(AbilityDefines.Event.OnHarvestRuin);
                break;
            case BuildingDefines.RuinType.Mana:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Mana, BuildingDefines.RuinManaBase);
                visitor.formation.HandleEvent(AbilityDefines.Event.OnHarvestRuin);
                break;
            case BuildingDefines.RuinType.Rest:
                visitor.movement.RefreshMovement();
                visitor.formation.HandleEvent(AbilityDefines.Event.OnRuinBoon);
                visitor.formation.HandleEvent(AbilityDefines.Event.OnRuinRest);
                break;
            case BuildingDefines.RuinType.Heal:
                foreach (var e in visitor.formation.GetUnits(true,false))
                {
                    e.health.GiveValue(e.health.GetLimit() * BuildingDefines.RuinHealBase);
                }
                visitor.formation.HandleEvent(AbilityDefines.Event.OnRuinBoon);
                visitor.formation.HandleEvent(AbilityDefines.Event.OnRuinHeal);
                break;
            case BuildingDefines.RuinType.Pray:
                visitor.formation.HandleEvent(AbilityDefines.Event.OnRuinPray);
                break;
        }
        visitor.formation.HandleEvent(AbilityDefines.Event.OnVisitRuin);
        display?.DrawAgain();
    }
}
