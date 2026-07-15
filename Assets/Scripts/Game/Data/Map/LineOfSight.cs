using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public int losPPU = 4;
    public Texture2D texture;
    public SpriteRenderer losTexture;
    Color[] colors;

    public void InitTexture()
    {
        colors = new Color[SidewaysMap.main.width * SidewaysMap.main.height];
        texture = new Texture2D(SidewaysMap.main.width + 2, SidewaysMap.main.height + 2);
        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        texture.filterMode = FilterMode.Point;

        if (losTexture != null)
            losTexture.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.zero,
                1);

        transform.localPosition = Vector3.left * 1 + Vector3.down * 2;
        transform.localScale = (float)TerrainDefines.UnitsPerTile * losPPU * Vector3.one;
    }

    void ApplyChanges()
    {
        texture.SetPixels(1, 1, SidewaysMap.main.width, SidewaysMap.main.height, colors);
        texture.Apply();
    }
    void RedrawTextureForPlayer(int playerID)
    {
        for (int x = 0; x < SidewaysMap.main.width; x++)
        {
            for (int y = 0; y < SidewaysMap.main.height; y++)
            {
                var tile = SidewaysMap.main.GetTile(x, y);
                int tindex = (SidewaysMap.main.height - 1 - y)  * SidewaysMap.main.width + x;
                switch (tile.RevealedByPlayer[playerID])
                {
                    case (int)UnitDefines.TileVisibility.hidden:
                        colors[tindex] = Color.black;
                        break;
                    case (int)UnitDefines.TileVisibility.foggy:
                        colors[tindex] = new Color(0, 0, 0, .6f);
                        break;
                    default:
                        colors[tindex] = Color.clear;
                        break;

                }
            }
        }
        ApplyChanges();
    }
    public void ReviseLoSForActivePlayer()
    {
        ReviseLoSForPlayer(GameManager.main.playerManager.currentPlayer);
    }
    public void ReviseLoSForPlayer(DataItemPlayer player)
    {
        foreach (var u in player.units)
        {
            u.RevealPosition();
        }
        foreach (var u in player.buildings)
        {
            u.RevealPosition();
        }
        UpdateDisplayVisible();
        ReviseRect(new RectInt(0, 0, SidewaysMap.main.width, SidewaysMap.main.height), player.ID);
        RedrawTextureForPlayer(player.ID);
    }
    public void UpdateDisplayVisible()
    {

        foreach (var unit in GameManager.main.armyManager.armies)
            unit?.display?.OnVisibilityChange();
        foreach (var unit in GameManager.main.castleManager.buildings)
            unit?.display?.OnVisibilityChange();
    }
    void UpdateTile(DataItemTile tile, int playerId, UnitDefines.TileVisibility visibility, bool overwrite)
    {
        UpdateTile(tile, playerId, (int)visibility, overwrite);
    }
    void UpdateTile(DataItemTile tile, int playerId, int visibility, bool overwrite)
    {
        if (overwrite)
            tile.RevealedByPlayer[playerId] = visibility;
        else
            tile.RevealedByPlayer[playerId] = Mathf.Max((int)visibility, tile.RevealedByPlayer[playerId]);
    }
    void ReviseRect(RectInt rect, int playerID)
    {
        foreach (var tile in SidewaysMap.main.GetTilesInIrect(rect))
            ResetTile(tile, playerID);
    }
    void ResetTile(DataItemTile tile)
    {
        for (int player = 0; player < tile.RevealedByPlayer.Length; player++)
        {
            ResetTile(tile, player);
        }
    }
    void ResetTile(DataItemTile tile, int player)
    {
        if (tile == null) return;
        switch (tile.RevealedByPlayer[player])
        {
            case (int)UnitDefines.TileVisibility.visible:
            case (int)UnitDefines.TileVisibility.truesight:
                tile.RevealedByPlayer[player] = (int)UnitDefines.TileVisibility.foggy;
                break;
            case (int)UnitDefines.TileVisibility.revealed_visible:
            case (int)UnitDefines.TileVisibility.revealed_truesight:
                tile.RevealedByPlayer[player]--;
                break;
        }
    }
    public void RevealCircle(int playerId, Vector2Int center, int radius, UnitDefines.TileVisibility visibility)
    {
        if (visibility <= UnitDefines.TileVisibility.foggy)
            return;
        else if (visibility == UnitDefines.TileVisibility.visible)
            visibility = UnitDefines.TileVisibility.revealed_visible;
        else if (visibility == UnitDefines.TileVisibility.truesight)
            visibility = UnitDefines.TileVisibility.revealed_truesight;

        foreach (var tile in SidewaysMap.main.GetTilesInCircle(center, radius))
        {
            UpdateTile(tile, playerId, visibility, false);
        }
    }
}