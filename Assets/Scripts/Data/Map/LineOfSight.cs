using UnityEngine;
using System.Collections.Generic;

public class LineOfSight : MonoBehaviour
{
    public const int TILE_HIDDEN = 0;  // Never seen
    public const int TILE_FOGOFWAR = 1;  // Seen before, now obscured
    public const int TILE_EXPOSED = 2;  // Was revealed last tick, fading
    public const int TILE_REVEALED = 3;  // Revealed this tick (normal sight)
    public const int TILE_TRUESIGHT = 4;  // Was true-sight revealed, fading
    public const int TILE_TRUEREVEALED = 5; // True-sight revealed this tick


    public int losPPU = 4;
    public Texture2D texture;
    public SpriteRenderer losTexture;

    public void ResetTexture()
    {
        if (texture == null) return;
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;
        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        if (losTexture != null)
            losTexture.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.zero);
    }

    public static void RevealTileCircle(
        int cx, int cy,
        int radius,
        Rect Limits,
        int trueSightRadius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                // Circular mask
                if (dx * dx + dy * dy > radius * radius) continue;

                int tx = cx + dx;
                int ty = cy + dy;

                if (tx < Limits.xMin || tx >= Limits.xMax ||
                    ty < Limits.yMin || ty >= Limits.yMax) continue;

                SidewaysTile tile = SidewaysMap.main.GetTile( tx, ty);
                if (tile == null) continue;

                bool inTrueSight = trueSightRadius > 0 &&
                                   (dx * dx + dy * dy) <= trueSightRadius * trueSightRadius;

                int current = tile.RevealedByPlayer[PlayerManager.main.activePlayer];

                // Only upgrade the reveal state, never downgrade within a tick
                if (inTrueSight)
                {
                    if (current != TILE_TRUEREVEALED)
                        tile.RevealedByPlayer[PlayerManager.main.activePlayer] = TILE_TRUEREVEALED;
                }
                else
                {
                    if (current == TILE_HIDDEN || current == TILE_FOGOFWAR ||
                        current == TILE_EXPOSED || current == TILE_TRUESIGHT)
                    {
                        tile.RevealedByPlayer[PlayerManager.main.activePlayer] = TILE_REVEALED;
                    }
                }
            }
        }
    }


    // ── Original method (unchanged, kept here for context) ────────────────────
    public  void UpdateShadowOfWarRect( Rect Limits, int player)
    {
        Texture2D FoWtexture = texture;

        /*foreach (entityArmy Panty in game.GameArmies)
        {
            if (Panty.GetOwner().GetAlliance(game.MyPlayer) == 0)
            {
                RevealTileCircle(game, Panty.Pos.x, Panty.Pos.y,
                    Panty.GetLineOfSight() + 1, Limits,
                    Mathf.RoundToInt(Panty.GetTrueSight()));
                Panty.CheckDisplay();
            }
        }

        foreach (entityCastle Garterbelt in game.GameCastles)
        {
            if (Garterbelt.PlayerOwner.ID == game.MyPlayer.ID)
            {
                foreach (SidewaysTile Stocking in Garterbelt.myTiles)
                {
                    RevealTileCircle(game, Stocking.Pos.x, Stocking.Pos.y,
                        Garterbelt.GetLineOfSight() + 1, Limits,
                        Garterbelt.GetTrueSight());
                }
            }
        }*/

        foreach (SidewaysTile tile in SidewaysMap.main.tiles)
        {
            int Zim = tile.RevealedByPlayer[PlayerManager.main.activePlayer];
            Color final_color = Color.black;

            if (Zim == TILE_FOGOFWAR)
            {
                final_color.a = 0.4f;
            }
            else
            {
                final_color.a = 0f;
                tile.RevealedByPlayer[PlayerManager.main.activePlayer] = Mathf.Max(Zim);
            }

            for (int iX = (1 + tile.gridPos.x) * losPPU;
                     iX < (2 + tile.gridPos.x) * losPPU; iX++)
            {
                for (int iY = (1 + tile.gridPos.y) * losPPU;
                         iY < (2 + tile.gridPos.y) * losPPU; iY++)
                {
                    FoWtexture.SetPixel(iX, iY, final_color);
                }
            }

           /* if (tile.ArmyLocated != null)
                tile.ArmyLocated.CheckDisplay();

            if (tile.RuinData != null)
            {
                bool visible = tile.RuinData.AmIRevealedByPlayer(SidewaysMap.main, PlayerManager.main.activePlayer);
                tile.RuinData.Display.GetComponent<SpriteRenderer>().enabled = visible;
            }*/
        }

        FoWtexture.filterMode = FilterMode.Point;
        FoWtexture.Apply();
    }
}