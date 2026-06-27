using TMPro;
using UnityEngine;

public class CastleProductionWindow : TabWindow
{
    public DataItemCastle assignedCastle;

    public TextMeshProUGUI title, desc;
    public void SetCastle(DataItemCastle castle)
    {
        assignedCastle = castle;
        title.text = castle.customName;
        desc.text = castle.customDescription + "<br>Owner: " + (castle.GetPlayerOwner()?.Name ?? "None");

        production.castle = castle;
    }
    public CastleBuildTab build;
    public CatleProductionTab production;
    public void OpenArmyProduction()
    {
        OpenTabGameObject(build.gameObject);
        build.ShowProduction(assignedCastle.production.availableUnits.ToArray());
    }
    public void OpenBuildingProduction()
    {
        OpenTabGameObject(build.gameObject);
        build.ShowProduction(assignedCastle.GetPlayerOwner().faction.availableBuildings);
    }
}
