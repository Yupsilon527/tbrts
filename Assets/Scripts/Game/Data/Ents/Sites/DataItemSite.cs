using UnityEngine;

public class DataItemSite : DataItemBuilding
{
    public Sprite ruinSprite;
    public int visitTurn=-1;
    public BuildingDefines.RuinType ruinTypes;
    public override void OnTurnBegin()
    {
        base.OnTurnBegin();
        if (GameManager.main.currentTurn > visitTurn + BuildingDefines.RuinRefreshTurns)
        {
            visitTurn = -1;
        }
        display?.DrawAgain();
    }
    public bool IsVisited()
    {
        return visitTurn < 0;
    }
    public void OnVisited(DataItemArmy visitor)
    {
        if (IsVisited()) return;
        visitTurn = GameManager.main.currentTurn ;
        switch (ruinTypes)
        {
            case BuildingDefines.RuinType.Metal:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Metal, BuildingDefines.RuinMetalBase);
                break;
            case BuildingDefines.RuinType.Gold:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Gold, BuildingDefines.RuinGoldBase);
                break;
            case BuildingDefines.RuinType.Mana:
                visitor.GetPlayerOwner().econ.GiveResource(EconomyDefines.EconomyResource.Mana, BuildingDefines.RuinManaBase);
                break;
        }
        display?.DrawAgain();
    }
}
