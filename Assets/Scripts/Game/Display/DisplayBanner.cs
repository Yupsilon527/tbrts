using UnityEngine;

public class DisplayBanner : MonoBehaviour
{
    public SpriteRenderer banner, crest;
    public void ChangeColor(Color color)
    {
        banner.color = color;
    }
    public void ChangePlayer(DataItemPlayer player)
    {
        if (player != null)
        {
            {
                if (player.faction != null)
                {
                    banner.sprite = player.faction.bannerTexture;
                    crest.sprite = player.faction.emblemTexture;
                }
                ChangeColor(player.color);
            }
        }
    }
    }
