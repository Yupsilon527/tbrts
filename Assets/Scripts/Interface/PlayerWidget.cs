using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWidget : MonoBehaviour
{
    DataItemPlayer assignedPlayer;
    public Image[] playerBanners, playerEmblems;

    public TextMeshProUGUI label;
    public ResourceValueIndicator gold, metal, mana;
    public void AssignPlayer(DataItemPlayer p)
    {
        ClearPlayer();
        assignedPlayer = p;

        label.text = assignedPlayer.Name + "<br>" + assignedPlayer.faction.InternalName + "<br>Turn " + GameManager.main.currentTurn;

        foreach (var banner in playerBanners)
        {
            banner.gameObject.SetActive(true);
            banner.sprite = assignedPlayer.faction.bannerTexture;
            banner.color = assignedPlayer.color;
        }
        foreach (var emblem in playerEmblems)
        {
            emblem.gameObject.SetActive(true);
            emblem.sprite = assignedPlayer.faction.emblemTexture;
        }
        if (assignedPlayer != null)
        {
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal).OnValueChanged.AddListener(UpdateMetal);
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Gold).OnValueChanged.AddListener(UpdateGold);
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Mana).OnValueChanged.AddListener(UpdateMana);
            UpdateMetal();
            UpdateGold();
            UpdateMana();
        }
    }
    void ClearPlayer()
    {
        if (assignedPlayer!= null)
        {
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal).OnValueChanged.RemoveListener(UpdateMetal);
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Gold).OnValueChanged.RemoveListener(UpdateGold);
            assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Mana).OnValueChanged.RemoveListener(UpdateMana);
        }
    }
    void UpdateMetal()
    {
        var metalRes = assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal);
        metal.UpdateValue(EconomyDefines.EconomyResource.Metal, metalRes);
    }
    void UpdateGold()
    {
        var goldRes = assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Metal);
        gold.UpdateValue(EconomyDefines.EconomyResource.Gold, goldRes);
    }
    void UpdateMana()
    {
        var manaRes = assignedPlayer.econ.GetResource(EconomyDefines.EconomyResource.Mana);
        mana.UpdateValue(EconomyDefines.EconomyResource.Mana, manaRes.GetValue()+"/"+manaRes.GetLimit());
    }
}
