
public class MapGenBiome : MapGen
{
    public override void GenerateMap()
    {
        GenerateMapFromBiomeData(biomeData, mapData);
        SidewaysMap.main.DrawTheMapFromEditorData(OutputToMapData());
    }
    public void GenerateMapFromBiomeData( MapBiomeSO biome, MapChunkSO map)
    {
        biomeData = biome;
        Initalize(map.MapData.GetWidth(), map.MapData.GetHeight());
        for (int iY = 0; iY < map.MapData.GetHeight(); iY++)
        {
            for (int iX = 0; iX < map.MapData.GetWidth(); iX++)
            {
                SetTile(iX, iY, map.MapData.GetTileAt(iX, iY, biomeData));
            }
        }
    }
}
