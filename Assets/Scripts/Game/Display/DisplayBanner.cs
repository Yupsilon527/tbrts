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
        banner.sprite = player.faction.bannerTexture;
        crest.sprite = player.faction.sigilTexture;
        ChangeColor(player.color);
    }
}
