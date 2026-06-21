using System;

[Serializable]
public class CustomMap 
{
    public string customName, customDescription, savedVersion, assignedWorld;

    public string NeutralName;
    public string[] neutralUnits;
    public ResourceCost[] startingPlayerResources;
    public CustomPlayer[] players;

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
        int t = (x * height + y )*2 ;
        if (t >= MapData.Length)
            return new SidewaysTile();

        string readData = MapData.Substring(t, 2);

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
