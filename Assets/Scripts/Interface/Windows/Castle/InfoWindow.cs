using TMPro;
using UnityEngine.UI;

public class InfoWindow : PlayerWindow
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Button closeButton;
    public void ShowPlayerTurn(DataItemPlayer player)
    {
        Open();
        AssignPlayer(player);
        if (player.IsAiControlled())
        {
            closeButton.enabled = false;
        }
        title.text = player.Name+"'s turn";
        description.text = $"Turn {GameManager.main.currentTurn}";
    }
   public void ShowCastleInfo(DataItemCastle castle)
    {
        Open();
        AssignPlayer(castle.GetPlayerOwner());
        title.text = castle.customName;

        description.text = $"{ castle.customDescription}<br><br>Owner: {castle.GetPlayerOwner().Name} ({castle.GetPlayerOwner().faction.InternalName})";
    }
   public void ShowTileInfo(DataItemTile tile)
    {
        Open();


        if (tile.IsRevealedByPlayer(GameManager.main.playerManager.GetActivePlayer(), UnitDefines.TileVisibility.foggy))
        {
            title.text = tile.terrain.elevation.ToString();
            if (tile.regionCastle is DataItemCastle castle)
            {
                AssignPlayer(tile.regionCastle.GetPlayerOwner());
                description.text = $"Region {castle.customName}<br>Owner: {castle.GetPlayerOwner().Name} ({castle.GetPlayerOwner().faction.InternalName})";
            }
        else
            {
                ClearPlayer();
            }
        }
        else
        {
            title.text = "Unknown";
            description.text = "???<br>???";
        }
    }
}