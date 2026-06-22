using UnityEngine;

public class PlayerDefines 
{
    public static Color[] playerColors = new Color[]
    {
        Color.gray,
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan,
    };
  public enum Alignment
    {
        enemy, ally, playerowned
    }
}
