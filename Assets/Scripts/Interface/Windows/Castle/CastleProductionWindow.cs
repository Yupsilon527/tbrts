using System.Linq;
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

        prodTab.castle = castle;
    }
    public CastleBuildTab buildTab;
    public CatleProductionTab prodTab;
    public void OpenArmyProduction()
    {
        OpenTabGameObject(buildTab.gameObject);
        buildTab.ShowProduction(assignedCastle.production.availableUnits.Where(u=>u.GetAvailableState(assignedCastle) != ProductionData.AvailableState.hidden).ToArray());
    }
    public void OpenBuildingProduction()
    {
        OpenTabGameObject(buildTab.gameObject);
        buildTab.ShowProduction(assignedCastle.GetPlayerOwner().faction.GetAvailableUpgrades(false).Where(u => u.GetAvailableState(assignedCastle) != ProductionData.AvailableState.hidden).ToArray());
    }
    public void QueueProduction(ProductionData p, bool ret)
    {
        assignedCastle.production.AddProduction(p, false);
        if (ret)
        OpenTabGameObject(prodTab.gameObject) ;
    }
}
