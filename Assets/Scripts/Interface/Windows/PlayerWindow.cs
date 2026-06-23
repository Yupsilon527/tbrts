using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWindow : Window
{
    DataItemPlayer assignedPlayer;
    public Image[] playerBanners, playerEmblems;
    public void AssignPlayer(DataItemPlayer p)
    {
        assignedPlayer = p;
        foreach (var banner in playerBanners)
        {
            banner.gameObject.SetActive(true);
            banner.sprite = assignedPlayer.faction.bannerTexture;
        }
        foreach (var emblem in playerEmblems)
        {
            emblem.gameObject.SetActive(true);
            emblem.sprite = assignedPlayer.faction.emblemTexture;
        }
    }
    public void ClearPlayer()
    {
        assignedPlayer = null;
        foreach (var banner in playerBanners)
        {
            banner.gameObject.SetActive(false);
        }
        foreach (var emblem in playerEmblems)
        {
            emblem.gameObject.SetActive(false);
        }
    }
}
