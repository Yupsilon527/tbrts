using System;

[Serializable]
public class CustomMap 
{
    public string customName, customDescription, savedVersion, assignedWorld;

    public int height = 0;
    public string MapData;
    public CistomCastle[] castles;
    public CustomArmy[] armies;
    public int GetWidth()
    {
        if (MapData.Length == 0)
            return 0;
        return MapData.Length / height / 2;
    }
    public int GetHeight()
    {
        return height;
    }
    public SidewaysTile GetTileAt(int x, int y, MapBiomeSO biome)
    {
        int t = x * 2 + y * height * 2;
        if (t >= MapData.Length)
            return new SidewaysTile();

        string readData = MapData.Substring((x + y * height) * 2, 2);

        var elevation = new ElevationData();
        foreach (var e in biome.Elevations)
        {
            if (e.CharID == readData[0])
            {
                elevation = e;
                break;
            }
        }
        var swt = new SidewaysTile(elevation, int.Parse(readData[1] + ""));

        return swt;
    }
}
